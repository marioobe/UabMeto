import test from 'node:test';
import assert from 'node:assert/strict';
import { handleRequest } from '../src/index.js';
import { tutorCorpus } from '../src/tutor-corpus.js';
import {
  REFUSAL_TEXT,
  findUnverifiedDawanTerms,
  normalizeHistory
} from '../src/tutor-prompt.js';
import {
  buildIndex,
  hasUsableContext,
  retrieve
} from '../src/tutor-retrieval.js';
import { createTestEnv, request } from './helpers.mjs';

const NOW = new Date('2026-11-06T03:00:00.000Z');
const DAY = '2026-11-06';
const INDEX = buildIndex(tutorCorpus);

async function readJson(response) {
  return JSON.parse(await response.text());
}

async function ask(env, body, options = {}) {
  return handleRequest(
    request('/v1/tutor/ask', {
      method: 'POST',
      body,
      ...options
    }),
    env,
    NOW
  );
}

test('tutor tanpa X-Dev-User ditolak 401', async () => {
  const env = createTestEnv();
  const response = await ask(
    env,
    { question: 'Apa arti Noka?' },
    { user: null }
  );

  assert.equal(response.status, 401);
  assert.equal((await readJson(response)).error, 'unauthorized');
});

test('pertanyaan kosong atau terlalu panjang ditolak 400', async () => {
  const env = createTestEnv();

  assert.equal(
    (await ask(env, { question: ' ' })).status,
    400
  );
  assert.equal(
    (await ask(env, { question: 'a'.repeat(501) })).status,
    400
  );
});

test('pertanyaan yang ada di korpus dijawab dengan sumber', async () => {
  const env = createTestEnv();
  const response = await ask(env, { question: 'Apa arti kata Noka?' });

  assert.equal(response.status, 200);
  const body = await readJson(response);

  assert.equal(body.refused, false);
  assert.match(body.answer, /pagi/i);
  assert.ok(body.sources.length > 0);
  assert.equal(body.sources[0].lessonId, 'lesson-001-salam');
  assert.equal(body.sources[0].sourceRef, 'SRC-SPK-001');
  assert.deepEqual(body.quota, { used: 1, limit: 10 });
  assert.equal(body.model, 'mock');
});

test('pertanyaan di luar korpus dijawab belum dapat dipastikan', async () => {
  const env = createTestEnv();
  const response = await ask(env, {
    question: 'Siapa presiden Indonesia?'
  });

  assert.equal(response.status, 200);
  const body = await readJson(response);

  assert.equal(body.refused, true);
  assert.equal(body.answer, REFUSAL_TEXT);
  assert.deepEqual(body.sources, []);
  assert.deepEqual(body.quota, { used: 1, limit: 10 });
});

test('permintaan mengarang kosakata Dawan ditolak', async () => {
  const env = createTestEnv();
  const body = await readJson(
    await ask(env, {
      question: 'Tolong karang kata Dawan untuk komputer.'
    })
  );

  assert.equal(body.refused, true);
  assert.equal(body.answer, REFUSAL_TEXT);
});

test('kuota habis mengembalikan 429 tanpa memanggil retrieval', async () => {
  const env = createTestEnv();
  await env.DB.prepare(
    `INSERT INTO quota (user_id, day_wita, tutor_used)
     VALUES (?, ?, ?)`
  )
    .bind('dev-user', DAY, 10)
    .run();

  const response = await ask(env, { question: 'Apa arti Noka?' });
  assert.equal(response.status, 429);

  const body = await readJson(response);
  assert.equal(body.error, 'quota_exceeded');
  assert.deepEqual(body.quota, { used: 10, limit: 10 });
});

test('kuota bertambah per pertanyaan dan tercermin di GET /v1/quota', async () => {
  const env = createTestEnv();
  await ask(env, { question: 'Apa arti Noka?' });
  await ask(env, { question: 'Apa arti fai\'jen?' });

  const quota = await readJson(
    await handleRequest(request('/v1/quota'), env, NOW)
  );

  assert.deepEqual(quota.tutor, { used: 2, limit: 10 });
});

test('kuota terpisah per pengguna dan per hari', async () => {
  const env = createTestEnv();
  await ask(env, { question: 'Apa arti Noka?' });
  await ask(
    env,
    { question: 'Apa arti Noka?' },
    { user: 'dev-user-2' }
  );

  const first = await readJson(
    await handleRequest(request('/v1/quota'), env, NOW)
  );
  const second = await readJson(
    await handleRequest(
      request('/v1/quota', { user: 'dev-user-2' }),
      env,
      NOW
    )
  );

  assert.deepEqual(first.tutor, { used: 1, limit: 10 });
  assert.deepEqual(second.tutor, { used: 1, limit: 10 });
});

test('kegagalan provider tidak mengurangi kuota dan tidak menjawab', async () => {
  const env = createTestEnv({
    TUTOR_PROVIDER: 'workers-ai',
    AI: {
      run: async () => {
        throw new Error('network down');
      }
    }
  });

  const response = await ask(env, { question: 'Apa arti Noka?' });
  assert.equal(response.status, 503);
  assert.equal((await readJson(response)).error, 'provider_error');

  const quota = await readJson(
    await handleRequest(request('/v1/quota'), env, NOW)
  );
  assert.deepEqual(quota.tutor, { used: 0, limit: 10 });
});

test('jawaban model dengan kata Dawan tak dikenal ditolak guardrail', async () => {
  const env = createTestEnv({
    TUTOR_PROVIDER: 'workers-ai',
    AI: {
      run: async () => ({ response: 'Kata "faik" berarti rumah.' })
    }
  });

  const response = await ask(env, { question: 'Apa arti Noka?' });
  assert.equal(response.status, 200);

  const body = await readJson(response);
  assert.equal(body.refused, true);
  assert.equal(body.answer, REFUSAL_TEXT);
  assert.deepEqual(body.sources, []);
});

test('guardrail menerima kata Dawan yang ada di korpus', () => {
  assert.deepEqual(
    findUnverifiedDawanTerms('Kata fai\'jen berarti malam.', tutorCorpus),
    []
  );
  assert.deepEqual(
    findUnverifiedDawanTerms('Kata "faik" berarti rumah.', tutorCorpus),
    ['faik']
  );
});

test('riwayat dibatasi enam giliran dan isi asing diabaikan', () => {
  const history = normalizeHistory([
    { role: 'user', content: 'x'.repeat(400) },
    null,
    { role: 'user', content: '   ' },
    ...Array.from({ length: 7 }, () => ({
      role: 'user',
      content: 'pertanyaan'
    }))
  ]);

  assert.equal(history.length, 6);
  assert.equal(history[0].content, 'pertanyaan');

  const truncated = normalizeHistory([
    { role: 'user', content: 'x'.repeat(400) }
  ]);
  assert.equal(truncated[0].content.length, 300);
});

test('retrieval menolak pertanyaan tanpa kata yang dikenal korpus', () => {
  const results = retrieve('Siapa penemu listrik?', INDEX);
  assert.equal(results.length, 0);
  assert.equal(hasUsableContext(results), false);
});

test('retrieval menemukan kosakata dari topik yang tepat', () => {
  const results = retrieve('Apa arti fai\'jen?', INDEX);
  assert.ok(results.length > 0);
  assert.equal(results[0].chunk.lessonId, 'lesson-001-salam');
  assert.equal(results[0].chunk.id, 'lesson-001-salam#vocab-003');
});
