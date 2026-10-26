# Daftar Remediasi Desain

Status: **semua butir OPEN.** Tidak ada yang boleh ditutup tanpa bukti
verifikasi. Sumber temuan: `design-review-stitch.md`.

## 1. Cara pakai

| Kolom | Arti |
|---|---|
| ID | pengenal unik, tidak berubah |
| Prioritas | `P0` menghalangi F1; `P1` menghalangi gate F1 atau rilis |
| Target | tahap penutupan |
| Status | `OPEN`, `IN PROGRESS`, `CLOSED`, `DEFERRED` |
| Verifikasi | bukti yang harus ada saat menutup |

Aturan:

1. Satu ID = satu masalah yang bisa diuji.
2. Menutup ID wajib menyertakan bukti (tes, tangkapan layar, atau commit).
3. Menunda ID wajib menyebut alasan dan pemilik keputusan.
4. Jangan menutup ID karena "sudah diperbaiki di mockup"; mockup bukan
   produk.

## 2. Ringkasan

| ID | Prioritas | Target | Status |
|---|---|---|---|
| REM-P0-01 | P0 | F1 | OPEN |
| REM-P0-02 | P0 | F1 | OPEN |
| REM-P0-03 | P0 | F1 (pipeline), konten menyusul | OPEN |
| REM-P0-04 | P0 | F1 | OPEN |
| REM-P0-05 | P0 | F1 | OPEN |
| REM-P0-06 | P0 | F1 | OPEN |
| REM-P0-07 | P0 | F1 | OPEN |
| REM-P0-08 | P0 | F1 | OPEN |
| REM-P1-01 | P1 | F1 | OPEN |
| REM-P1-02 | P1 | F1 | OPEN |
| REM-P1-03 | P1 | F1 | OPEN |
| REM-P1-04 | P1 | F1 | OPEN |
| REM-P1-05 | P1 | F1 | OPEN |
| REM-P1-06 | P1 | F1 | OPEN |
| REM-P1-07 | P1 | F1 | OPEN |
| REM-P1-08 | P1 | F1 | OPEN |
| REM-P1-09 | P1 | F1 | OPEN |
| REM-P1-10 | P1 | F2 | OPEN |

## 3. P0 — menghalangi F1

### REM-P0-01 — Daftar topik tidak sesuai PRD §3.2

- **Lokasi:** Beranda (jalur topik), Ringkasan, layar ujian.
- **Masalah:** mockup memuat topik di luar PRD, menghilangkan dua topik
  wajib, dan menjadikan ujian sebagai topik.
- **Kriteria penerimaan:**
  1. `content/path/topics.json` berisi tepat 12 topik PRD, berurutan.
  2. Tidak ada topik "Alam, Ladang & Hewan" atau "Cerita Tradisi".
  3. Ujian akhir tidak memiliki `topicId`.
  4. Beranda merender 12 kartu, termasuk topik tanpa pelajaran terbit
     dengan status "belum tersedia".
  5. Tes membandingkan isi registry dengan daftar konstan PRD.
- **Verifikasi:** unit test `TopicPathTests`; review tangkapan layar
  Beranda; `grep` nama topik terlarang.
- **Bukti pendukung:** `topic-scope.md`, `content/path/topics.json`.

### REM-P0-02 — Cakupan dialek melenceng dari TTU

- **Lokasi:** Masuk, Beranda, Kartu kosakata, Tutor AI, Peringkat, Profil.
- **Masalah:** label Amarasi, Amanuban, Soe, Kupang, Sonbai, Mutis, dan
  Miomafo muncul sebagai bagian produk.
- **Kriteria penerimaan:**
  1. Satu-satunya nilai varian yang boleh muncul: `TTU-Kefamenanu`.
  2. Tidak ada string nama dialek lain di `ui-strings`, `topics.json`,
     prefab, atau konten.
  3. Nama dialek lain hanya boleh ada di dokumen editorial, bukan UI.
- **Verifikasi:** tes `grep` pada aset UI dan konten; review konten.

