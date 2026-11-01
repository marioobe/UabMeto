import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { dirname, resolve } from "node:path";
import test from "node:test";
import { fileURLToPath } from "node:url";

import {
  evaluateLesson,
  isIsoDate,
  lessonHasAudio,
  metadataIssues,
  publishGate
} from "../src/rules.mjs";

const testDirectory = dirname(fileURLToPath(import.meta.url));
const contentRoot = resolve(testDirectory, "../../../content");

function loadJson(relativePath) {
  return JSON.parse(readFileSync(resolve(contentRoot, relativePath), "utf8"));
}

function makeValidatedLesson() {
  return {
    schemaVersion: "1.1.0",
    id: "lesson-900",
    topicId: "topic-99",
    order: 1,
    xp: 10,
    estimatedMinutes: 5,
    title: "Pelajaran uji",
    status: "validated",
    validation: {
      reviewedBy: "reviewer-ttu-01",
      reviewedAt: "2026-10-26",
      sources: [{ id: "SRC-0001", citation: "Sumber uji" }],
      audioLicense: "CC-BY-4.0",
      consentRef: "CONSENT-0001"
    },
    objectives: ["Tujuan uji"],
    vocabulary: [
      {
        id: "vocab-001",
        dawan: "kata-uji",
        indonesian: "kata uji",
        audio: null
      }
    ],
    dialogues: [],
    exercises: [
      {
        id: "ex-001",
        type: "multiple_choice",
        skill: "reading",
        prompt: "Pilih jawaban.",
        explanation: "Penjelasan.",
        content: {
          options: [
            { id: "opt-a", text: "A" },
            { id: "opt-b", text: "B" }
          ],
          correctOptionId: "opt-a"
        }
      }
    ],
    examples: [
      {
        id: "exm-001",
        dawan: "contoh-uji",
        indonesian: "contoh uji",
        context: "situasi uji",
        status: "validated",
        sourceRef: "SRC-0001",
        audio: null
      }
    ]
  };
}

test("template UNVALIDATED lolos validasi struktur", () => {
  const lesson = loadJson("templates/lesson.template.json");
  const result = evaluateLesson(lesson);

  assert.deepEqual(result.errors, []);
});

test("contoh UNVALIDATED hanya menghasilkan warning audio, bukan error", () => {
  const lesson = loadJson("samples/lesson-001.UNVALIDATED.json");
  const result = evaluateLesson(lesson, {
    scope: "samples",
    audioFileExists: () => false
  });

  assert.deepEqual(result.errors, []);
  assert.ok(result.warnings.length >= 1, "audio yang belum ada harus menjadi warning");
});

test("UNVALIDATED dengan teks Dawan non-placeholder ditolak", () => {
  const lesson = loadJson("templates/lesson.template.json");
  lesson.vocabulary[0].dawan = "kata-karangan";

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("wajib memakai placeholder")));
});

test("status UNVALIDATED di folder lessons ditolak", () => {
  const lesson = loadJson("templates/lesson.template.json");
  const result = evaluateLesson(lesson, { scope: "lessons" });

  assert.ok(result.errors.some((message) => message.includes("hanya boleh berada di folder samples")));
});

test("file samples dengan status selain UNVALIDATED ditolak", () => {
  const lesson = loadJson("templates/lesson.template.json");
  lesson.status = "draft";

  const result = evaluateLesson(lesson, { scope: "samples" });

  assert.ok(result.errors.some((message) => message.includes("wajib berstatus")));
});

test("konten validated lengkap lolos metadata dan gate", () => {
  const lesson = makeValidatedLesson();

  assert.deepEqual(metadataIssues(lesson), []);
  const gate = publishGate(lesson);
  assert.equal(gate.allowed, true, gate.reasons.join(" | "));
});

test("konten validated tanpa reviewer ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.validation.reviewedBy = null;

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("reviewedBy")));
});

test("konten validated yang masih berisi placeholder ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.vocabulary[0].dawan = "[PLACEHOLDER-DWN-001]";

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("placeholder tidak boleh ada")));
});

test("tanggal tinjauan tidak valid ditolak", () => {
  assert.equal(isIsoDate("2026-02-31"), false);
  assert.equal(isIsoDate("26/10/2026"), false);
  assert.equal(isIsoDate("2026-10-26"), true);

  const lesson = makeValidatedLesson();
  lesson.validation.reviewedAt = "2026-02-31";

  assert.ok(metadataIssues(lesson).some((message) => message.includes("reviewedAt")));
});

test("audio non-ogg ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.vocabulary[0].audio = {
    file: "audio/lesson-900/vocab-001.mp3",
    durationMs: 1000,
    license: "CC-BY-4.0",
    consentRef: "CONSENT-0001"
  };

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("Ogg Vorbis")));
});

