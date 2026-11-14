# Bukti Uji Perangkat F1

Tanggal: 2026-11-03 (WITA); diperbarui 2026-11-12 (WITA)
Status: **PASS dengan catatan** — pada OPPO `CPH1803`, D1-01, D1-02, D1-03,
D1-04, D1-06 (sampai batas slider perangkat 1.6×), dan D1-07 lulus. D1-05
**WAIVED** atas keputusan pemilik 2026-11-12: F1 portrait-only, rotasi
landscape tidak diklaim. Skala 2.0× hanya terbukti di PlayMode karena slider
font perangkat ini berhenti di 1.6×.

## 1. Perangkat dan APK

Tabel ini menunjukkan APK yang terpasang saat pembaruan terakhir. Riwayat
perangkat Xiaomi `24117RN76O` dan APK 0.1.0/0.2.0 tetap dicatat di bagian
2–3.

| Item | Nilai |
|---|---|
| Perangkat | OPPO `CPH1803` (`26b4c7b0`) |
| Android | 8.1 (API 27) |
| Layar | 720×1520, 320 dpi |
| ADB | `device` (terotorisasi) |
| Paket | `id.uabmeto.app` |
| Versi | `0.5.0` (`versionCode 6`) |
| minSdk / targetSdk | 26 / 36 |
| ABI | `arm64-v8a` |
| APK | `Builds/Android/uabmeto-f1.apk` |
| Ukuran | 28.994.614 byte |
| SHA-256 | `4901E8D345ED65B5B395A742A37AFCA73794C5CFF05B10D650C9100F16A7A166` |
| firstInstallTime | 2026-11-12 16:21:09 (aplikasi di-uninstall lalu dipasang ulang untuk uji D1-04) |
| lastUpdateTime | 2026-11-12 16:21:09 |
| Tanda tangan | Android Debug; sertifikat SHA-256 `d1b79798219af505ae030cad661f7b905beb214c71b66ba73d0c9e32b81d90cf` |

`adb install -r` berhasil. `dumpsys package` mengonfirmasi versi, minSdk,
targetSdk, ABI, tanda tangan, dan waktu instalasi di atas. Perangkat Xiaomi
`24117RN76O` (Android 16, 1080×2400) dipakai pada verifikasi 0.1.0 dan
dicatat di bagian 2–3.

## 2. Hasil per butir gate perangkat

| ID | Status | Bukti |
|---|---|---|
| D1-01 APK terpasang | **PASS** | `adb install` → `Success`; `dumpsys package` (0.5.0, `versionCode 6`) |
| D1-02 Dibuka tanpa crash | **PASS** | `UnityPlayerGameActivity` foreground; logcat tanpa fatal/exception aplikasi |
| D1-03 Alur pelajaran | **PASS (0.2.1, diulang 0.5.0)** | Tiga pelajaran rilis Topik 1 dibuka berurutan lewat daftar; replay tanpa XP ganda; alur 5 langkah diulang pada APK 0.5.0 |
| D1-04 Mode offline | **PASS (0.5.0)** | Wi-Fi + data seluler dimatikan, `ping 8.8.8.8` gagal; pelajaran diselesaikan offline, banner "Offline — 1 hasil menunggu sinkron", XP tertunda 10, Selesai 1 |
| D1-05 Rotasi portrait ↔ landscape | **WAIVED (keputusan pemilik, 2026-11-12)** | F1 dikunci portrait-only; manifest `screenOrientation=1`; rotasi paksa lewat `settings put` diblokir ColorOS (`WRITE_SETTINGS`). Tidak ada klaim landscape |
| D1-06 Font sistem 1.3×/2.0× | **PASS sampai 1.6× (0.5.0)** | Perangkat: 1.0×, 1.35×, dan 1.6×; logcat `[UabMeto] fontScale sistem … diterapkan sebagai …`. 2.0× hanya di PlayMode karena slider perangkat maksimum 1.6× |
| D1-07 Tombol back | **PASS** | Profil → Beranda; dialog nama → Profil; daftar pelajaran → Beranda |

