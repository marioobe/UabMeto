import { contentSeed } from './content-seed.js';

// Worker mode online LOKAL. Tidak ada autentikasi Firebase, tidak ada
// Firestore, dan tidak boleh di-deploy. Lihat docs/architecture/local-online-mode.md.

const JSON_HEADERS = {
  'content-type': 'application/json; charset=utf-8',
  'access-control-allow-origin': '*',
  'cache-control': 'no-store'
};

const CORS_HEADERS = {
  'access-control-allow-origin': '*',
  'access-control-allow-headers': 'content-type, x-dev-user',
  'access-control-allow-methods': 'GET, POST, OPTIONS'
};

const QUOTA_LIMITS = {
  tutor: 10,
  conversation: 3,
  pronunciation: 10
};

const WITA_OFFSET_MS = 8 * 60 * 60 * 1000;

export default {
  fetch(request, env) {
    return handleRequest(request, env);
  }
};

export async function handleRequest(request, env, now = new Date()) {
  try {
    const url = new URL(request.url);

    if (request.method === 'OPTIONS') {
      return new Response(null, { status: 204, headers: CORS_HEADERS });
    }

    if (url.pathname === '/health' && request.method === 'GET') {
      return json({
        ok: true,
        mode: env.APP_ENV ?? 'local',
        time: now.toISOString()
      });
    }

    if (url.pathname === '/v1/sync' && request.method === 'POST') {
      return await handleSync(request, env, now);
    }

    if (url.pathname.startsWith('/v1/sync/') && request.method === 'GET') {
      return await handleSyncStatus(request, env, url);
    }

    if (url.pathname === '/v1/quota' && request.method === 'GET') {
      return await handleQuota(request, env, url, now);
    }

    if (url.pathname === '/v1/leaderboard' && request.method === 'GET') {
      return await handleLeaderboard(request, env, url, now);
    }

    return json({ error: 'not_found' }, 404);
  } catch (error) {
    console.error('[uabmeto-local] request failed', error);
    return json({ error: 'internal_error' }, 500);
  }
}

async function handleSync(request, env, now) {
  const user = getDevUser(request);
  if (!user) {
    return json(
      { error: 'unauthorized', message: 'Header X-Dev-User wajib di mode lokal.' },
      401
    );
  }

  let body;
  try {
    body = await request.json();
  } catch {
    return json({ error: 'invalid_json' }, 400);
  }

  const input = normalizeSyncInput(body);
  if (typeof input === 'string') {
    return json({ error: input }, 400);
  }

  await seedContent(env.DB);

  const content = await env.DB.prepare(
    'SELECT xp, status FROM content_versions WHERE lesson_id = ? AND content_version = ?'
  )
    .bind(input.lessonId, input.contentVersion)
    .first();

  if (!content) {
    return json(
      {
        error: 'unknown_content',
        lessonId: input.lessonId,
        contentVersion: input.contentVersion
      },
      422
    );
  }

  const devMode = env.DEV_ALLOW_XP_FOR_IN_REVIEW === 'true';
  if (content.status !== 'validated' && !devMode) {
    return json(
      {
        error: 'content_not_validated',
        status: content.status,
        message: 'XP hanya boleh diberikan untuk konten validated.'
      },
      422
    );
  }

  const payloadHash = await sha256(
    JSON.stringify({
      userId: user,
      lessonId: input.lessonId,
      contentVersion: input.contentVersion,
      clientClaimedXp: input.clientClaimedXp
    })
  );

  const timestamp = now.toISOString();

  await env.DB.prepare(
    `INSERT INTO operations
       (operation_id, user_id, lesson_id, content_version, payload_hash,
        client_claimed_xp, awarded_xp, state, created_at, updated_at)
     VALUES (?, ?, ?, ?, ?, ?, 0, 'reserved', ?, ?)
     ON CONFLICT(operation_id) DO NOTHING`
  )
    .bind(
      input.operationId,
      user,
      input.lessonId,
      input.contentVersion,
      payloadHash,
      input.clientClaimedXp,
      timestamp,
      timestamp
    )
    .run();

  const operation = await env.DB.prepare(
    'SELECT * FROM operations WHERE operation_id = ?'
  )
    .bind(input.operationId)
    .first();

  if (!operation) {
    return json({ error: 'internal_error' }, 500);
  }

  if (operation.user_id !== user || operation.payload_hash !== payloadHash) {
    return json(
      {
        error: 'operation_conflict',
        message: 'operationId sudah dipakai untuk payload lain.'
      },
      409
    );
  }

  if (operation.state === 'confirmed') {
    return json({
      state: 'confirmed',
      awardedXp: operation.awarded_xp,
      duplicate: true,
      devMode
    });
  }

  if (operation.state === 'rejected') {
    return json({
      state: 'rejected',
      awardedXp: 0,
      duplicate: true,
      devMode
    });
  }

  const awardedXp = content.xp;
  const applied = await env.DB.prepare(
    'SELECT awarded_xp FROM applied_effects WHERE operation_id = ?'
  )
    .bind(input.operationId)
    .first();

  if (!applied) {
    const insert = await env.DB.prepare(
      `INSERT INTO applied_effects
         (operation_id, user_id, nickname, lesson_id, awarded_xp, applied_at)
       VALUES (?, ?, ?, ?, ?, ?)
       ON CONFLICT(operation_id) DO NOTHING`
    )
      .bind(
        input.operationId,
        user,
        input.nickname || user,
        input.lessonId,
        awardedXp,
        timestamp
      )
      .run();

    if (insert.meta.changes > 0) {
      // Efek baru. Proyeksi leaderboard dihitung dari applied_effects saat
      // dibaca, jadi tidak ada penulisan kedua yang bisa menggandakan XP.
      console.log(
        `[uabmeto-local] applied operation=${input.operationId} ` +
          `user=${user} lesson=${input.lessonId} xp=${awardedXp}`
      );
    }
  }

  await env.DB.prepare(
    "UPDATE operations SET state = 'confirmed', awarded_xp = ?, updated_at = ? WHERE operation_id = ?"
  )
    .bind(awardedXp, timestamp, input.operationId)
    .run();

  return json({
    state: 'confirmed',
    awardedXp,
    duplicate: false,
    devMode
  });
}

