# Bukti Uji Perangkat F1

Tanggal: 2026-11-03 (WITA); diperbarui 2026-11-09 (WITA)
Status: **PARTIAL** — instalasi, peluncuran, UI dasar, tombol back, alur
tiga pelajaran rilis (portrait), replay tanpa XP ganda, banner gagal-server,
sinkron lokal, dan Tutor AI teks lulus di OPPO `CPH1803`; offline mock (D1-04),
landscape, dan skala font belum.

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
| Versi | `0.3.0` (`versionCode 4`) |
| minSdk / targetSdk | 26 / 36 |
| ABI | `arm64-v8a` |
| APK | `Builds/Android/uabmeto-f1.apk` |
| Ukuran | 28.932.254 byte |
| SHA-256 | `BA99B129FF5CEDD5830DC443F162B2F6639540054C76DF9A57281B95C00FE713` |
| firstInstallTime | 2026-11-06 00:15:17 |
| lastUpdateTime | 2026-11-09/10 (instalasi APK final 0.3.0) |
| Tanda tangan | `5a7f688` (sama dengan 0.1.0/0.2.0/0.2.1) |

`adb install -r` berhasil. `dumpsys package` mengonfirmasi versi, minSdk,
targetSdk, ABI, tanda tangan, dan waktu instalasi di atas. Perangkat Xiaomi
`24117RN76O` (Android 16, 1080×2400) dipakai pada verifikasi 0.1.0 dan
dicatat di bagian 2–3.

## 2. Hasil per butir gate perangkat

| ID | Status | Bukti |
|---|---|---|
| D1-01 APK terpasang | **PASS** | `adb install -r` → `Success`; `dumpsys package` (0.3.0, `versionCode 4`) |
| D1-02 Dibuka tanpa crash | **PASS** | `UnityPlayerGameActivity` foreground; logcat tanpa fatal/exception aplikasi |
| D1-03 Alur pelajaran | **PASS (0.2.1)** | Tiga pelajaran rilis Topik 1 dibuka berurutan lewat daftar; replay tanpa XP ganda |
| D1-04 Mode offline | **NOT RUN** | Banner gagal-server/offline sudah diuji dengan melepas `adb reverse`, tetapi mode pesawat/mock reachability belum |
| D1-05 Rotasi portrait ↔ landscape | **BLOCKED (perangkat)** | Perangkat/launcher memaksa portrait; rotasi paksa menghasilkan letterbox ±885×1080 |
| D1-06 Font sistem 1.3×/2.0× | **NOT RUN / BLOCKED (fitur)** | Belum diuji; audit statis: tidak ada pembacaan `AndroidConfiguration.fontScale` |
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

## 5. Batasan yang belum ditutup

- Tidak ada audio pada pelajaran `validated`, jadi alur audio dan consent
  tidak dapat diuji. Tiga pelajaran Topik 1 `validated` atas override pemilik,
  bukan reviewer independen.
- Perangkat/launcher portrait-only; uji landscape perangkat tidak sah.
- TalkBack dan skala font sistem belum diuji. Aplikasi juga belum membaca
  `AndroidConfiguration.fontScale`, jadi D1-06 tidak dapat lulus tanpa
  perubahan kode. Keyboard Android kini diuji untuk layar Tutor AI
  (lihat §4.5), tetapi belum untuk layar lain karena belum ada kolom tulis
  lain di F1.
- Banner gagal-server sudah diuji dengan melepas `adb reverse`, tetapi
  banner offline belum diuji dengan mode pesawat (D1-04).
- Tidak ada profiling performa atau pengukuran startup.
- Build 0.3.0 masih memiliki 3 warning shader TMP.
- Tutor AI memakai provider `mock`; kualitas model Workers AI nyata belum
  diukur dan belum ada kredensial Cloudflare di mesin pengembangan.