## 3. UI yang diperiksa

Semua tangkapan layar berasal dari perangkat, bukan mockup:

| Berkas | Isi |
|---|---|
| `docs/testing/evidence/f1-device/f1-final-coldstart.png` | Beranda setelah cold start, empty state |
| `docs/testing/evidence/f1-device/f1-fixed-beranda-scroll.png` | Daftar topik setelah scroll |
| `docs/testing/evidence/f1-device/f1-fixed-beranda-bottom.png` | Bagian bawah daftar topik |
| `docs/testing/evidence/f1-device/f1-final-profile2.png` | Profil, kartu identitas |
| `docs/testing/evidence/f1-device/f1-fixed-profil-scroll.png` | Profil setelah scroll ke Tentang |
| `docs/testing/evidence/f1-device/f1-final-dialog2.png` | Dialog nama panggilan |
| `docs/testing/evidence/f1-device/f1-final-topic-modal4.png` | Modal topik belum tersedia |
| `docs/testing/evidence/f1-device/f1-final-tutor.png` | Tab Tutor AI |
| `docs/testing/evidence/f1-device/f1-final-peringkat.png` | Tab Peringkat |
| `docs/testing/evidence/f1-device/f1-back-home.png` | Back dari Profil ke Beranda |
| `docs/testing/evidence/f1-device/f1-back-dialog.png` | Back menutup dialog |

Catatan: salinan ini di-commit sebagai bukti; berkas asli dari perangkat ada
di `Builds/device/` yang diabaikan Git. Tangkapan layar bukan aset rilis.

### 3.1 Verifikasi layout pratinjau di OPPO (demo)

Tanggal: 2026-11-06. Perangkat: OPPO `CPH1803`, Android 8.1 (API 27),
`arm64-v8a`, layar 720×1520, densitas 320 dpi. APK: `uabmeto-f1-preview.apk`
(SHA-256 `491B5FB619F82521024B794B4A452BB2D6CF440B0855C8ECD0C5E7996E1971FE`).

Verifikasi ini memakai materi `in_review` di mode pratinjau berlabel. Hasilnya
menunjukkan tampilan demo, **bukan** bukti gate alur pelajaran rilis.

| Berkas | Isi |
|---|---|
| `f1-layout-home-oppo.png` | Beranda: banner, statistik, kartu pratinjau |
| `f1-layout-preview-modal-oppo.png` | Modal konfirmasi pratinjau |
| `f1-layout-lesson-intro-oppo.png` | Pembuka pelajaran pratinjau |
| `f1-layout-lesson-material-oppo.png` | Materi bagian atas |
| `f1-layout-material-bottom-oppo.png` | Dialog bagian bawah |
| `f1-layout-exercise-oppo.png` | Latihan pilihan ganda |
| `f1-layout-feedback-oppo.png` | Umpan balik |
| `f1-layout-summary-oppo.png` | Ringkasan pratinjau |
| `f1-layout-home-scrolled-oppo.png` | Beranda setelah digulir ke bawah |
| `f1-layout-home-reset-oppo.png` | Beranda kembali ke atas setelah pindah tab |

Yang diperiksa: kartu tumbuh mengikuti isi, teks tidak terpotong, semua
daftar gulir mulai dari atas, dan posisi gulir Beranda kembali ke atas
setelah Beranda → Profil → Beranda.

### 3.2 Verifikasi rilis 0.2.0 dan sinkron lokal (OPPO)

Tanggal: 2026-11-06. Perangkat: OPPO `CPH1803` (`26b4c7b0`), Android 8.1
(API 27), `arm64-v8a`, layar 720×1520, densitas 320 dpi.

| Item | Nilai |
|---|---|
| APK | `Builds/Android/uabmeto-f1.apk` |
| Versi | `0.2.0` (`versionCode 2`) |
| Ukuran | 28.900.298 byte |
| SHA-256 | `EA0E0E7500E4169705CEC6DFB50C64E6D3BEC6BDA6549B282FAEE9796290C6BD` |
| Pemasangan | `adb install -r` → `Success` (tanpa uninstall) |
| Tanda tangan | `5a7f688` (sama dengan 0.1.0) |