async function handleSyncStatus(request, env, url) {
  const user = getDevUser(request);
  if (!user) {
    return json({ error: 'unauthorized' }, 401);
  }

  const operationId = decodeURIComponent(url.pathname.slice('/v1/sync/'.length));
  if (!operationId) {
    return json({ error: 'invalid_operation_id' }, 400);
  }

  const row = await env.DB.prepare(
    `SELECT operation_id, user_id, lesson_id, state, awarded_xp, updated_at
     FROM operations WHERE operation_id = ?`
  )
    .bind(operationId)
    .first();

  if (!row || row.user_id !== user) {
    return json({ error: 'not_found' }, 404);
  }

  return json({
    operationId: row.operation_id,
    lessonId: row.lesson_id,
    state: row.state,
    awardedXp: row.awarded_xp,
    updatedAt: row.updated_at
  });
}

async function handleQuota(request, env, url, now) {
  const user = getDevUser(request);
  if (!user) {
    return json({ error: 'unauthorized' }, 401);
  }

  const day = url.searchParams.get('day') ?? dayKey(now);
  if (!/^\d{4}-\d{2}-\d{2}$/.test(day)) {
    return json({ error: 'invalid_day' }, 400);
  }

  const row = await env.DB.prepare(
    `SELECT tutor_used, conversation_used, pronunciation_used
     FROM quota WHERE user_id = ? AND day_wita = ?`
  )
    .bind(user, day)
    .first();

  return json({
    dayWita: day,
    tutor: {
      used: row?.tutor_used ?? 0,
      limit: QUOTA_LIMITS.tutor
    },
    conversation: {
      used: row?.conversation_used ?? 0,
      limit: QUOTA_LIMITS.conversation
    },
    pronunciation: {
      used: row?.pronunciation_used ?? 0,
      limit: QUOTA_LIMITS.pronunciation
    }
  });
}

