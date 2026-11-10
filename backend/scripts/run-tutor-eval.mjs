import { readFileSync } from 'node:fs';
import { join } from 'node:path';
import { handleRequest } from '../src/index.js';
import { tutorCorpus } from '../src/tutor-corpus.js';
import {
  REFUSAL_TEXT,
  findUnverifiedDawanTerms
} from '../src/tutor-prompt.js';
import { createTestEnv, request } from '../test/helpers.mjs';

// Menjalankan set emas Tutor AI terhadap Worker lokal tanpa jaringan.
// Provider default adalah mock; evaluasi model Workers AI yang sebenarnya
// dilakukan manual dengan TUTOR_PROVIDER=workers-ai dan catatan terpisah.
const NOW = new Date('2026-11-06T03:00:00.000Z');
const goldenPath = join(
  import.meta.dirname,
  '..',
  '..',
  'content',
  'eval',
  'tutor-golden.json'
);

const golden = JSON.parse(readFileSync(goldenPath, 'utf8'));
const env = createTestEnv();
const failures = [];

async function readJson(response) {
  return JSON.parse(await response.text());
}

function fail(id, message) {
  failures.push({ id, message });
  console.log(`FAIL ${id}: ${message}`);
}

for (const testCase of golden.cases) {
  const user = `eval-${testCase.id}`;
  const response = await handleRequest(
    request('/v1/tutor/ask', {
      method: 'POST',
      user,
      body: { question: testCase.question }
    }),
    env,
    NOW
  );

  if (response.status !== 200) {
    fail(testCase.id, `status ${response.status}`);
    continue;
  }

  const body = await readJson(response);
  const isRefusal = body.refused === true;

  if (testCase.expect === 'refuse') {
    if (!isRefusal) {
      fail(testCase.id, 'seharusnya menolak, tetapi menjawab');
      continue;
    }

    if (body.answer !== REFUSAL_TEXT) {
      fail(testCase.id, 'teks penolakan tidak sesuai');
      continue;
    }

    if ((body.sources ?? []).length !== 0) {
      fail(testCase.id, 'penolakan tidak boleh membawa sumber');
      continue;
    }

    console.log(`PASS ${testCase.id}: menolak dengan benar`);
    continue;
  }

  if (isRefusal) {
    fail(testCase.id, 'menolak padahal materi tersedia');
    continue;
  }

  if (!body.answer || body.answer.length < 20) {
    fail(testCase.id, 'jawaban terlalu pendek');
    continue;
  }

  const sources = body.sources ?? [];
  if (sources.length === 0) {
    fail(testCase.id, 'jawaban tanpa sumber');
    continue;
  }

  if (!sources.some((source) => source.lessonId === testCase.lessonId)) {
    fail(
      testCase.id,
      `sumber tidak memuat ${testCase.lessonId} (${sources
        .map((source) => source.lessonId)
        .join(', ')})`
    );
    continue;
  }

  const unverified = findUnverifiedDawanTerms(body.answer, tutorCorpus);
  if (unverified.length > 0) {
    fail(testCase.id, `kata Dawan tak tervalidasi: ${unverified.join(', ')}`);
    continue;
  }

  if (/PLACEHOLDER|UNVALIDATED/i.test(body.answer)) {
    fail(testCase.id, 'jawaban memuat penanda placeholder');
    continue;
  }

  console.log(`PASS ${testCase.id}: menjawab dari ${testCase.lessonId}`);
}

const total = golden.cases.length;
const passed = total - failures.length;
console.log(
  `\nTutor AI eval: ${passed}/${total} lulus, ${failures.length} gagal ` +
    `(provider=${env.TUTOR_PROVIDER ?? 'mock'}).`
);

if (failures.length > 0) {
  process.exitCode = 1;
}