Alur yang dijalankan:

1. Beranda menampilkan kartu **"PELAJARAN TERVALIDASI"** dan pelajaran
   "Sapaan waktu" (10 XP).
2. Alur pelajaran rilis: pembuka → materi → latihan → umpan balik →
   ringkasan "Pelajaran selesai".
3. "Simpan hasil" menaruh satu hasil di outbox (10 XP tertunda).
4. Profil menampilkan kartu "Mode online lokal — Aktif ·
   `http://127.0.0.1:8787`" dan satu hasil menunggu.
5. "Sinkronkan sekarang" → **"Sinkron selesai: 1 hasil terkirim."**
6. Server mencatat `POST /v1/sync 200` dan `applied_effects` 10 XP untuk
   `lesson-001-salam`, pengguna `dev-4febf86729044b5fb65c` (rio).
7. Tab Peringkat menampilkan "Minggu 2026-11-02 · 3 peserta": Aina 20 XP,
   rio 10 XP (aksen), Penguji 10 XP.

Bukti: `docs/testing/evidence/f1-device/f1-release-0.2.0-*.png`
(Beranda, ringkasan, Profil online, sinkron selesai, Peringkat).

Catatan jujur:

- Koneksi memakai `adb reverse tcp:8787 tcp:8787`, **bukan** Wi-Fi/LAN.
  Wi-Fi HP tidak terhubung ke hotspot PC.
- Banner tetap menampilkan "Offline" karena `internetReachability` melihat
  Wi-Fi seluler mati. Sinkron lokal tetap berhasil. Ini temuan UX yang belum
  diperbaiki.
- `devMode: false` karena pelajaran sudah `validated`; flag
  `DEV_ALLOW_XP_FOR_IN_REVIEW` tidak dipakai untuk hasil ini.
- Hanya pelajaran pertama per topik yang dapat dibuka dari UI. Pelajaran 2
  dan 3 Topik 1 ada di APK dan server, tetapi belum ada layar pemilih
  pelajaran; verifikasi sinkron kedua di perangkat karena itu belum dilakukan.

### 3.3 Perbaikan XP tertunda setelah sinkron

Temuan pada verifikasi 0.2.0 pertama: setelah sinkron sukses, Beranda masih
menampilkan "10 XP tertunda" karena `ProgressStore.pendingXp` hanya bertambah
saat hasil disimpan dan tidak pernah dikurangi ketika server mengonfirmasi.

Perbaikan:

- `OutboxStore.PendingXpTotal` menjumlahkan `clientClaimedXp` untuk item
  berstatus `pending`/`reserved`.
- `ProgressStore.ReconcilePendingXp` menyelaraskan `pendingXp` dengan angka itu.
- Rekonsiliasi dijalankan saat `AppRoot.Start` dan setelah
  `ShellView.ApplySyncReport`.
- Tes baru: `LocalStoreTests.Progress_ReconcilePendingXpFollowsOutbox`
  (EditMode) dan penegasan `PendingXp == 0` di `OnlineUiTests` (PlayMode).

Temuan lanjutan: statistik Profil menampilkan **"Antrean 1"** padahal kartu
di bawahnya berkata "Tidak ada hasil menunggu sinkron." Penyebabnya stat
memakai `Outbox.Count` (termasuk item terkonfirmasi), bukan `PendingCount`.
Setelah diperbaiki, stat menampilkan **Antrean 0**.

APK final: 28.900.298 byte, SHA-256
`EA0E0E7500E4169705CEC6DFB50C64E6D3BEC6BDA6549B282FAEE9796290C6BD`,
dipasang dengan `adb install -r` → `Success`. Setelah dibuka, Beranda
menampilkan **XP tertunda 0** dan **Selesai 1**; Profil menampilkan
**Pelajaran selesai 1, XP tertunda 0, Antrean 0**, dan mode online **Aktif**.
Tab Peringkat berhasil memuat data server setelah pemasangan ulang. Tes:
EditMode 82/82, PlayMode 27/27.