test("pelajaran beraudio tanpa consentRef ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.vocabulary[0].audio = {
    file: "audio/lesson-900/vocab-001.ogg",
    durationMs: 1000,
    license: "CC-BY-4.0",
    consentRef: "CONSENT-0001"
  };
  lesson.validation.consentRef = null;

  assert.equal(lessonHasAudio(lesson), true);
  assert.ok(metadataIssues(lesson).some((message) => message.includes("consentRef")));
});

test("soal listening tanpa audioRef ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.exercises[0] = {
    id: "ex-002",
    type: "listening",
    skill: "listening",
    prompt: "Dengarkan.",
    explanation: "Penjelasan.",
    content: {
      options: [
        { id: "opt-a", text: "A" },
        { id: "opt-b", text: "B" }
      ],
      correctOptionId: "opt-a"
    }
  };

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("audioRef wajib")));
});

test("sentence_order dengan token tidak konsisten ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.exercises[0] = {
    id: "ex-003",
    type: "sentence_order",
    skill: "writing",
    prompt: "Susun.",
    explanation: "Penjelasan.",
    content: {
      tokens: ["a", "b", "c"],
      correctOrder: ["a", "b", "d"]
    }
  };

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("token yang sama")));
});

test("id soal duplikat ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.exercises.push(structuredClone(lesson.exercises[0]));

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("duplikat")));
});

test("pilihan ganda tanpa explanation ditolak", () => {
  const lesson = makeValidatedLesson();
  delete lesson.exercises[0].explanation;

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("explanation wajib")));
});

test("gate penerbitan menolak konten draft", () => {
  const lesson = makeValidatedLesson();
  lesson.status = "draft";

  const gate = publishGate(lesson);

  assert.equal(gate.allowed, false);
  assert.ok(gate.reasons.some((message) => message.includes('status harus "validated"')));
});

test("schemaVersion tidak dikenal ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.schemaVersion = "2.0.0";

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("schemaVersion")));
});

test("skema 1.0.0 dapat dibaca tetapi tidak dapat diterbitkan", () => {
  const lesson = makeValidatedLesson();
  lesson.schemaVersion = "1.0.0";
  delete lesson.xp;
  delete lesson.estimatedMinutes;
  delete lesson.examples;

  const result = evaluateLesson(lesson);
  assert.deepEqual(result.errors, []);
  assert.ok(result.warnings.some((message) => message.includes("legacy")));

  const gate = publishGate(lesson);
  assert.equal(gate.allowed, false);
  assert.ok(gate.reasons.some((message) => message.includes("1.1.0")));
});

test("xp hilang pada skema 1.1.0 ditolak", () => {
  const lesson = makeValidatedLesson();
  delete lesson.xp;

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("xp")));
});

test("xp negatif ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.xp = -1;

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("xp")));
});

test("estimatedMinutes nol ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.estimatedMinutes = 0;

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("estimatedMinutes")));
});

test("examples hilang pada skema 1.1.0 ditolak", () => {
  const lesson = makeValidatedLesson();
  delete lesson.examples;

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("examples")));
});

test("pelajaran validated tanpa contoh kalimat ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.examples = [];

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("contoh kalimat")));
});

test("contoh validated tanpa sourceRef ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.examples[0].sourceRef = null;

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("sourceRef")));
});

test("contoh validated berstatus UNVALIDATED ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.examples[0].status = "UNVALIDATED";

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("status harus")));
});

test("pelajaran UNVALIDATED dengan contoh validated ditolak", () => {
  const lesson = loadJson("templates/lesson.template.json");
  lesson.examples[0].status = "validated";

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes('status harus "UNVALIDATED"')));
});

test("id contoh duplikat ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.examples.push(structuredClone(lesson.examples[0]));

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("duplikat")));
});

test("audio contoh non-ogg ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.examples[0].audio = {
    file: "audio/lesson-900/exm-001.mp3",
    durationMs: 1000,
    license: "CC-BY-4.0",
    consentRef: "CONSENT-0001"
  };

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("Ogg Vorbis")));
});

test("sourceRef contoh harus berpola SRC", () => {
  const lesson = makeValidatedLesson();
  lesson.examples[0].sourceRef = "src-0001";

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes("berpola SRC")));
});

test("contoh published pada pelajaran validated ditolak", () => {
  const lesson = makeValidatedLesson();
  lesson.examples[0].status = "published";

  const result = evaluateLesson(lesson);

  assert.ok(result.errors.some((message) => message.includes('tidak boleh "published"')));
});

test("lessonHasAudio mendeteksi audio pada contoh", () => {
  const lesson = makeValidatedLesson();
  lesson.examples[0].audio = {
    file: "audio/lesson-900/exm-001.ogg",
    durationMs: 1000,
    license: "CC-BY-4.0",
    consentRef: "CONSENT-0001"
  };

  assert.equal(lessonHasAudio(lesson), true);
});
