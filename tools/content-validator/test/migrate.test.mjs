import assert from "node:assert/strict";
import test from "node:test";

import { evaluateLesson } from "../src/rules.mjs";
import { migrateLesson } from "../src/migrate.mjs";

function legacyLesson(overrides = {}) {
  return {
    schemaVersion: "1.0.0",
    id: "lesson-901",
    topicId: "topic-99",
    order: 1,
    title: "Pelajaran legacy",
    status: "UNVALIDATED",
    validation: {
      reviewedBy: null,
      reviewedAt: null,
      sources: [],
      audioLicense: null,
      consentRef: null
    },
    objectives: ["[PLACEHOLDER-OBJ-901]"],
    vocabulary: [
      {
        id: "vocab-001",
        dawan: "[PLACEHOLDER-DWN-901]",
        indonesian: "[PLACEHOLDER-IDN-901]",
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
            { id: "opt-a", text: "[PLACEHOLDER-OPT-901]" },
            { id: "opt-b", text: "[PLACEHOLDER-OPT-902]" }
          ],
          correctOptionId: "opt-a"
        }
      }
    ],
    ...overrides
  };
}

test("migrasi 1.0.0 menambah field netral tanpa mengubah teks", () => {
  const source = legacyLesson();
  const before = JSON.stringify(source);

  const { lesson, warnings } = migrateLesson(source);

  assert.equal(lesson.schemaVersion, "1.1.0");
  assert.equal(lesson.xp, 0);
  assert.equal(lesson.estimatedMinutes, 1);
  assert.deepEqual(lesson.examples, []);
  assert.equal(lesson.status, "UNVALIDATED");
  assert.equal(JSON.stringify(source), before, "sumber tidak boleh dimutasi");
  assert.ok(warnings.length >= 3);

  const result = evaluateLesson(lesson, { scope: "samples" });
  assert.deepEqual(result.errors, []);
});

test("migrasi menolak konten validated tanpa penurunan status", () => {
  const source = legacyLesson({ status: "validated" });

  assert.throws(() => migrateLesson(source), /demote-to-in-review/);
});

test("migrasi validated dengan flag menurunkan status ke in_review", () => {
  const source = legacyLesson({ status: "validated" });

  const { lesson, warnings } = migrateLesson(source, { demoteToInReview: true });

  assert.equal(lesson.status, "in_review");
  assert.ok(warnings.some((message) => message.includes("in_review")));
});

test("migrasi tidak mengarang contoh kalimat", () => {
  const { lesson } = migrateLesson(legacyLesson());

  assert.deepEqual(lesson.examples, []);
});

test("migrasi menolak versi yang tidak didukung", () => {
  const source = legacyLesson({ schemaVersion: "1.1.0" });

  assert.throws(() => migrateLesson(source), /sudah memakai skema/);
});