### 3.4 Verifikasi rilis 0.2.1: tiga pelajaran, replay, dan banner (OPPO)

Tanggal: 2026-11-07/08. Perangkat: OPPO `CPH1803` (`26b4c7b0`), Android 8.1
(API 27), `arm64-v8a`, 720×1520, 320 dpi.

| Item | Nilai |
|---|---|
| APK | `Builds/Android/uabmeto-f1.apk` |
| Versi | `0.2.1` (`versionCode 3`) |
| Ukuran | 28.907.506 byte |
| SHA-256 | `8E46C47E16E3B586818F6CDA9D9F7620E6F9B6533AEDA4AE3B8D3F22796BC6A3` |
| Pemasangan | `adb install -r` → `Success` (tanpa uninstall) |
| Tanda tangan | `5a7f688` |
| Tes | EditMode 87/87, PlayMode 31/31 |

Alur yang dijalankan:

1. Beranda menampilkan "Lanjutkan belajar" untuk pelajaran berikutnya.
   Menyelesaikan pelajaran 1 memajukan tombol ke pelajaran 2, lalu 3.
2. Kartu Topik 1 membuka daftar tiga pelajaran. Tiap baris menampilkan
   `Pelajaran N`, perkiraan menit, XP, dan tombol mulai. Pelajaran yang sudah
   selesai berbadge **`SELESAI`** dengan tombol "Ulangi latihan"; pelajaran
   yang belum selesai tetap bisa dibuka.
3. Pelajaran 1→2→3 diselesaikan penuh (pembuka → materi → latihan → umpan
   balik → ringkasan → simpan). Setelah tiga pelajaran: Beranda dan Profil
   menampilkan **Selesai 3**, **XP tertunda 20**, **Antrean 2** sebelum
   sinkron.
4. Pelajaran 1 diulang dari daftar. Ringkasan menampilkan **"Latihan ulang
   selesai"** dan catatan bahwa XP tidak bertambah; tombol `FinishRepeat`.
   Setelah replay, statistik tetap **Selesai 3** dan **XP tertunda 20**.
5. `adb reverse tcp:8787 tcp:8787` dilepas, lalu sinkron ditekan. Sinkron
   gagal dan banner menampilkan **"Server lokal tidak dapat dihubungi. Hasil
   tetap tersimpan."** Antrean tidak hilang.
6. `adb reverse` dipasang lagi, sinkron ditekan. Hasil terkirim, **XP
   tertunda 0** dan **Antrean 0**. Peringkat memuat data server: rio 40 XP,
   Aina 20 XP, Penguji 10 XP.

Bukti: `docs/testing/evidence/f1-device/f1-release-0.2.1-*.png`
(Beranda, daftar pelajaran selesai, pembuka pelajaran 3, ringkasan pelajaran
3, ringkasan replay, banner gagal sinkron, sinkron sukses, statistik Profil,
Peringkat).

Catatan jujur:

- Koneksi memakai `adb reverse`, bukan Wi-Fi/LAN. Melepas `adb reverse`
  mensimulasikan server tidak terjangkau, **bukan** mode pesawat; D1-04
  tetap **NOT RUN**.
- Banner "Offline" tidak lagi muncul hanya karena `internetReachability`
  salah saat server lokal masih terjangkau.
- Wi-Fi/LAN belum bisa diuji karena HP tidak terhubung ke hotspot PC.

### 3.5 Verifikasi Tutor AI rilis 0.3.0 (OPPO)

Tanggal: 2026-11-09/10. Perangkat: OPPO `CPH1803` (`26b4c7b0`), Android 8.1
(API 27), `arm64-v8a`, 720×1520, 320 dpi.

