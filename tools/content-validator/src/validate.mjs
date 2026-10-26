#!/usr/bin/env node
/**
 * CLI validator konten Uab Meto.
 *
 * Pemakaian:
 *   node src/validate.mjs [--root <dir>] [--strict]
 *
 * - Memindai <root>/lessons/**\/*.json dan <root>/samples/**\/*.json.
 * - File di samples wajib berstatus UNVALIDATED.
 * - Keluar dengan kode 1 bila ada error.
 */

import { existsSync, readFileSync, readdirSync, statSync } from "node:fs";
import { dirname, join, relative, resolve, sep } from "node:path";
import { fileURLToPath } from "node:url";
import { evaluateLesson, publishGate } from "./rules.mjs";

const scriptDirectory = dirname(fileURLToPath(import.meta.url));
const defaultRoot = resolve(scriptDirectory, "../../../content");

function parseArguments(argv) {
  const options = { root: defaultRoot, strict: false, help: false };

  for (let index = 0; index < argv.length; index += 1) {
    const argument = argv[index];
    if (argument === "--root") {
      const value = argv[index + 1];
      if (!value) {
        throw new Error("--root memerlukan nilai direktori.");
      }
      options.root = resolve(value);
      index += 1;
    } else if (argument === "--strict") {
      options.strict = true;
    } else if (argument === "--help" || argument === "-h") {
      options.help = true;
    } else {
      throw new Error(`Argumen tidak dikenal: ${argument}`);
    }
  }

  return options;
}

function collectJsonFiles(directory) {
  if (!existsSync(directory)) {
    return [];
  }

  const results = [];
  for (const entry of readdirSync(directory, { withFileTypes: true })) {
    const fullPath = join(directory, entry.name);
    if (entry.isDirectory()) {
      results.push(...collectJsonFiles(fullPath));
    } else if (entry.isFile() && entry.name.toLowerCase().endsWith(".json")) {
      results.push(fullPath);
    }
  }

  return results;
}

function scopeFor(filePath, root) {
  const relativePath = relative(root, filePath);
  const firstSegment = relativePath.split(sep)[0];
  return firstSegment === "samples" ? "samples" : "lessons";
}

function main() {
  const options = parseArguments(process.argv.slice(2));

  if (options.help) {
    console.log("Usage: node src/validate.mjs [--root <dir>] [--strict]");
    return 0;
  }

  const root = options.root;
  if (!existsSync(root)) {
    console.error(`Root konten tidak ditemukan: ${root}`);
    return 2;
  }

  const files = [
    ...collectJsonFiles(join(root, "lessons")),
    ...collectJsonFiles(join(root, "samples"))
  ].sort();

  console.log("Uab Meto content validator");
  console.log(`Root   : ${root}`);
  console.log(`Files  : ${files.length}`);
  console.log("");

  const lessonIds = new Map();
  const orderKeys = new Map();
  let totalErrors = 0;
  let totalWarnings = 0;
  let failedFiles = 0;

  for (const filePath of files) {
    const displayPath = relative(root, filePath).split(sep).join("/");
    const scope = scopeFor(filePath, root);

    let lesson;
    try {
      lesson = JSON.parse(readFileSync(filePath, "utf8"));
    } catch (error) {
      totalErrors += 1;
      failedFiles += 1;
      console.log(`[ERROR] ${displayPath}`);
      console.log(`  - JSON tidak valid: ${error.message}`);
      continue;
    }

    const { errors, warnings } = evaluateLesson(lesson, {
      scope,
      audioFileExists: (file) => existsSync(join(root, file))
    });

    if (typeof lesson?.id === "string") {
      if (lessonIds.has(lesson.id)) {
        errors.push(`id pelajaran duplikat dengan ${lessonIds.get(lesson.id)}.`);
      } else {
        lessonIds.set(lesson.id, displayPath);
      }
    }

    if (typeof lesson?.topicId === "string" && Number.isInteger(lesson?.order)) {
      const key = `${lesson.topicId}#${lesson.order}`;
      if (orderKeys.has(key)) {
        errors.push(`urutan ${key} duplikat dengan ${orderKeys.get(key)}.`);
      } else {
        orderKeys.set(key, displayPath);
      }
    }

    if (scope === "samples") {
      const gate = publishGate(lesson);
      if (gate.allowed) {
        errors.push("konten contoh lolos gate penerbitan; ini tidak boleh terjadi.");
      }
    }

    totalWarnings += warnings.length;

    if (errors.length > 0) {
      totalErrors += errors.length;
      failedFiles += 1;
      console.log(`[ERROR] ${displayPath}`);
      for (const message of errors) {
        console.log(`  - ${message}`);
      }
    } else if (warnings.length > 0) {
      console.log(`[WARN ] ${displayPath}`);
      for (const message of warnings) {
        console.log(`  - ${message}`);
      }
    } else {
      console.log(`[OK   ] ${displayPath}`);
    }
  }

  console.log("");
  console.log(
    `Summary: ${files.length} file, ${totalErrors} error, ${totalWarnings} warning, ${failedFiles} file gagal.`
  );

  if (options.strict && totalWarnings > 0) {
    console.log("RESULT: FAIL (--strict: warning dianggap error)");
    return 1;
  }

  if (totalErrors > 0) {
    console.log("RESULT: FAIL");
    return 1;
  }

  console.log("RESULT: PASS");
  return 0;
}

process.exitCode = main();
