#!/usr/bin/env node
/**
 * Migrasi mekanis pelajaran dari skema 1.0.0 ke 1.1.0.
 *
 * Prinsip:
 * - Tidak menambah, mengubah, atau mengarang teks Dawan.
 * - Tidak menaikkan status validasi.
 * - Konten validated/published harus diturunkan ke in_review secara eksplisit.
 * - Nilai xp, estimatedMinutes, dan examples yang kosong diisi nilai netral
 *   dan wajib dilengkapi manusia sebelum terbit.
 *
 * Pemakaian:
 *   node src/migrate.mjs --in <file> --out <file> [--demote-to-in-review]
 */

import { readFileSync, writeFileSync } from "node:fs";
import { CURRENT_SCHEMA_VERSION, evaluateLesson } from "./rules.mjs";

export function migrateLesson(lesson, options = {}) {
  if (lesson === null || typeof lesson !== "object" || Array.isArray(lesson)) {
    throw new Error("root harus objek JSON.");
  }

  if (lesson.schemaVersion === CURRENT_SCHEMA_VERSION) {
    throw new Error(`sudah memakai skema ${CURRENT_SCHEMA_VERSION}.`);
  }

  if (lesson.schemaVersion !== "1.0.0") {
    throw new Error(`versi tidak didukung: ${lesson.schemaVersion}.`);
  }

  const isPublished = lesson.status === "validated" || lesson.status === "published";
  if (isPublished && !options.demoteToInReview) {
    throw new Error(
      "konten validated/published menolak migrasi otomatis. " +
        "Pakai --demote-to-in-review agar status diturunkan dan direview ulang."
    );
  }

  const migrated = structuredClone(lesson);
  const warnings = [];

  migrated.schemaVersion = CURRENT_SCHEMA_VERSION;

  if (migrated.xp === undefined) {
    migrated.xp = 0;
    warnings.push("xp diisi 0 (netral). Isi nilai nyata sebelum terbit.");
  }

  if (migrated.estimatedMinutes === undefined) {
    migrated.estimatedMinutes = 1;
    warnings.push("estimatedMinutes diisi 1 (netral). Isi perkiraan nyata sebelum terbit.");
  }

  if (migrated.examples === undefined) {
    migrated.examples = [];
    warnings.push("examples kosong. Tambahkan contoh bersumber sebelum terbit.");
  }

  if (isPublished && options.demoteToInReview) {
    migrated.status = "in_review";
    warnings.push("status diturunkan ke in_review. Review manusia wajib diulang.");
  }

  return { lesson: migrated, warnings };
}

function parseArguments(argv) {
  const options = { input: null, output: null, demoteToInReview: false, help: false };

  for (let index = 0; index < argv.length; index += 1) {
    const argument = argv[index];
    if (argument === "--in") {
      options.input = argv[index + 1];
      index += 1;
    } else if (argument === "--out") {
      options.output = argv[index + 1];
      index += 1;
    } else if (argument === "--demote-to-in-review") {
      options.demoteToInReview = true;
    } else if (argument === "--help" || argument === "-h") {
      options.help = true;
    } else {
      throw new Error(`argumen tidak dikenal: ${argument}`);
    }
  }

  return options;
}

function main() {
  let options;

  try {
    options = parseArguments(process.argv.slice(2));
  } catch (error) {
    console.error(`[migrate] ${error.message}`);
    process.exit(1);
  }

  if (options.help || !options.input || !options.output) {
    console.log(
      "Pemakaian: node src/migrate.mjs --in <file> --out <file> [--demote-to-in-review]"
    );
    process.exit(options.help ? 0 : 1);
  }

  try {
    const source = JSON.parse(readFileSync(options.input, "utf8"));
    const { lesson, warnings } = migrateLesson(source, {
      demoteToInReview: options.demoteToInReview
    });

    const result = evaluateLesson(lesson, { scope: "lessons" });
    writeFileSync(options.output, `${JSON.stringify(lesson, null, 2)}\n`, "utf8");

    for (const warning of warnings) {
      console.warn(`[migrate] PERINGATAN: ${warning}`);
    }

    if (result.errors.length > 0) {
      console.error("[migrate] Hasil migrasi belum valid:");
      for (const error of result.errors) {
        console.error(`  - ${error}`);
      }
      process.exit(2);
    }

    console.log(`[migrate] Ditulis: ${options.output}`);
    console.log("[migrate] Migrasi mekanis selesai. Validasi manusia tetap wajib.");
  } catch (error) {
    console.error(`[migrate] ${error.message}`);
    process.exit(1);
  }
}

if (process.argv[1] && process.argv[1].endsWith("migrate.mjs")) {
  main();
}
