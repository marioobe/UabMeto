import test from 'node:test';
import assert from 'node:assert/strict';
import { handleRequest } from '../src/index.js';
import {
  createTestEnv,
  request,
  seedInReviewLesson,
  seedValidatedLesson,
  syncBody
} from './helpers.mjs';

// 2026-11-06 11:00 WITA (Jumat). Awal minggu = 2026-11-02.
const NOW = new Date('2026-11-06T03:00:00.000Z');

async function readJson(response) {
  return JSON.parse(await response.text());
}

async function sync(env, overrides = {}, options = {}) {
  return handleRequest(
    request('/v1/sync', {
      method: 'POST',
      body: syncBody(overrides),
      ...options
    }),
    env,
    NOW
  );
}

test('health melaporkan mode lokal', async () => {
  const env = createTestEnv();
  const response = await handleRequest(request('/health'), env, NOW);
  assert.equal(response.status, 200);

  const body = await readJson(response);
  assert.equal(body.ok, true);
  assert.equal(body.mode, 'test');
});

test('sync tanpa X-Dev-User ditolak 401', async () => {
  const env = createTestEnv();
  const response = await handleRequest(
    request('/v1/sync', {
      method: 'POST',
      body: syncBody(),
      user: null
    }),
    env,
    NOW
  );

  assert.equal(response.status, 401);
  assert.equal((await readJson(response)).error, 'unauthorized');
});

test('sync pertama memberi XP dari content_versions, bukan klaim klien', async () => {
  const env = createTestEnv();
  await seedValidatedLesson(env, 'lesson-test', 10);

  const response = await sync(env);
  assert.equal(response.status, 200);

  const body = await readJson(response);
  assert.equal(body.state, 'confirmed');
  assert.equal(body.awardedXp, 10);
  assert.equal(body.duplicate, false);
  assert.equal(body.devMode, false);
});

test('operationId yang sama dengan payload sama tidak menggandakan XP', async () => {
  const env = createTestEnv();
  await seedValidatedLesson(env, 'lesson-test', 10);

  const first = await readJson(await sync(env));
  const second = await readJson(await sync(env));

  assert.equal(first.awardedXp, 10);
  assert.equal(second.awardedXp, 10);
  assert.equal(second.duplicate, true);

  const leaderboard = await readJson(
    await handleRequest(
      request('/v1/leaderboard'),
      env,
      NOW
    )
  );

  assert.equal(leaderboard.entries.length, 1);
  assert.equal(leaderboard.entries[0].xp, 10);
});

test('operationId yang sama dengan payload berbeda ditolak 409', async () => {
  const env = createTestEnv();
  await seedValidatedLesson(env, 'lesson-test', 10);

  assert.equal((await sync(env)).status, 200);

  const conflict = await sync(env, { clientClaimedXp: 1 });
  assert.equal(conflict.status, 409);
  assert.equal((await readJson(conflict)).error, 'operation_conflict');
});

test('lesson yang tidak dikenal ditolak 422', async () => {
  const env = createTestEnv();
  const response = await sync(env, { lessonId: 'lesson-tidak-ada' });

  assert.equal(response.status, 422);
  assert.equal((await readJson(response)).error, 'unknown_content');
});

test('konten in_review ditolak tanpa flag dev', async () => {
  const env = createTestEnv();
  await seedInReviewLesson(env);
  const response = await sync(env, {
    lessonId: 'lesson-in-review-test',
    operationId: 'op-in-review'
  });

  assert.equal(response.status, 422);
  assert.equal((await readJson(response)).error, 'content_not_validated');
});

test('flag dev mengizinkan konten in_review dan menandai devMode', async () => {
  const env = createTestEnv({ DEV_ALLOW_XP_FOR_IN_REVIEW: 'true' });
  await seedInReviewLesson(env);
  const response = await sync(env, {
    lessonId: 'lesson-in-review-test',
    operationId: 'op-in-review-dev'
  });

  assert.equal(response.status, 200);
  const body = await readJson(response);
  assert.equal(body.state, 'confirmed');
  assert.equal(body.devMode, true);
  assert.equal(body.awardedXp, 10);
});

test('sync dapat dicek lewat GET /v1/sync/{operationId}', async () => {
  const env = createTestEnv();
  await seedValidatedLesson(env, 'lesson-test', 10);
  await sync(env);

  const response = await handleRequest(
    request('/v1/sync/op-00001'),
    env,
    NOW
  );
  assert.equal(response.status, 200);

  const body = await readJson(response);
  assert.equal(body.state, 'confirmed');
  assert.equal(body.awardedXp, 10);
});

test('kuota mengembalikan hari WITA dan mengabaikan hari lain', async () => {
  const env = createTestEnv();
  await env.DB.prepare(
    `INSERT INTO quota (user_id, day_wita, tutor_used, conversation_used, pronunciation_used)
     VALUES (?, ?, ?, ?, ?)`
  )
    .bind('dev-user', '2026-11-06', 4, 1, 2)
    .run();
  await env.DB.prepare(
    `INSERT INTO quota (user_id, day_wita, tutor_used)
     VALUES (?, ?, ?)`
  )
    .bind('dev-user', '2026-11-05', 9)
    .run();

  const response = await handleRequest(
    request('/v1/quota'),
    env,
    NOW
  );
  assert.equal(response.status, 200);

  const body = await readJson(response);
  assert.equal(body.dayWita, '2026-11-06');
  assert.deepEqual(body.tutor, { used: 4, limit: 10 });
  assert.deepEqual(body.conversation, { used: 1, limit: 3 });
  assert.deepEqual(body.pronunciation, { used: 2, limit: 10 });
});

test('leaderboard mingguan diurutkan berdasarkan XP', async () => {
  const env = createTestEnv();
  await seedValidatedLesson(env, 'lesson-test', 10);
  await seedValidatedLesson(env, 'lesson-test-2', 30);

  await sync(env, {
    operationId: 'op-0000a',
    lessonId: 'lesson-test',
    nickname: 'Aina'
  });
  await sync(
    env,
    {
      operationId: 'op-0000b',
      lessonId: 'lesson-test-2',
      nickname: 'Am'
    },
    { user: 'dev-user-2' }
  );

  const response = await handleRequest(
    request('/v1/leaderboard'),
    env,
    NOW
  );
  assert.equal(response.status, 200);

  const body = await readJson(response);
  assert.equal(body.weekKey, '2026-11-02');
  assert.deepEqual(
    body.entries.map((entry) => [entry.userId, entry.xp]),
    [
      ['dev-user-2', 30],
      ['dev-user', 10]
    ]
  );
});