| Item | Nilai |
|---|---|
| APK | `Builds/Android/uabmeto-f1.apk` |
| Versi | `0.3.0` (`versionCode 4`) |
| Ukuran | 28.932.254 byte |
| SHA-256 | `BA99B129FF5CEDD5830DC443F162B2F6639540054C76DF9A57281B95C00FE713` |
| Pemasangan | `adb install -r` → `Success` (data lama tetap) |
| Server | `wrangler dev --local` port 8787, `TUTOR_PROVIDER=mock` |
| Koneksi | `adb reverse tcp:8787 tcp:8787` |
| Tes | EditMode 93/93, PlayMode 39/39, backend 25/25, eval Tutor 25/25 |

Alur yang dijalankan:

1. Tab Tutor AI menampilkan penjelasan batas, kuota "Sisa 10 dari 10
   pertanyaan hari ini", empat chip pertanyaan, kolom tulis, dan tombol Kirim.
2. Ketuk kolom tulis: keyboard muncul, panel tulis dan tombol Kirim bergeser
   ke atas keyboard (offset 275 unit kanvas ≈ 550 px), teks yang diketik
   terlihat penuh.
3. Satu ketukan "Kirim" saat keyboard terbuka mengirim pertanyaan, menutup
   keyboard, dan menampilkan jawaban.
4. "Apa arti kata Noka?" dijawab Noka = pagi, sumber "Berdasarkan: Sapaan
   waktu", status "Jawaban berasal dari materi tervalidasi."
5. "Siapa presiden Indonesia" dijawab "Belum dapat dipastikan dari materi
   tervalidasi saat ini…", tanpa sumber, status "Jawaban tidak dikarang."
6. Setelah dua pertanyaan, kuota menjadi "Sisa 8 dari 10"; `GET /v1/quota`
   dengan identitas perangkat mencatat `used: 2`.
7. `adb reverse` dilepas, pertanyaan dikirim → pesan "Tidak dapat menghubungi
   server lokal. Tutor AI membutuhkan server lokal yang aktif." berwarna
   `danger-text`; tidak ada jawaban palsu.
8. `adb reverse` dipasang lagi → "faen" dijawab pulang, sumber "Bertemu dan
   berpisah".
9. Kuota dihabiskan (10 pertanyaan) → "Sisa 0 dari 10", status kuota habis,
   tombol Kirim nonaktif.

Bukti: `docs/testing/evidence/f1-device/f1-release-0.3.0-tutor-*.png`
(8 berkas: tab, keyboard, jawaban, refusal, kuota, offline, pemulihan,
kuota habis).

Catatan jujur:

- Provider `mock` menghasilkan jawaban dari potongan chunk tervalidasi; model
  Workers AI nyata **belum diuji** karena mesin ini belum login Cloudflare.
- Tidak ada audio, ASR, atau penilaian pelafalan pada versi ini.
- Melepas `adb reverse` bukan uji mode pesawat; D1-04 tetap **NOT RUN**.
- Kuota D1 lokal direset sebelum rangkaian bukti agar dimulai dari 10/10;
  reset hanya untuk pengujian lokal.

### 3.6 Verifikasi visual rilis 0.4.0 (OPPO)

Tanggal: 2026-11-12. Perangkat: OPPO `CPH1803` (`26b4c7b0`), Android 8.1
(API 27), 720×1520, 320 dpi.

| Item | Nilai |
|---|---|
| APK | `Builds/Android/uabmeto-f1.apk` |
| Versi | `0.4.0` (`versionCode 5`) |
| Ukuran | 28.942.014 byte |
| SHA-256 | `B02341187AEAB6621D8AC70E113742A26FBF114C0E6024F676D9233FBD1492F0` |
| Pemasangan | `adb install -r` → `Success`; data lama tetap (progres 3/36, Topik 1 3/3) |
| Tes | EditMode 93/93, PlayMode 41/41, backend 25/25, eval Tutor 25/25, kontras 12/12 |

Yang diperiksa pada layar:

1. Beranda menampilkan satu kartu hero: sapaan "Selamat datang, rio", motif
   tiga belah ketupat, badge pelajaran tervalidasi, judul, metadata, label
   "3 dari 36 pelajaran", bilah progres terisi, dan tombol Mulai.