### REM-P0-03 — Teks Dawan tanpa sumber

- **Lokasi:** 8 layar, lihat `content-inventory.md` DWN-01..DWN-08.
- **Masalah:** kata, frasa, transkripsi, dan peribahasa Dawan tampil tanpa
  `sources` dan `reviewedBy`.
- **Kriteria penerimaan:**
  1. Tidak ada teks Dawan di luar `content/lessons/*.json` berstatus
     `validated`/`published`.
  2. Semua item `validated` punya `validation.sources` tidak kosong,
     `reviewedBy`, dan `reviewedAt`.
  3. Build rilis menolak konten yang memuat `[PLACEHOLDER`.
  4. Delapan lokasi diganti placeholder di dokumen dan referensi.
- **Verifikasi:** `npm test`; CLI validator; `PublishGate`; review konten.
- **Catatan:** pipeline bisa ditutup di F1; isi konten tidak bisa ditutup
  sebelum ada reviewer manusia.

### REM-P0-04 — Klaim dan angka karangan

- **Lokasi:** Tutor AI, Pelafalan, Profil, Peringkat, Beranda.
- **Masalah:** 10 klaim/angka tanpa sumber, lihat `content-inventory.md`
  CLM-01..CLM-10.
- **Kriteria penerimaan:**
  1. Kesepuluh klaim dihapus atau diganti nilai runtime.
  2. Tidak ada versi model AI, latensi, atau status verifikasi di UI.
  3. XP, ukuran cache, versi aplikasi, dan kuota berasal dari sumber nyata.
  4. Tes memastikan tidak ada literal angka kunci di prefab.
- **Verifikasi:** `grep`; unit test pemetaan data; review screenshot.

### REM-P0-05 — Konfigurasi saling bertentangan

- **Lokasi:** Beranda, Pembuka, Latihan, Ringkasan, Tutor AI.
- **Masalah:** kuota 15 vs 10; XP +15 vs +25; "3 kata" vs 4 kata; progres
  40% vs "1 dari 3"; hati 5/5 vs "sisa 4".
- **Kriteria penerimaan:**
  1. XP dan durasi berasal dari `lesson.xp` dan `lesson.estimatedMinutes`.
  2. Jumlah langkah dihitung `2 + exercises.length`.
  3. Kuota berasal dari Worker; klien tidak menyimpan angka.
  4. Hati memakai satu state; header dan panel membaca nilai yang sama.
  5. Label progres menyebut tingkat secara eksplisit.
- **Verifikasi:** unit test `ProgressModelTests`; tes kontrak kuota;
  review screenshot.

### REM-P0-06 — Atribusi orang nyata tanpa bukti

- **Lokasi:** Kartu kosakata, audio pelajaran.
- **Masalah:** nama dan klaim "penutur asli" ditampilkan tanpa izin.
- **Kriteria penerimaan:**
  1. Tidak ada nama orang nyata di aset atau konten tanpa `consentRef`.
  2. Validator menolak audio tanpa `consentRef` pada konten `validated`.
  3. UI hanya menampilkan penutur bila metadata mengizinkan.
- **Verifikasi:** aturan validator; review konten; `grep` nama.

### REM-P0-07 — Panel pratinjau desainer bocor

- **Lokasi:** Tutor AI, Latihan, Beranda (varian).
- **Masalah:** "Simulasi Jaringan", "Pratinjau Status Sistem", pengalih
  state, dan toggle audio muncul di layar produk.
- **Kriteria penerimaan:**
  1. Semua panel debug berada di `#if DEVELOPMENT_BUILD`.
  2. Build rilis tidak memuat string panel debug.
  3. Tangkapan layar rilis tidak menampilkan panel debug.
- **Verifikasi:** pemeriksaan build; `grep` pada APK/assembly; screenshot.

### REM-P0-08 — `DESIGN.md` bertentangan dengan mockup

- **Lokasi:** design system.
- **Masalah:** klaim tanpa bayangan/blur, tetapi mockup memakai keduanya;
  lima nilai warna berbeda.
