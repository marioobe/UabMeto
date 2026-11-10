# Kualitas Tutor AI (Teks)

Tanggal: 2026-11-09 (WITA).
Lingkungan: Windows, Node `v24.11.1`, Worker lokal `wrangler dev --local`,
Unity `6000.6.0f1`, OPPO `CPH1803` (Android 8.1/API 27).
Status: **PASS untuk cakupan teks dengan provider `mock` di mode online
lokal.** Model Workers AI sungguhan **belum diuji** karena mesin ini belum
memiliki kredensial Cloudflare. Ini bukan klaim kelulusan AI produksi.

## 1. Cakupan

Yang dikerjakan pada tahap ini:

- Tutor AI **teks** di tab Tutor AI: pertanyaan bebas, jawaban Bahasa
  Indonesia, sumber pelajaran, kuota harian, dan state galat.
- Jawaban hanya dari korpus tiga pelajaran Topik 1 yang berstatus `validated`.
- Penolakan eksplisit bila referensi tidak ditemukan.
- Guardrail untuk menolak bentuk Dawan yang tidak ada di korpus.

Yang **ditunda** (lihat PRD §3.4 dan rencana F4):

- Percakapan suara dua arah.
- Penilaian pelafalan/ASR; tidak ada skor pelafalan pada versi ini.
- Tutor adaptif yang memilih materi berdasarkan riwayat belajar.
- Audio Dawan; hanya rekaman terverifikasi yang boleh diputar, dan belum ada
  rekaman seperti itu.

## 2. Aturan jawaban

| Aturan | Implementasi |
|---|---|
| Jawab hanya dalam Bahasa Indonesia | `buildSystemPrompt()` di `backend/src/tutor-prompt.js` |
| Jawab hanya dari konteks | Retrieval + prompt; tanpa konteks layak → refusal |
| Jangan mengarang kosakata/terjemahan/dialog/pelafalan | Guardrail `findUnverifiedDawanTerms()` + refusal |
| Sebut sumber | `sources[]` berisi `lessonId`, `title`, `sourceRef` |
| Referensi tidak tersedia | Teks tetap `REFUSAL_TEXT`: "Belum dapat dipastikan…" |
| Tidak menilai pelafalan | Prompt melarang skor; tidak ada fitur pelafalan |
| Kuota 10 pertanyaan/akun/hari | Tabel `quota`, dihitung setelah jawaban/refusal |

Korpus dibuat ulang dengan `npm run corpus` dari `content/lessons/*.json`
yang `validated`/`published` saja. Hasil 2026-11-09: **3 pelajaran, 60 chunk,
50 istilah Dawan, 44 frasa**. Tidak ada teks Dawan yang ditulis manual di
skrip korpus.

### 2.1 Provider

| Provider | Kapan dipakai | Perilaku |
|---|---|---|
| `mock` (default lokal) | `TUTOR_PROVIDER=mock` atau tidak diset | Jawaban deterministik dari potongan chunk tervalidasi, tanpa model |
| `workers-ai` | `TUTOR_PROVIDER=workers-ai` + binding `AI` | Memanggil `@cf/meta/llama-3.1-8b-instruct` (dapat diganti `TUTOR_MODEL`) |

Kegagalan provider mengembalikan `503 provider_error` dan **tidak** mengurangi
kuota. Jawaban yang memuat bentuk Dawan tak dikenal ditolak dan diganti
`REFUSAL_TEXT`.

## 3. Hasil evaluasi

### 3.1 Set emas

`content/eval/tutor-golden.json` berisi 25 kasus:

- 19 kasus `answer` dari tiga pelajaran Topik 1 (7/6/6).
- 6 kasus `refuse` di luar materi (presiden, rumah adat, komputer, ibu kota,
  budaya umum, harga mobil).

Runner: `npm run eval:tutor` (`backend/scripts/run-tutor-eval.mjs`). Runner
memeriksa status HTTP, `refused`, teks penolakan persis, daftar sumber,
kecocokan `lessonId`, tidak adanya kata Dawan tak tervalidasi, dan tidak
adanya penanda placeholder.

Hasil 2026-11-09:

```text
Tutor AI eval: 25/25 lulus, 0 gagal (provider=mock).
```

### 3.2 Tes otomatis

| Lapisan | Hasil | Cakupan |
|---|---|---|
| Backend `npm test` | **25/25 lulus** | 11 tes sinkron/kuota/leaderboard + 14 tes Tutor AI |
| Unity EditMode | **93/93 lulus** | 87 lama + 6 tes kontrak `AskTutor` |
| Unity PlayMode | **39/39 lulus** | 31 lama + 8 tes `TutorView` |

Tes Tutor AI mencakup: tanpa identitas → 401; pertanyaan kosong/terlalu
panjang → 400; jawaban + sumber; refusal di luar korpus; permintaan mengarang
Dawan; kuota habis → 429 tanpa retrieval; kuota bertambah dan tercermin di
`GET /v1/quota`; kuota terpisah per pengguna; kegagalan provider tidak
mengurangi kuota; guardrail menerima kata korpus dan menolak kata asing;
riwayat dibatasi enam giliran; retrieval menolak pertanyaan tanpa kata dikenal.