2. Tiga kotak statistik sudah diganti satu baris ramping dengan dua pemisah
   garis: Streak 0, XP tertunda 0, Selesai 3.
3. Setelah digulir, 12 baris topik menampilkan bilah progres 4 dp dan angka
   "3/3" pada Topik 1; baris lain "0/3".
4. Tab bar 68 dp: tab aktif berpil `accent` 12%, ikon dan label `accent`,
   label tebal; tab pasif `text-muted`.
5. Tab Tutor AI dan Profil tetap berfungsi dan ikut memakai tab pil baru.

Bukti: `docs/testing/evidence/f1-device/f1-release-0.4.0-*.png` (4 berkas:
Beranda, daftar topik setelah gulir, Tutor AI, Profil).

Catatan jujur:

- Ini pemeriksaan visual pada satu perangkat portrait, bukan uji TalkBack,
  skala font, landscape, atau mode pesawat. D1-04..D1-06 tidak berubah.
- `ScreenshotTests` di batchmode hanya menghasilkan gambar kosong karena
  `-nographics`; karena itu tangkapan perangkat menjadi bukti visual utama.
- Gerak tekan/transisi tidak dapat dibuktikan lewat tangkapan statis; yang
  diuji otomatis adalah easing dan keadaan akhirnya saat gerak dimatikan.

### 3.7 Verifikasi rilis 0.5.0 (2026-11-12)

Tanggal: 2026-11-12 (WITA). APK 0.5.0 (`versionName=0.5.0`,
`versionCode=6`, SHA-256 `4901E8D3…A166`) dipasang dan dibuka di OPPO
`CPH1803` (Android 8.1/API 27, 720×1520). Tes: EditMode **99/99**,
PlayMode **43/43**, kontras **12/12**, validator **PASS** (0 error,
4 warning audio contoh).

**Skala font (D1-06/F1-21).** `UiScale` membaca `Configuration.fontScale`
lewat JNI dan membatasi 1.0–2.0; `UIFactory` memakai faktor itu untuk teks,
ikon, target sentuh, padding, dan jarak. Modal dibuat dapat digulir lewat
`ModalCardFitter`.

| Skala | Cara | Hasil |
|---|---|---|
| 1.0× | slider font 0.9 (dibatasi ke 1.0) | Semua layar muat; tidak ada teks terpotong |
| 1.35× | slider perangkat (langkah terdekat 1.3×) | Logcat `fontScale sistem 1,35 diterapkan sebagai 1,35`; Beranda, Materi, Profil, dan label tab muat |
| 1.6× | slider maksimum perangkat | Logcat `fontScale sistem 1,60 diterapkan sebagai 1,60`; teks membungkus di batas kata, tombol Mulai tetap dapat dicapai dengan gulir |
| 2.0× | `LayoutTests` PlayMode | Lulus di Editor; **tidak dapat diuji di perangkat ini** karena slider berhenti di 1.6× |

Temuan saat uji 1.6×: label statistik Beranda terpotong di tengah kata
("Strea/k hari") dan label "Antrean" di Profil patah karena lebar tiga kartu
tidak sama. Perbaikan: label statistik memakai autosize dengan batas bawah,
dan lebar sel/kartu dipaksa sama (`minWidth = preferredWidth = 0`,
`flexibleWidth = 1`). Setelah perbaikan, tidak ada kata yang patah di 1.6×.

**Mode offline (D1-04).** Wi-Fi dan data seluler dimatikan
(`svc wifi disable`, `svc data disable`); `ping 8.8.8.8` gagal. APK
dipasang bersih (uninstall + install), nama "rio" diisi, lalu pelajaran 1
diselesaikan sampai ringkasan: "10 XP menunggu verifikasi server". Setelah
"Simpan hasil", Beranda menampilkan banner **"Offline — 1 hasil menunggu
sinkron. Pembelajaran tetap bisa dilanjutkan."**, XP tertunda 10, Selesai 1,
Topik 1 1/3. Wi-Fi dinyalakan lagi, tetapi tidak ada jaringan/hotspot dan
server lokal tidak berjalan, jadi **sinkron pemulihan tidak diuji**.