- **Kriteria penerimaan:**
  1. `design-tokens.md` menjadi satu-satunya sumber token.
  2. Tidak ada bayangan, blur, atau gradien di prefab.
  3. Warna prefab hanya berasal dari `ThemeTokens`.
  4. Validator prefab menolak warna mentah.
- **Verifikasi:** validator prefab; review kode; screenshot.

## 4. P1 — menghalangi gate F1 atau rilis

### REM-P1-01 — Landscape belum didesain

- **Lokasi:** Beranda, Pelajaran, Latihan, Ujian, Tutor AI, Peringkat,
  Profil.
- **Kriteria penerimaan:** tujuh layar memakai layout dua kolom sesuai
  `screens.md` §12; tidak ada teks terpotong pada 915 × 412; bottom bar
  tetap 4 tab.
- **Verifikasi:** tangkapan layar landscape per layar; inspeksi layout.

### REM-P1-02 — Layar wajib belum ada

- **Lokasi:** splash, verifikasi email, lupa sandi, persetujuan suara,
  dialog, penjelasan, ujian akhir (7 layar).
- **Kriteria penerimaan:** semua ID di `screens.md` punya prefab dan state
  wajib; tes inventaris gagal bila ada ID hilang.
- **Verifikasi:** tes inventaris layar; review prefab.

### REM-P1-03 — Hanya satu jenis latihan

- **Lokasi:** Latihan.
- **Kriteria penerimaan:** enam prefab latihan (`multiple_choice`, `match`,
  `sentence_order`, `listening`, `writing`, `speaking`) punya state dan
  tes masing-masing.
- **Verifikasi:** PlayMode per jenis; screenshot.

### REM-P1-04 — Nickname tidak diminta saat daftar

- **Lokasi:** Daftar, Beranda, Peringkat.
- **Kriteria penerimaan:** field wajib 3–20 karakter; validasi tampil;
  tersimpan di profil; tampil di Beranda dan Peringkat; tidak boleh sama
  dengan email.
- **Verifikasi:** unit test validasi; PlayMode; screenshot.

### REM-P1-05 — Hapus akun hanya satu konfirmasi

- **Lokasi:** Profil.
- **Kriteria penerimaan:** dua langkah konfirmasi, autentikasi ulang,
  pembersihan sesi dan cache, pesan kegagalan.
- **Verifikasi:** PlayMode dengan kredensial salah dan benar.

### REM-P1-06 — Login offline belum punya jalur

- **Lokasi:** Splash, Masuk, Beranda.
- **Kriteria penerimaan:** sesi valid + offline → Beranda dengan banner;
  tanpa sesi + offline → state terkunci; tidak ada percobaan login palsu.
- **Verifikasi:** PlayMode dengan mock jaringan; screenshot.

### REM-P1-07 — State AI belum ada

- **Lokasi:** Tutor AI, Percakapan, Pelafalan.
- **Kriteria penerimaan:** lima state (kuota habis, layanan tidak tersedia,
  referensi tidak ditemukan, izin mikrofon ditolak, sesi kedaluwarsa) ada
  di ketiga layar.
- **Verifikasi:** PlayMode per state; screenshot.

### REM-P1-08 — Aksesibilitas: kontras dan target sentuh

- **Lokasi:** lintas layar.
- **Masalah:** 5 pasangan kontras gagal dan 12 target sentuh < 48 dp.
- **Kriteria penerimaan:** semua sub-butir C1–C5 dan T1–T12 `CLOSED`;
  skrip kontras lulus; audit target sentuh lulus.
- **Verifikasi:** skrip kontras dari token; pengukuran rect di PlayMode;
  review manual. **Belum boleh diklaim lulus sebelum itu.**
- **Catatan:** pembaca layar (TalkBack) belum termasuk kriteria ini; lihat
  risiko di `accessibility.md` §8.

#### 4.1 Kontras