### 3.3 Bukti perangkat

APK rilis `0.3.0` (`versionCode 4`, SHA-256
`BA99B129FF5CEDD5830DC443F162B2F6639540054C76DF9A57281B95C00FE713`)
dipasang dengan `adb install -r` di OPPO `CPH1803`, koneksi
`adb reverse tcp:8787 tcp:8787`, Worker lokal dengan `TUTOR_PROVIDER=mock`.
Kuota D1 lokal direset sebelum rangkaian bukti (khusus pengujian lokal).

| Uji | Hasil |
|---|---|
| Tab Tutor AI | Judul "Tutor AI", penjelasan batas, kuota "Sisa 10 dari 10", 4 chip pertanyaan |
| Keyboard | Ketuk kolom tulis → panel tulis dan tombol Kirim naik ke atas keyboard; teks terlihat penuh |
| Kirim pertanyaan | Satu ketukan Kirim saat keyboard terbuka mengirim, menutup keyboard, lalu jawaban "Noka berarti pagi…" muncul dengan sumber "Berdasarkan: Sapaan waktu" |
| Refusal | "Siapa presiden Indonesia" → "Belum dapat dipastikan…", tanpa sumber, status "Jawaban tidak dikarang." |
| Kuota | Setelah dua pertanyaan, "Sisa 8 dari 10"; `GET /v1/quota` juga mencatat `used: 2` untuk identitas perangkat |
| Server tidak terjangkau | `adb reverse` dilepas → "Tidak dapat menghubungi server lokal…" berwarna `danger-text`, tanpa jawaban palsu |
| Pemulihan | `adb reverse` dipasang lagi → jawaban "faen = pulang" dari "Bertemu dan berpisah" |
| Kuota habis | Setelah 10 pertanyaan, "Sisa 0 dari 10" dan tombol Kirim nonaktif; status menjelaskan kuota habis |

Bukti: `docs/testing/evidence/f1-device/f1-release-0.3.0-tutor-*.png`
(8 berkas: tab, keyboard, jawaban, refusal, kuota, offline, pemulihan,
kuota habis).

## 4. Batasan yang diakui

1. **Model nyata belum diuji.** `npx wrangler whoami` → not authenticated.
   Semua hasil di atas memakai provider `mock`. Kualitas bahasa model
   Workers AI (halusinasi, panjang jawaban, sitasi) belum diukur.
2. **Mock bukan model.** Jawaban mock adalah gabungan potongan chunk
   tervalidasi. Kelulusan eval mock membuktikan retrieval, refusal, sumber,
   kuota, dan guardrail, bukan kemampuan berbahasa LLM.
3. **Guardrail heuristik.** Pemeriksaan kata Dawan memakai penanda ortografi
   (apostrof/glotal/macron) dan kutipan yang diperkenalkan sebagai kata
   Dawan. Kata Indonesia biasa tidak diperiksa; ini bukan jaminan penuh.
4. **Korpus kecil.** Hanya 3 pelajaran/60 chunk. Pertanyaan di luar Topik 1
   ditolak dengan benar, tetapi cakupan jawaban memang sempit.
5. **Tanpa audio/pelafalan.** Tidak ada ASR, skor pelafalan, atau audio
   Dawan. Prompt melarang penilaian pelafalan.
6. **Kuota tidak transaksional ketat.** Pemeriksaan kuota dan penambahan
   counter terjadi dalam dua query terpisah. Dua permintaan paralel dari akun
   yang sama dapat melewati batas satu pertanyaan. Cukup untuk mode lokal,
   belum untuk produksi.
7. **Mode lokal, bukan produksi.** `X-Dev-User` bukan autentikasi; tidak ada
   rate limit, audit, atau deployment.
8. **Tanpa memori panjang.** Riwayat hanya enam pesan terakhir dan tidak
   dipakai untuk menilai kebenaran; sumber tetap dari retrieval pertanyaan
   saat ini.

## 5. Cara mereproduksi

```powershell
cd backend
npm install
npm run seed
npm run db:init
npm run corpus
npm test
npm run eval:tutor
npm run dev            # TUTOR_PROVIDER=mock dari wrangler.toml

# perangkat
adb reverse tcp:8787 tcp:8787
adb install -r Builds/Android/uabmeto-f1.apk
```

Untuk menguji model nyata, isi `.dev.vars`:

```text
TUTOR_PROVIDER="workers-ai"
TUTOR_MODEL="@cf/meta/llama-3.1-8b-instruct"
```

lalu jalankan `npx wrangler login` (atau set `CLOUDFLARE_API_TOKEN`) dan
ulangi `npm run eval:tutor` serta uji perangkat. Hasilnya **belum** dicatat
di dokumen ini karena belum dijalankan.

## 6. Keputusan

- Fitur Tutor AI teks dinyatakan **siap untuk demo/UAS lokal** dengan batasan
  di atas.
- Gate AI produksi tetap **BELUM LULUS** sampai model nyata diuji, audio dan
  consent tersedia, serta evaluasi kualitas independen dilakukan.
- Gate F1 tetap **TIDAK LULUS**; lihat `docs/testing/f1-gate-result.md`.
