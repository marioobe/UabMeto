# Lembar Elisitasi Materi

Status: **alat kerja, bukan materi terbit.**
Tujuan: mengumpulkan bentuk Bahasa Dawan varian TTU/Kefamenanu dari penutur
asli tanpa mengarang kosakata, terjemahan, atau dialog.

## 1. Mengapa lembar ini ada

Repo ini tidak boleh mengisi teks Dawan dari tebakan, terjemahan mesin, atau
karangan. Lembar ini adalah jalur resmi untuk mendapatkan teks dari penutur
manusia. Hasilnya tidak otomatis menjadi `validated`; statusnya `draft` atau
`in_review` sampai reviewer menandatangani.

Lembar ini **tidak** menggantikan sumber tertulis. Sumber tetap dicatat di
`content/references/` dan dirujuk oleh setiap item yang masuk ke JSON. Untuk
hasil elisitasi, sumber dicatat sebagai wawancara penutur dengan ID
`SRC-ELIC-...` (contoh: `SRC-ELIC-01`).

## 2. Berkas

| Berkas | Untuk siapa | Isi |
|---|---|---|
| `topic-01-salam/lesson-plan.md` | penyusun/reviewer | rencana 3 pelajaran |
| `topic-01-salam/speaker-sheet.md` | penutur | versi cetak |
| `topic-01-salam/speaker-sheet.csv` | penyusun | versi mesin |
| `topic-01-salam/reviewer-sheet.md` | reviewer | checklist validasi |
| `_template/speaker-sheet.csv` | penyusun | pola untuk topik lain |

## 3. Pembagian tugas

| Kolom | Diisi oleh |
|---|---|
| `item_id`, `lesson_id`, `topic_id`, `item_type`, `item_ref`, `indonesian_prompt`, `context_register` | penyusun (sudah terisi) |
| `dawan_response`, `dawan_variant`, `usage_note`, `pronunciation_note`, `speaker_status`, `speaker_ref`, `audio_recorded`, `consent_ref` | penutur |
| `review_status`, `reviewer_ref`, `reviewed_at`, `notes` | reviewer |

## 3.1 Pemetaan ke skema

| `item_type` | Field JSON |
|---|---|
| `vocabulary` | `vocabulary[]` |
| `dialogue_line` | `dialogues[].lines[]` |
| `example` | `examples[]` |

Isi latihan (opsi pilihan ganda, pasangan, token) diturunkan dari item di atas;
tidak ada teks Dawan baru yang ditambahkan untuk latihan.

## 4. Cara mengisi

1. Baca prompt Bahasa Indonesia dan konteksnya.
2. Tulis bentuk Dawan yang **paling wajar** untuk konteks itu, bukan
   terjemahan kata per kata.
3. Kalau tidak ada padanan langsung, tulis `TIDAK_ADA` di `speaker_status`
   dan jelaskan alternatifnya di `usage_note`.
4. Kalau ragu, tulis `RAGU`; jangan menebak.
5. Kalau ada beberapa bentuk, tulis bentuk utama di `dawan_response` dan
   variasi di `dawan_variant`.
6. Tulis catatan pemakaian: kepada siapa, formal/akrab, daerah pemakaian,
   apakah kata serapan.
7. Ejaan ditulis apa adanya. Ortografi `ā`, `’`, `ʔ` dirapikan terpisah oleh
   penyusun, tidak dibebankan ke penutur.
8. Jangan mengisi kolom reviewer.

## 5. Kode status penutur

| Kode | Arti |
|---|---|
| `ADA` | ada bentuk yang wajar |
| `TIDAK_ADA` | tidak ada padanan langsung; jelaskan alternatifnya |
| `RAGU` | penutur tidak yakin; perlu penutur atau reviewer lain |

## 6. Kode status review

| Kode | Arti |
|---|---|
| `PENDING` | belum diperiksa |
| `APPROVED` | disetujui |
| `REVISION` | perlu perbaikan |
| `REJECTED` | ditolak |

## 7. Privasi dan izin

- Nama asli penutur opsional; gunakan `SPK-01` bila tidak ingin nama tampil.
- Jangan menulis nomor telepon, alamat, atau data pribadi lain di repo.
- Rekaman audio hanya dipakai bila penutur menandatangani izin
  (`consent_ref`).
- Identitas asli disimpan di luar repo; repo hanya menyimpan kode.

## 8. Setelah lembar terisi

1. Penyusun memasukkan hasil ke JSON pelajaran.
2. Validator dijalankan.
3. Reviewer mengisi `reviewer-sheet.md`.
4. Status naik ke `validated` hanya setelah sign-off.
5. Audio direkam terpisah setelah teks disetujui.

## 9. Yang tidak boleh

- Mengisi `dawan_response` dengan tebakan, terjemahan mesin, atau karangan.
- Menyalin dari sumber tanpa mencatat sumbernya.
- Menaikkan status ke `validated` tanpa reviewer manusia.
- Menganggap lembar ini sebagai materi siap terbit.