async function handleLeaderboard(request, env, url, now) {
  const user = getDevUser(request);
  if (!user) {
    return json({ error: 'unauthorized' }, 401);
  }

  const requested = url.searchParams.get('week');
  let key = weekKey(now);
  if (requested) {
    if (!/^\d{4}-\d{2}-\d{2}$/.test(requested)) {
      return json({ error: 'invalid_week' }, 400);
    }

    key = requested;
  }

  const start = new Date(`${key}T00:00:00+08:00`);
  if (Number.isNaN(start.getTime())) {
    return json({ error: 'invalid_week' }, 400);
  }

  const end = new Date(start.getTime() + 7 * 24 * 60 * 60 * 1000);

  const rows = await env.DB.prepare(
    `SELECT e.user_id AS userId,
            (SELECT nickname FROM applied_effects e2
              WHERE e2.user_id = e.user_id
              ORDER BY e2.applied_at DESC LIMIT 1) AS nickname,
            SUM(e.awarded_xp) AS xp
     FROM applied_effects e
     WHERE e.applied_at >= ? AND e.applied_at < ?
     GROUP BY e.user_id
     ORDER BY xp DESC, userId ASC
     LIMIT 50`
  )
    .bind(start.toISOString(), end.toISOString())
    .all();

  return json({
    weekKey: key,
    entries: rows.results ?? []
  });
}

async function seedContent(DB) {
  for (const item of contentSeed) {
    await DB.prepare(
      `INSERT INTO content_versions (lesson_id, content_version, xp, status)
       VALUES (?, ?, ?, ?)
       ON CONFLICT(lesson_id, content_version) DO UPDATE SET
         xp = excluded.xp,
         status = excluded.status`
    )
      .bind(item.lessonId, item.contentVersion, item.xp, item.status)
      .run();
  }
}

function normalizeSyncInput(body) {
  if (!body || typeof body !== 'object') {
    return 'invalid_body';
  }

  const operationId = stringField(body.operationId, 8, 64);
  if (!operationId || !/^[A-Za-z0-9_-]+$/.test(operationId)) {
    return 'invalid_operation_id';
  }

  const lessonId = stringField(body.lessonId, 1, 100);
  if (!lessonId) {
    return 'invalid_lesson_id';
  }

  const contentVersion = stringField(body.contentVersion, 1, 20);
  if (!contentVersion) {
    return 'invalid_content_version';
  }

  const clientClaimedXp = body.clientClaimedXp;
  if (
    !Number.isInteger(clientClaimedXp) ||
    clientClaimedXp < 0 ||
    clientClaimedXp > 1000
  ) {
    return 'invalid_client_claimed_xp';
  }

  const nickname =
    typeof body.nickname === 'string'
      ? body.nickname.trim().slice(0, 40)
      : '';

  return {
    operationId,
    lessonId,
    contentVersion,
    clientClaimedXp,
    nickname
  };
}

function stringField(value, min, max) {
  if (typeof value !== 'string') {
    return null;
  }

  const trimmed = value.trim();
  if (trimmed.length < min || trimmed.length > max) {
    return null;
  }

  return trimmed;
}

function getDevUser(request) {
  const value = request.headers.get('x-dev-user');
  if (!value) {
    return null;
  }

  const trimmed = value.trim();
  if (!/^[A-Za-z0-9._-]{3,64}$/.test(trimmed)) {
    return null;
  }

  return trimmed;
}

function json(data, status = 200) {
  return new Response(JSON.stringify(data), {
    status,
    headers: JSON_HEADERS
  });
}

async function sha256(value) {
  const bytes = new TextEncoder().encode(value);
  const digest = await crypto.subtle.digest('SHA-256', bytes);
  return [...new Uint8Array(digest)]
    .map((byte) => byte.toString(16).padStart(2, '0'))
    .join('');
}

function witaDate(now) {
  return new Date(now.getTime() + WITA_OFFSET_MS);
}

function dayKey(now) {
  return witaDate(now).toISOString().slice(0, 10);
}

function weekKey(now) {
  const wita = witaDate(now);
  const day = wita.getUTCDay();
  const diff = (day + 6) % 7;
  const monday = new Date(
    Date.UTC(wita.getUTCFullYear(), wita.getUTCMonth(), wita.getUTCDate() - diff)
  );
  return monday.toISOString().slice(0, 10);
}
