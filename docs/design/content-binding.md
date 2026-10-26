# Pemetaan UI ke Sumber Data

Status: **F0.5 — kontrak untuk F1.** Menegakkan ADR-0001: konten JSON adalah
sumber kebenaran; tidak ada teks pelajaran di dalam C# atau prefab.

## 1. Empat kategori teks

| Kategori | Contoh | Sumber | Boleh hardcode? |
|---|---|---|---|
| Chrome | "Masuk", "Lanjutkan", "Coba lagi" | `ui-strings.id.json` | tidak, pakai kunci |
| Konten | kata Dawan, arti, dialog, soal, penjelasan | `content/lessons/*.json` | tidak |
| Konfigurasi | XP, durasi, kuota, harga hati, ambang ujian | `config/app-config.json` + Worker | tidak |
| Runtime | XP pengguna, streak, hati, sisa kuota, peringkat | Firestore / D1 / outbox | tidak |

Aturan: tidak ada string berbahasa Dawan di kode C#, prefab, atau scene.
Satu-satunya pengecualian adalah `content/samples/` yang berstatus
`UNVALIDATED` dan tidak pernah dimuat build rilis.

## 2. Peta elemen mockup ke sumber

| Elemen UI | Sumber | Catatan |
|---|---|---|
| Judul topik | `path.json` → `topics[].title` | 12 topik PRD |
| Deskripsi topik | `path.json` → `topics[].summary` | opsional |
| Ikon topik | `path.json` → `topics[].icon` | nama ikon, bukan path |
| Status topik | dihitung dari progres lokal | terkunci/aktif/selesai/belum tersedia |
| Judul pelajaran | `lesson.title` | — |
| Jumlah langkah | `2 + exercises.length` | lihat §4 |
| XP pelajaran | `lesson.xp` | field baru, lihat §3 |
| Estimasi waktu | `lesson.estimatedMinutes` | field baru |
| Daftar kosakata | `lesson.vocabulary[]` | `dawan`, `indonesian`, `notes` |
| Audio kosakata | `lesson.vocabulary[].audio` | `null` → state tidak tersedia |
| Dialog | `lesson.dialogues[]` | per baris ada audio opsional |
| Soal | `lesson.exercises[]` | `prompt`, `content`, `explanation` |
| Umpan balik benar/salah | `exercise.explanation` | wajib untuk pilihan ganda |
| Skor pelafalan | Worker | bukan dari klien |
| Sisa kuota | Worker | angka, bukan hardcode |
| XP/streak/hati pengguna | Firestore | bukan hardcode |
| Peringkat | D1 projection | nickname + XP saja |
| Nama lencana | `config/app-config.json` | nama netral |
| Nama level | `config/app-config.json` | nama netral |
| Versi aplikasi | `Application.version` | build metadata |
| Ukuran cache | hitung dari `Application.persistentDataPath` | — |
| Nama penutur | `audio.consentRef` + metadata | tampil hanya bila diizinkan |

## 3. Perubahan skema yang diusulkan (F1)

Aditif, tidak memutus klien. `schemaVersion` naik ke `1.1.0`.

### 3.1 Field pelajaran baru

| Field | Tipe | Wajib | Aturan |
|---|---|---|---|
| `xp` | integer | ya | 1–100; diberikan sekali saat pelajaran selesai |
| `estimatedMinutes` | integer | ya | 1–30; hanya tampilan |

### 3.2 `content/schema/path.schema.json` (baru)

```json
{
  "schemaVersion": "1.1.0",
  "topics": [
    {
      "id": "topic-01-salam",
      "order": 1,
      "title": "Salam dan sapaan",
      "summary": "…",
      "icon": "waving_hand",
      "lessons": [
        { "lessonId": "lesson-001", "order": 1 }
      ]
    }
  ]
}
```

Aturan:

- `topics` wajib berurutan 1–12 dan sama dengan PRD §3.2.
- `title` Bahasa Indonesia.
- `lessons[].lessonId` harus ada di `content/lessons/`.
- Topik tanpa pelajaran terbit tetap tampil sebagai "belum tersedia".
- `icon` harus ada di `docs/design/icon-inventory.md`.

### 3.3 Konfigurasi aplikasi

`config/app-config.json`:

| Kunci | Nilai awal | Sumber akhir |
|---|---|---|
| `hearts.max` | 5 | config |
| `hearts.regenerationMinutes` | menunggu prototipe | config |
| `hearts.costExerciseTypes` | `multiple_choice`, `sentence_order` | config |
| `quota.tutor` | 10 | Worker (otoritatif) |
| `quota.conversation` | 3 | Worker |
| `quota.pronunciation` | 10 | Worker |
| `quota.resetTimezone` | `Asia/Makassar` | Worker |
| `leaderboard.resetDay` | `monday` | Worker |
| `exam.passTotal` | 70 | config |
| `exam.passSkill` | 50 | config |
| `level.thresholds` | menunggu desain | config |

Klien hanya menampilkan; penegakan tetap di server.

## 4. Turunan jumlah langkah

Jumlah langkah tidak disimpan, tetapi dihitung:

```
langkah = 1 (materi) + exercises.length + 1 (ringkasan)
```

- Langkah 1 = seluruh materi (kosakata, dialog, catatan) dalam satu alur
  gulir.
- Langkah 2..n = satu soal per layar.
- Langkah terakhir = ringkasan.

Dengan begitu "Langkah 3 dari 5" selalu konsisten dengan isi JSON dan tidak
bisa berbeda antar layar.

## 5. Aturan placeholder

1. `[PLACEHOLDER-DWN-xxx]` hanya boleh muncul di `content/samples/`.
2. Build rilis menolak memuat konten yang mengandung placeholder.
3. Bila tidak ada konten `validated`, UI menampilkan state kosong:
   "Materi belum tersedia", tanpa teks contoh.
4. Dokumen desain tidak boleh memuat kata Dawan sebagai contoh; gunakan
   placeholder.

## 6. Teks chrome

`app/Assets/_Project/Resources/ui-strings.id.json`:

```json
{
  "auth.login.title": "Masuk",
  "auth.login.submit": "Masuk sekarang",
  "common.retry": "Coba lagi",
  "state.offline.title": "Mode offline",
  "state.not_assessable": "Belum dapat dinilai"
}
```

Aturan:

- Semua teks chrome lewat kunci. Tidak ada literal di prefab.
- Bahasa Indonesia saja untuk versi pertama.
- Kunci tidak boleh berisi teks Dawan.
- Perubahan teks tidak boleh mengubah layout secara tak terduga; uji
  panjang maksimum.

## 7. Penegakan

| Aturan | Cara |
|---|---|
| Tidak ada warna mentah | validator prefab |
| Tidak ada teks konten di C# | review + pencarian string non-ASCII |
| Tidak ada placeholder di rilis | `PublishGate` + build check |
| XP dan durasi dari JSON | unit test konten |
| Kuota dari Worker | test kontrak |
| 12 topik sesuai PRD | test `path.json` |

## 8. Larangan eksplisit

- Jangan menyalin teks dari mockup Stitch ke C#.
- Jangan menyimpan XP, durasi, kuota, atau ambang ujian di prefab.
- Jangan menampilkan nama penutur tanpa `consentRef`.
- Jangan menampilkan skor pelafalan bila hasil analisis tidak meyakinkan.
- Jangan menampilkan "tersinkron" tanpa bukti sinkron.