**Alur pelajaran (D1-03).** Pada APK 0.5.0, pelajaran 1 dijalankan penuh:
pembuka → materi (kosakata, contoh, dialog) → latihan → umpan balik "Benar"
→ ringkasan "Latihan ulang selesai" dengan catatan XP tidak bertambah. Ini
mengulang bukti 0.2.1 pada build dan tata letak terbaru.

**Waiver rotasi (D1-05).** `aapt2 dump xmltree` menunjukkan
`android:screenOrientation=1` (portrait). Upaya memaksa rotasi lewat
`settings put system user_rotation 1` dan `cmd window user-rotation lock 1`
gagal pada ColorOS/API 27 (permission `WRITE_SETTINGS` dan tidak ada
implementasi perintah). Aplikasi tetap portrait; tidak ada bukti landscape.

Bukti: `docs/testing/evidence/f1-device/f1-release-0.5.0-*.png` (12 berkas:
Beranda 1×, 1.35×, 1.6×, statistik Beranda 1.6×, statistik Profil 1.6×,
pembuka/materi/latihan/umpan balik/ringkasan replay, banner offline, dan
hasil tersimpan offline).

Catatan jujur:

- Uji offline memakai Wi-Fi + data dimatikan, bukan toggle mode pesawat
  sistem; efeknya sama (tidak ada rute jaringan) dan dibuktikan dengan
  `ping` yang gagal.
- Setelah Wi-Fi dinyalakan, perangkat tetap tanpa internet; banner offline
  tetap tampil. Sinkron pemulihan butuh hotspot/server dan belum dijalankan.
- Slider font OPPO tidak punya nilai 1.3×; nilai terdekat adalah 1.35×.
- TalkBack tidak diuji dan tidak diklaim; lihat §5.
- Tangkapan layar perangkat adalah bukti visual; bukan rekaman video.

## 4. Temuan dan perbaikan

### 4.1 Scroll tidak dapat digeser

- **Gejala:** drag pada Beranda/Profil tidak menggeser konten.
- **Penyebab:** `ScrollRect` tidak punya `Graphic` raycast target di viewport;
  kartu dan teks memakai `raycastTarget=false`.
- **Perbaikan:** viewport diberi `Image` transparan dengan
  `raycastTarget=true` di `UIFactory.CreateScrollView`.
- **Regresi:** tes PlayMode memastikan viewport punya Graphic raycast target.

### 4.2 Kartu identitas Profil bertumpuk

- **Gejala:** nickname, catatan, dan tombol saling menimpa; avatar melebar.
- **Penyebab:** tinggi kartu tetap 170 dp dan `childForceExpandWidth=true`.
- **Perbaikan:** kartu memakai `VerticalLayoutGroup` dinamis, lebar anak
  dikontrol, avatar 64 dp, dan teks/tombol memakai `flexibleWidth`.
- **Regresi:** tes PlayMode memeriksa tinggi kartu, lebar avatar, dan
  tumpang tindih teks/tombol.

### 4.3 Kartu modal bertumpuk

- **Gejala:** judul, isi, input, dan tombol modal saling menimpa; tombol
  Batal tidak ada.
- **Penyebab:** `ContentSizeFitter` dipasang pada panel tanpa layout group;
  isi diletakkan di child group yang di-stretch.
- **Perbaikan:** panel modal memakai `VerticalLayoutGroup` + fitter; tombol
  `Batal` ditambahkan pada dialog nama.
- **Regresi:** tes PlayMode memeriksa tinggi kartu, urutan tanpa tumpang
  tindih, dan keberadaan tombol Batal/Simpan.

### 4.4 Garis putih di tepi kiri

Garis melengkung putih di tepi kiri terlihat di semua layar aplikasi. Garis
yang sama terlihat di home screen sistem, jadi ini panel tepi OEM, bukan
bagian UI aplikasi. Tidak dihitung sebagai temuan aplikasi.

### 4.5 Keyboard Android menutup kolom tulis

