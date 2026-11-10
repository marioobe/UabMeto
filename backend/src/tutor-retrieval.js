// Pencarian BM25 sederhana di atas korpus Tutor AI. Tidak memakai vector
// search: korpus F1 masih kecil, dan arsitektur menunda vector search sampai
// korpus cukup besar (docs/architecture/overview.md §8).

const STOPWORDS = new Set([
  'yang',
  'dan',
  'di',
  'ke',
  'dari',
  'itu',
  'ini',
  'dengan',
  'untuk',
  'pada',
  'adalah',
  'saya',
  'aku',
  'kamu',
  'anda',
  'nya',
  'kah',
  'lah',
  'sih',
  'dong',
  'boleh',
  'bagaimana',
  'mengapa',
  'kenapa',
  'apakah',
  'apa',
  'tolong',
  'jelaskan',
  'sebutkan',
  'berikan',
  'arti',
  'kata',
  'dalam',
  'bahasa',
  'dawan',
  'uab',
  'meto',
  'contoh',
  'kalimat',
  'cara',
  'pakai',
  'dipakai',
  'gunakan',
  'digunakan',
  'materi',
  'pelajaran',
  'orang',
  'kepada',
  'indonesia',
  'serapan',
  'belum',
  'sudah',
  'bisa',
  'harus',
  'akan'
]);

const K1 = 1.2;
const B = 0.75;
export const TOP_K = 5;
export const MIN_SCORE = 1.5;

export function tokenize(value) {
  if (typeof value !== 'string') {
    return [];
  }

  const normalized = value
    .toLowerCase()
    .replaceAll('’', "'")
    .replaceAll('ʔ', "'");

  return (normalized.match(/[\p{L}\p{M}']+/gu) ?? [])
    .map((token) => token.replace(/^'+|'+$/g, ''))
    .filter((token) => token.length > 1);
}

function contentTokens(value) {
  return tokenize(value).filter(
    (token) => !STOPWORDS.has(token) && !/^\d+$/.test(token)
  );
}

/**
 * Menyiapkan indeks sekali per korpus. Hasilnya dipakai berulang untuk setiap
 * pertanyaan tanpa menyentuh jaringan.
 */
export function buildIndex(corpus) {
  const chunks = corpus?.chunks ?? [];
  const documents = chunks.map((chunk) => {
    const tokens = contentTokens(
      `${chunk.text} ${chunk.lessonTitle} ${chunk.kind}`
    );
    const termFrequencies = new Map();
    for (const token of tokens) {
      termFrequencies.set(token, (termFrequencies.get(token) ?? 0) + 1);
    }

    return { chunk, tokens, termFrequencies };
  });

  const documentFrequencies = new Map();
  for (const document of documents) {
    for (const token of document.termFrequencies.keys()) {
      documentFrequencies.set(
        token,
        (documentFrequencies.get(token) ?? 0) + 1
      );
    }
  }

  const totalLength = documents.reduce(
    (sum, document) => sum + document.tokens.length,
    0
  );

  return {
    corpus,
    documents,
    documentFrequencies,
    averageLength: documents.length === 0 ? 0 : totalLength / documents.length
  };
}

function idf(index, token) {
  const total = index.documents.length;
  const frequency = index.documentFrequencies.get(token) ?? 0;
  return Math.log(1 + (total - frequency + 0.5) / (frequency + 0.5));
}

/**
 * Mengembalikan chunk paling relevan beserta skornya. Query tanpa satu pun
 * token yang dikenal korpus menghasilkan daftar kosong, sehingga Tutor AI
 * menolak menjawab alih-alih memanggil model tanpa referensi.
 */
export function retrieve(question, index, options = {}) {
  const topK = options.topK ?? TOP_K;
  const queryTokens = [...new Set(contentTokens(question))];
  if (queryTokens.length === 0) {
    return [];
  }

  const scored = [];
  for (const document of index.documents) {
    let score = 0;
    let matched = 0;

    for (const token of queryTokens) {
      const frequency = document.termFrequencies.get(token) ?? 0;
      if (frequency === 0) {
        continue;
      }

      matched += 1;
      const lengthNorm =
        1 - B + B * (document.tokens.length / (index.averageLength || 1));
      score +=
        idf(index, token) *
        ((frequency * (K1 + 1)) /
          (frequency + K1 * lengthNorm));
    }

    if (matched > 0) {
      scored.push({ chunk: document.chunk, score, matched });
    }
  }

  scored.sort((left, right) => {
    if (right.score !== left.score) {
      return right.score - left.score;
    }

    return left.chunk.id.localeCompare(right.chunk.id);
  });

  return scored.slice(0, topK);
}

/**
 * Apakah hasil retrieval cukup kuat untuk dijadikan konteks jawaban.
 */
export function hasUsableContext(results, options = {}) {
  const minimum = options.minScore ?? MIN_SCORE;
  return results.length > 0 && results[0].score >= minimum;
}
