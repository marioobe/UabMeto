/**
 * Aturan validasi konten Uab Meto.
 *
 * Prinsip:
 * - Konten berstatus UNVALIDATED hanya boleh berisi placeholder, bukan kata Dawan karangan.
 * - Konten validated/published tidak boleh berisi placeholder.
 * - Penerbitan hanya boleh dari status "validated" dengan metadata lengkap.
 *
 * Aturan ini harus konsisten dengan UabMeto.Core.Content.PublishGate (C#).
 */

export const LESSON_STATUSES = ["UNVALIDATED", "draft", "in_review", "validated", "published"];
export const EXERCISE_TYPES = ["multiple_choice", "match", "sentence_order", "listening", "writing", "speaking"];
export const SKILLS = ["reading", "writing", "listening", "speaking"];

const PLACEHOLDER = /^\[PLACEHOLDER-[A-Z0-9]+(-[A-Z0-9]+)*\]$/;
const PLACEHOLDER_ANYWHERE = /\[PLACEHOLDER/i;
const OGG_PATH = /^audio\/[A-Za-z0-9._/-]+\.ogg$/;
const PATTERNS = {
  lesson: /^lesson-[a-z0-9]+(-[a-z0-9]+)*$/,
  topic: /^topic-[a-z0-9]+(-[a-z0-9]+)*$/,
  vocab: /^vocab-[a-z0-9]+(-[a-z0-9]+)*$/,
  dialogue: /^dlg-[a-z0-9]+(-[a-z0-9]+)*$/,
  exercise: /^ex-[a-z0-9]+(-[a-z0-9]+)*$/,
  source: /^SRC-[A-Z0-9]+(-[A-Z0-9]+)*$/
};

const EXERCISE_CONTENT_KEYS = {
  multiple_choice: ["options", "correctOptionId"],
  match: ["pairs"],
  sentence_order: ["tokens", "correctOrder"],
  listening: ["audioRef", "options", "correctOptionId"],
  writing: ["expectedAnswer"],
  speaking: ["targetAudioRef"]
};

const isObject = (value) => typeof value === "object" && value !== null && !Array.isArray(value);
const isNonEmptyString = (value) => typeof value === "string" && value.trim().length > 0;

export function isIsoDate(value) {
  if (!isNonEmptyString(value) || !/^\d{4}-\d{2}-\d{2}$/.test(value)) {
    return false;
  }

  const parsed = new Date(`${value}T00:00:00Z`);
  return !Number.isNaN(parsed.getTime()) && parsed.toISOString().slice(0, 10) === value;
}

function* walkStrings(node, path) {
  if (typeof node === "string") {
    yield { path, value: node };
    return;
  }

  if (Array.isArray(node)) {
    for (let index = 0; index < node.length; index += 1) {
      yield* walkStrings(node[index], `${path}[${index}]`);
    }
    return;
  }

  if (!isObject(node)) {
    return;
  }

  for (const [key, value] of Object.entries(node)) {
    yield* walkStrings(value, path ? `${path}.${key}` : key);
  }
}

function* walkLanguageTexts(node, path) {
  if (Array.isArray(node)) {
    for (let index = 0; index < node.length; index += 1) {
      yield* walkLanguageTexts(node[index], `${path}[${index}]`);
    }
    return;
  }

  if (!isObject(node)) {
    return;
  }

  for (const [key, value] of Object.entries(node)) {
    const nextPath = path ? `${path}.${key}` : key;
    if ((key === "dawan" || key === "indonesian") && typeof value === "string") {
      yield { path: nextPath, value };
      continue;
    }

    yield* walkLanguageTexts(value, nextPath);
  }
}

export function lessonHasAudio(lesson) {
  const vocabularyAudio = Array.isArray(lesson?.vocabulary)
    ? lesson.vocabulary.some((item) => isObject(item) && isObject(item.audio))
    : false;

  const dialogueAudio = Array.isArray(lesson?.dialogues)
    ? lesson.dialogues.some(
        (dialogue) =>
          isObject(dialogue) &&
          Array.isArray(dialogue.lines) &&
          dialogue.lines.some((line) => isObject(line) && isObject(line.audio))
      )
    : false;

  const exerciseAudio = Array.isArray(lesson?.exercises)
    ? lesson.exercises.some((exercise) => isObject(exercise) && exercise.type === "listening")
    : false;

  return vocabularyAudio || dialogueAudio || exerciseAudio;
}

function checkAudioRef(audio, path, errors, warnings, options) {
  if (audio === null || audio === undefined) {
    return;
  }

  if (!isObject(audio)) {
    errors.push(`${path}: audio harus objek atau null.`);
    return;
  }

  if (!isNonEmptyString(audio.file)) {
    errors.push(`${path}.file wajib diisi.`);
  } else if (!OGG_PATH.test(audio.file)) {
    errors.push(`${path}.file harus path relatif berawalan "audio/" dan berakhiran ".ogg" (Ogg Vorbis).`);
  } else if (audio.file.includes("..")) {
    errors.push(`${path}.file tidak boleh mengandung "..".`);
  } else if (typeof options.audioFileExists === "function" && !options.audioFileExists(audio.file)) {
    const message = `${path}.file tidak ditemukan: ${audio.file}`;
    if (options.requireAudioFiles) {
      errors.push(message);
    } else {
      warnings.push(message);
    }
  }

  if (!Number.isInteger(audio.durationMs) || audio.durationMs <= 0) {
    errors.push(`${path}.durationMs harus bilangan bulat > 0.`);
  }

  if (audio.bitrateKbps !== undefined) {
    if (!Number.isInteger(audio.bitrateKbps) || audio.bitrateKbps < 8 || audio.bitrateKbps > 128) {
      errors.push(`${path}.bitrateKbps harus bilangan bulat 8-128.`);
    }
  }

  if (!isNonEmptyString(audio.license)) {
    errors.push(`${path}.license wajib diisi.`);
  }

  if (!isNonEmptyString(audio.consentRef)) {
    errors.push(`${path}.consentRef wajib diisi.`);
  }
}

function checkOptions(content, path, errors) {
  if (!Array.isArray(content.options) || content.options.length < 2) {
    errors.push(`${path}.options minimal dua pilihan.`);
    return;
  }

  const ids = new Set();
  for (let index = 0; index < content.options.length; index += 1) {
    const option = content.options[index];
    const optionPath = `${path}.options[${index}]`;
    if (!isObject(option)) {
      errors.push(`${optionPath}: harus objek.`);
      continue;
    }

    if (!isNonEmptyString(option.id)) {
      errors.push(`${optionPath}.id wajib diisi.`);
    } else if (ids.has(option.id)) {
      errors.push(`${optionPath}.id duplikat: ${option.id}.`);
    } else {
      ids.add(option.id);
    }

    if (!isNonEmptyString(option.text)) {
      errors.push(`${optionPath}.text wajib diisi.`);
    }
  }

  if (!isNonEmptyString(content.correctOptionId) || !ids.has(content.correctOptionId)) {
    errors.push(`${path}.correctOptionId harus merujuk salah satu opsi.`);
  }
}

function checkExercise(exercise, index, errors, warnings, options) {
  const path = `exercises[${index}]`;

  if (!isObject(exercise)) {
    errors.push(`${path}: harus objek.`);
    return;
  }

  if (!isNonEmptyString(exercise.id)) {
    errors.push(`${path}.id wajib diisi.`);
  } else if (!PATTERNS.exercise.test(exercise.id)) {
    errors.push(`${path}.id tidak sesuai pola: ${exercise.id}.`);
  }

  if (!EXERCISE_TYPES.includes(exercise.type)) {
    errors.push(`${path}.type tidak dikenal: ${exercise.type}.`);
    return;
  }

  if (!SKILLS.includes(exercise.skill)) {
    errors.push(`${path}.skill tidak dikenal: ${exercise.skill}.`);
  }

  if (!isNonEmptyString(exercise.prompt)) {
    errors.push(`${path}.prompt wajib diisi.`);
  }

  if (!isObject(exercise.content)) {
    errors.push(`${path}.content harus objek.`);
    return;
  }

  for (const key of EXERCISE_CONTENT_KEYS[exercise.type]) {
    if (exercise.content[key] === undefined) {
      errors.push(`${path}.content.${key} wajib untuk tipe ${exercise.type}.`);
    }
  }

  const content = exercise.content;
  const contentPath = `${path}.content`;

  switch (exercise.type) {
    case "multiple_choice":
      checkOptions(content, contentPath, errors);
      if (!isNonEmptyString(exercise.explanation)) {
        errors.push(`${path}.explanation wajib untuk soal pilihan ganda.`);
      }
      break;

    case "listening":
      if (!isNonEmptyString(content.audioRef) || !OGG_PATH.test(content.audioRef)) {
        errors.push(`${contentPath}.audioRef harus path .ogg.`);
      } else if (
        typeof options.audioFileExists === "function" &&
        !options.audioFileExists(content.audioRef)
      ) {
        const message = `${contentPath}.audioRef tidak ditemukan: ${content.audioRef}`;
        if (options.requireAudioFiles) {
          errors.push(message);
        } else {
          warnings.push(message);
        }
      }
      checkOptions(content, contentPath, errors);
      if (!isNonEmptyString(exercise.explanation)) {
        errors.push(`${path}.explanation wajib untuk soal mendengarkan.`);
      }
      break;

    case "match": {
      if (!Array.isArray(content.pairs) || content.pairs.length < 2) {
        errors.push(`${contentPath}.pairs minimal dua pasangan.`);
        break;
      }

      const left = new Set();
      const right = new Set();
      content.pairs.forEach((pair, pairIndex) => {
        if (!isObject(pair) || !isNonEmptyString(pair.leftId) || !isNonEmptyString(pair.rightId)) {
          errors.push(`${contentPath}.pairs[${pairIndex}] harus punya leftId dan rightId.`);
          return;
        }

        if (left.has(pair.leftId) || right.has(pair.rightId)) {
          errors.push(`${contentPath}.pairs[${pairIndex}] duplikat.`);
        }

        left.add(pair.leftId);
        right.add(pair.rightId);
      });
      break;
    }

    case "sentence_order": {
      if (!Array.isArray(content.tokens) || content.tokens.length < 2) {
        errors.push(`${contentPath}.tokens minimal dua token.`);
        break;
      }

      if (content.tokens.some((token) => !isNonEmptyString(token))) {
        errors.push(`${contentPath}.tokens harus berisi string tidak kosong.`);
      }

      if (!Array.isArray(content.correctOrder) || content.correctOrder.length !== content.tokens.length) {
        errors.push(`${contentPath}.correctOrder harus sepanjang tokens.`);
        break;
      }

      const sortedTokens = [...content.tokens].sort();
      const sortedOrder = [...content.correctOrder].sort();
      if (JSON.stringify(sortedTokens) !== JSON.stringify(sortedOrder)) {
        errors.push(`${contentPath}.correctOrder harus memakai token yang sama dengan tokens.`);
      }
      break;
    }

    case "writing":
      if (!isNonEmptyString(content.expectedAnswer)) {
        errors.push(`${contentPath}.expectedAnswer wajib diisi.`);
      }
      break;

    case "speaking":
      if (!isNonEmptyString(content.targetAudioRef) || !OGG_PATH.test(content.targetAudioRef)) {
        errors.push(`${contentPath}.targetAudioRef harus path .ogg.`);
      } else if (
        typeof options.audioFileExists === "function" &&
        !options.audioFileExists(content.targetAudioRef)
      ) {
        const message = `${contentPath}.targetAudioRef tidak ditemukan: ${content.targetAudioRef}`;
        if (options.requireAudioFiles) {
          errors.push(message);
        } else {
          warnings.push(message);
        }
      }
      break;

    default:
      break;
  }
}

/**
 * Memeriksa kelengkapan metadata validasi. Mengembalikan daftar masalah.
 * Dipakai oleh gate penerbitan dan pemeriksaan integritas konten terbit.
 */
export function metadataIssues(lesson) {
  const issues = [];

  if (!isObject(lesson)) {
    return ["root bukan objek JSON"];
  }

  const validation = lesson.validation;
  if (!isObject(validation)) {
    return ["metadata validasi tidak ada"];
  }

  if (!isNonEmptyString(validation.reviewedBy)) {
    issues.push("reviewedBy belum diisi");
  }

  if (!isIsoDate(validation.reviewedAt)) {
    issues.push("reviewedAt harus ISO-8601 (YYYY-MM-DD)");
  }

  if (!Array.isArray(validation.sources) || validation.sources.length === 0) {
    issues.push("minimal satu sumber referensi diperlukan");
  } else {
    validation.sources.forEach((source, index) => {
      if (!isObject(source)) {
        issues.push(`sources[${index}] harus objek`);
        return;
      }

      if (!isNonEmptyString(source.id) || !PATTERNS.source.test(source.id)) {
        issues.push(`sources[${index}].id harus berpola SRC-...`);
      }

      if (!isNonEmptyString(source.citation)) {
        issues.push(`sources[${index}].citation wajib diisi`);
      }
    });
  }

  if (!isNonEmptyString(validation.audioLicense)) {
    issues.push("audioLicense belum diisi");
  }

  if (lessonHasAudio(lesson) && !isNonEmptyString(validation.consentRef)) {
    issues.push("consentRef wajib untuk pelajaran beraudio");
  }

  return issues;
}

/**
 * Gate penerbitan: hanya konten berstatus "validated" dengan metadata lengkap
 * dan tanpa placeholder yang boleh diterbitkan.
 */
export function publishGate(lesson) {
  const reasons = [];

  if (!isObject(lesson)) {
    return { allowed: false, reasons: ["root bukan objek JSON"] };
  }

  if (lesson.status === "UNVALIDATED") {
    reasons.push("status UNVALIDATED");
  }

  if (lesson.status !== "validated") {
    reasons.push(`status harus "validated", ditemukan "${lesson.status}"`);
  }

  reasons.push(...metadataIssues(lesson));

  for (const text of walkStrings(lesson, "")) {
    if (PLACEHOLDER_ANYWHERE.test(text.value)) {
      reasons.push(`placeholder ditemukan di ${text.path}`);
    }
  }

  return { allowed: reasons.length === 0, reasons };
}

/**
 * Evaluasi lengkap satu pelajaran.
 *
 * @param {unknown} lesson
 * @param {{ scope?: "lessons"|"samples", audioFileExists?: (file: string) => boolean, requireAudioFiles?: boolean }} [options]
 */
export function evaluateLesson(lesson, options = {}) {
  const errors = [];
  const warnings = [];

  if (!isObject(lesson)) {
    return { errors: ["root harus objek JSON"], warnings };
  }

  if (lesson.schemaVersion !== "1.0.0") {
    errors.push(`schemaVersion harus "1.0.0", ditemukan "${lesson.schemaVersion}".`);
  }

  if (!isNonEmptyString(lesson.id) || !PATTERNS.lesson.test(lesson.id)) {
    errors.push(`id pelajaran tidak sesuai pola: ${lesson.id}.`);
  }

  if (!isNonEmptyString(lesson.topicId) || !PATTERNS.topic.test(lesson.topicId)) {
    errors.push(`topicId tidak sesuai pola: ${lesson.topicId}.`);
  }

  if (!Number.isInteger(lesson.order) || lesson.order < 1) {
    errors.push("order harus bilangan bulat >= 1.");
  }

  if (!isNonEmptyString(lesson.title)) {
    errors.push("title wajib diisi.");
  }

  if (!LESSON_STATUSES.includes(lesson.status)) {
    errors.push(`status tidak dikenal: ${lesson.status}.`);
  }

  if (options.scope === "samples" && lesson.status !== "UNVALIDATED") {
    errors.push('file di folder samples wajib berstatus "UNVALIDATED".');
  }

  if (options.scope === "lessons" && lesson.status === "UNVALIDATED") {
    errors.push('status "UNVALIDATED" hanya boleh berada di folder samples.');
  }

  if (!Array.isArray(lesson.objectives) || lesson.objectives.length === 0) {
    errors.push("objectives minimal satu butir.");
  } else if (lesson.objectives.some((item) => !isNonEmptyString(item))) {
    errors.push("objectives harus berisi string tidak kosong.");
  }

  if (!isObject(lesson.validation)) {
    errors.push("validation wajib objek.");
  } else {
    for (const key of ["reviewedBy", "reviewedAt", "sources", "audioLicense", "consentRef"]) {
      if (!(key in lesson.validation)) {
        errors.push(`validation.${key} wajib ada (boleh null).`);
      }
    }

    if (!Array.isArray(lesson.validation.sources)) {
      errors.push("validation.sources harus array.");
    }
  }

  const requireAudioFiles = lesson.status === "validated" || lesson.status === "published";
  const audioOptions = { ...options, requireAudioFiles };

  if (!Array.isArray(lesson.vocabulary)) {
    errors.push("vocabulary harus array.");
  } else {
    const vocabularyIds = new Set();
    lesson.vocabulary.forEach((item, index) => {
      const path = `vocabulary[${index}]`;
      if (!isObject(item)) {
        errors.push(`${path}: harus objek.`);
        return;
      }

      if (!isNonEmptyString(item.id) || !PATTERNS.vocab.test(item.id)) {
        errors.push(`${path}.id tidak sesuai pola.`);
      } else if (vocabularyIds.has(item.id)) {
        errors.push(`${path}.id duplikat: ${item.id}.`);
      } else {
        vocabularyIds.add(item.id);
      }

      if (!isNonEmptyString(item.dawan)) {
        errors.push(`${path}.dawan wajib diisi.`);
      }

      if (!isNonEmptyString(item.indonesian)) {
        errors.push(`${path}.indonesian wajib diisi.`);
      }

      if (!("audio" in item)) {
        errors.push(`${path}.audio wajib ada (boleh null).`);
      } else {
        checkAudioRef(item.audio, `${path}.audio`, errors, warnings, audioOptions);
      }
    });
  }

  if (!Array.isArray(lesson.dialogues)) {
    errors.push("dialogues harus array.");
  } else {
    const dialogueIds = new Set();
    lesson.dialogues.forEach((dialogue, index) => {
      const path = `dialogues[${index}]`;
      if (!isObject(dialogue)) {
        errors.push(`${path}: harus objek.`);
        return;
      }

      if (!isNonEmptyString(dialogue.id) || !PATTERNS.dialogue.test(dialogue.id)) {
        errors.push(`${path}.id tidak sesuai pola.`);
      } else if (dialogueIds.has(dialogue.id)) {
        errors.push(`${path}.id duplikat: ${dialogue.id}.`);
      } else {
        dialogueIds.add(dialogue.id);
      }

      if (!isNonEmptyString(dialogue.title)) {
        errors.push(`${path}.title wajib diisi.`);
      }

      if (!Array.isArray(dialogue.lines) || dialogue.lines.length === 0) {
        errors.push(`${path}.lines minimal satu baris.`);
        return;
      }

      dialogue.lines.forEach((line, lineIndex) => {
        const linePath = `${path}.lines[${lineIndex}]`;
        if (!isObject(line)) {
          errors.push(`${linePath}: harus objek.`);
          return;
        }

        for (const key of ["speaker", "dawan", "indonesian"]) {
          if (!isNonEmptyString(line[key])) {
            errors.push(`${linePath}.${key} wajib diisi.`);
          }
        }

        if (!("audio" in line)) {
          errors.push(`${linePath}.audio wajib ada (boleh null).`);
        } else {
          checkAudioRef(line.audio, `${linePath}.audio`, errors, warnings, audioOptions);
        }
      });
    });
  }

  if (!Array.isArray(lesson.exercises) || lesson.exercises.length === 0) {
    errors.push("exercises minimal satu soal.");
  } else {
    const exerciseIds = new Set();
    lesson.exercises.forEach((exercise, index) => {
      checkExercise(exercise, index, errors, warnings, audioOptions);
      if (isObject(exercise) && isNonEmptyString(exercise.id)) {
        if (exerciseIds.has(exercise.id)) {
          errors.push(`exercises[${index}].id duplikat: ${exercise.id}.`);
        } else {
          exerciseIds.add(exercise.id);
        }
      }
    });
  }

  // Kebijakan placeholder.
  if (lesson.status === "UNVALIDATED") {
    for (const text of walkLanguageTexts(lesson, "")) {
      if (!PLACEHOLDER.test(text.value)) {
        errors.push(
          `${text.path}: konten UNVALIDATED wajib memakai placeholder [PLACEHOLDER-...], ditemukan teks lain.`
        );
      }
    }
  } else {
    for (const text of walkStrings(lesson, "")) {
      if (PLACEHOLDER_ANYWHERE.test(text.value)) {
        errors.push(`${text.path}: placeholder tidak boleh ada pada konten berstatus ${lesson.status}.`);
      }
    }
  }

  // Integritas konten yang sudah divalidasi/diterbitkan.
  if (lesson.status === "validated" || lesson.status === "published") {
    for (const issue of metadataIssues(lesson)) {
      errors.push(`validasi belum lengkap: ${issue}.`);
    }
  }

  if (lesson.status === "UNVALIDATED") {
    const gate = publishGate(lesson);
    if (gate.allowed) {
      errors.push("konten UNVALIDATED tidak boleh lolos gate penerbitan.");
    }
  }

  return { errors, warnings };
}