- **Gejala:** pada build 0.3.0 pertama, membuka keyboard di tab Tutor AI
  menutupi panel tulis; teks yang diketik tidak terlihat.
- **Penyebab:** Unity Android (Game Activity) tidak melakukan `adjustResize`,
  dan `TouchScreenKeyboard.visible`/`area` selalu kosong di perangkat ini
  meski log native mencatat "Keyboard Visible = True".
- **Perbaikan:** `KeyboardInset.cs` membaca tinggi keyboard dari window insets
  Android (`getWindowVisibleDisplayFrame`), lalu menggeser panel tulis dan
  menyusutkan area gulir. Panel tidak digeser selama jari masih menekan agar
  pointer-up tombol Kirim tidak jatuh di luar tombol; `Send()` juga memanggil
  `DeactivateInputField()` setelah mengirim.
- **Verifikasi:** APK final 0.3.0 (SHA-256 `BA99B129…`) — keyboard terbuka,
  panel tulis dan tombol Kirim terlihat di atas keyboard, satu ketukan Kirim
  mengirim, menutup keyboard, dan menampilkan jawaban. Bukti:
  `f1-release-0.3.0-tutor-keyboard-oppo.png` dan
  `f1-release-0.3.0-tutor-answer-oppo.png`.
- **Batasan:** offset mengikuti window insets sehingga menyisakan celah kecil
  di atas keyboard (±77 px pada perangkat ini); ini kosmetik, bukan
  teks terpotong.

### 4.6 Label statistik patah di tengah kata pada font 1.6×

- **Gejala:** pada skala font perangkat 1.6×, Beranda menampilkan
  "Strea/k hari"; Profil menampilkan "Antrea/n" pada kartu ketiga.
- **Penyebab:** label dibungkus hanya di dalam sel selebar sepertiga baris;
  di Profil, lebar kartu tidak sama karena lebar pilihan teks ikut menentukan
  pembagian `HorizontalLayoutGroup`.
- **Perbaikan:** `UIFactory.AllowShrinkToFit` memberi autosize dengan batas
  bawah pada label statistik; sel Beranda dan kartu Profil diberi
  `minWidth = 0`, `preferredWidth = 0`, dan `flexibleWidth = 1` agar lebarnya
  benar-benar dibagi rata.
- **Regresi:** tes PlayMode 1.3×/2.0× memeriksa teks tidak terpotong di
  Beranda, Materi, dan Profil, serta label tab tetap satu baris. Perangkat
  1.6× diperiksa ulang lewat tangkapan layar setelah perbaikan.

## 5. Batasan yang belum ditutup

- Tidak ada audio pada pelajaran `validated`, jadi alur audio dan consent
  tidak dapat diuji. Tiga pelajaran Topik 1 divalidasi Ibu Nofri pada
  2026-11-12 tanpa koreksi; bukti validasi berupa pesan/chat dan disimpan di
  luar repo.
- F1 dikunci portrait-only atas keputusan pemilik 2026-11-12. D1-05 dicatat
  **WAIVED**, bukan lulus; tidak ada klaim landscape perangkat.
- TalkBack belum diuji dan tidak diklaim. Skala font sistem kini dibaca dan
  diuji sampai 1.6× di perangkat; 2.0× hanya terbukti di PlayMode karena
  slider perangkat berhenti di 1.6×. Keyboard Android diuji untuk layar Tutor
  AI (lihat §4.5), belum untuk layar lain karena belum ada kolom tulis lain.
- Banner offline dan penyimpanan hasil offline sudah diuji; sinkron pemulihan
  setelah online belum diuji karena tidak ada hotspot/server lokal saat uji.
- Tidak ada profiling performa atau pengukuran startup.
- Build 0.5.0 masih mencatat 3 warning IL2CPP "Method was given it's own cpp
  file…" (informasional, bukan warning kompilasi C#/shader).
- Tutor AI memakai provider `mock`; kualitas model Workers AI nyata belum
  diukur dan belum ada kredensial Cloudflare di mesin pengembangan.