| Sub-ID | Lokasi | Masalah | Rasio | Kriteria |
|---|---|---|---|---|
| REM-P1-08.C1 | Latihan, Ringkasan | `danger-fill` sebagai teks di `surface-card` | 3.61 | ganti `danger-text`; ≥ 4.5 |
| REM-P1-08.C2 | Beranda (hati) | `danger-fill` sebagai teks di `canvas` | 4.20 | ganti `danger-text`; ≥ 4.5 |
| REM-P1-08.C3 | Latihan (label 12 sp) | `danger-fill` di `danger-panel` | 3.92 | ganti `danger-text`; ≥ 4.5 |
| REM-P1-08.C4 | Beranda, Pembuka | `#FFEFD7` di atas `accent` | 1.31 | ganti `on-accent`; ≥ 4.5 |
| REM-P1-08.C5 | chip & label | `outline` di `surface-high` < 14 sp | 4.53 | naikkan ke `text-muted`; ≥ 4.5 |

#### 4.2 Target sentuh

| Sub-ID | Lokasi | Ukuran | Kriteria |
|---|---|---|---|
| REM-P1-08.T1 | Masuk | tombol tampilkan sandi 40 | ≥ 48 |
| REM-P1-08.T2 | modal | tombol tutup 32 | ≥ 48 |
| REM-P1-08.T3 | header | avatar 32 | ≥ 48 atau non-interaktif |
| REM-P1-08.T4 | Profil | "Bersihkan" 40 | ≥ 48 |
| REM-P1-08.T5 | Profil | "Ubah Waktu" 44 | ≥ 48 |
| REM-P1-08.T6 | Tutor AI | chip pertanyaan 38 | ≥ 48 |
| REM-P1-08.T7 | Kartu kosakata | kecepatan audio 26 | ≥ 48 |
| REM-P1-08.T8 | Kartu kosakata | segmented audio 24–28 | ≥ 48 |
| REM-P1-08.T9 | Peringkat | tab 40 | ≥ 48 |
| REM-P1-08.T10 | persetujuan suara | "Nanti Saja" 40 | ≥ 48 |
| REM-P1-08.T11 | Kartu kosakata | "Simpan Kata" 40 | ≥ 48 atau dihapus (F2) |
| REM-P1-08.T12 | debug | chip state 30 | ≥ 48 atau hapus dari rilis |

### REM-P1-09 — Navigasi dan header tidak konsisten

- **Lokasi:** bottom bar semua tab; header Peringkat dan Profil.
- **Kriteria penerimaan:** bottom bar identik (ikon, tinggi, label) di
  semua tab; header menyebut nama layar yang benar; tes snapshot lulus.
- **Verifikasi:** tangkapan layar per tab; tes snapshot.

### REM-P1-10 — Kamus Saku di luar lingkup

- **Lokasi:** Kartu kosakata ("Simpan Kata").
- **Keputusan:** dihapus dari F1; dicatat sebagai ide F2.
- **Kriteria penerimaan:** tidak ada tombol/route "Simpan Kata" di F1;
  tercatat di backlog F2.
- **Verifikasi:** `grep`; review prefab.

## 5. Urutan penutupan yang disarankan

1. REM-P0-08 dan REM-P0-07 — fondasi token dan kebersihan build.
2. REM-P0-01, REM-P0-02, REM-P0-05 — model data dan cakupan.
3. REM-P0-03, REM-P0-04, REM-P0-06 — konten dan klaim.
4. REM-P1-08 — aksesibilitas.
5. REM-P1-02, REM-P1-03, REM-P1-04 — kelengkapan layar.
6. REM-P1-01, REM-P1-05, REM-P1-06, REM-P1-07, REM-P1-09.
7. REM-P1-10 — dicatat untuk F2.

## 6. Yang tidak termasuk daftar ini

- Perbaikan konten Dawan nyata (menunggu reviewer).
- Rekaman audio (menunggu penutur dan izin).
- Integrasi Firebase/Cloudflare/Supabase (menunggu akun).
- Uji perangkat fisik (menunggu perangkat).
