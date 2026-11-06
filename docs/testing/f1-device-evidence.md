# Bukti Uji Perangkat F1

Tanggal: 2026-11-03 (WITA); diperbarui 2026-11-06 (WITA)
Status: **PARTIAL** — instalasi, peluncuran, UI dasar, tombol back, alur
pelajaran rilis (satu pelajaran, portrait), dan sinkron lokal lulus di OPPO
`CPH1803`; offline mock, landscape, dan skala font belum.

## 1. Perangkat dan APK

| Item | Nilai |
|---|---|
| Perangkat | Xiaomi `24117RN76O` |
| Android | 16 (API 36) |
| Layar | 1080×2400, 450 dpi |
| ADB | `device` (terotorisasi) |
| Paket | `id.uabmeto.app` |
| Versi | `0.1.0` (`versionCode 1`) |
| minSdk / targetSdk | 26 / 36 |
| ABI | `arm64-v8a` |
| APK | `Builds/Android/uabmeto-f1.apk` |
| Ukuran | 28.278.020 byte |
| SHA-256 | `D4416E0250AD0F188A45B142BE6A1764850D4C4BD0D6BF18A0FE3AF97BA3C465` |
| firstInstallTime | 2026-11-03 13:56:17 |
| lastUpdateTime | 2026-11-03 14:47:55 |

`adb install -r` berhasil. `dumpsys package` mengonfirmasi versi, minSdk,
targetSdk, dan waktu instalasi di atas.

## 2. Hasil per butir gate perangkat

| ID | Status | Bukti |
|---|---|---|
| D1-01 APK terpasang | **PASS** | `adb install -r` → `Success`; `dumpsys package` |
| D1-02 Dibuka tanpa crash | **PASS** | `UnityPlayerGameActivity` foreground; logcat tanpa fatal/exception aplikasi |
| D1-03 Alur pelajaran | **BLOCKED** | APK rilis tidak memuat pelajaran `validated`; fixture Editor tidak boleh masuk rilis |
| D1-04 Mode offline | **NOT RUN** | Belum diuji dengan mode pesawat/mock reachability |
| D1-05 Rotasi portrait ↔ landscape | **BLOCKED (perangkat)** | Perangkat/launcher memaksa portrait; rotasi paksa menghasilkan letterbox ±885×1080 |
| D1-06 Font sistem 1.3×/2.0× | **NOT RUN / BLOCKED (fitur)** | Belum diuji; audit statis: tidak ada pembacaan `AndroidConfiguration.fontScale` |
| D1-07 Tombol back | **PASS** | Profil → Beranda; dialog nama → Profil |

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
| Ukuran | 28.900.302 byte |
| SHA-256 | `957CECCD4A2814979EA362F327112659856141C5F374844C2CD3915BC4AC3FAB` |
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

APK diperbaiki: 28.900.302 byte, SHA-256
`957CECCD4A2814979EA362F327112659856141C5F374844C2CD3915BC4AC3FAB`,
dipasang dengan `adb install -r` → `Success`. Setelah dibuka, Beranda
menampilkan **XP tertunda 0** dan **Selesai 1**. Tes: EditMode 82/82,
PlayMode 27/27.

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

## 5. Batasan yang belum ditutup

- Tidak ada audio pada pelajaran `validated`, jadi alur audio dan consent
  tidak dapat diuji. Tiga pelajaran Topik 1 `validated` atas override pemilik,
  bukan reviewer independen.
- Perangkat/launcher portrait-only; uji landscape perangkat tidak sah.
- TalkBack, keyboard Android, dan skala font sistem belum diuji. Aplikasi
  juga belum membaca `AndroidConfiguration.fontScale`, jadi D1-06 tidak
  dapat lulus tanpa perubahan kode.
- Banner offline belum diuji dengan mode pesawat.
- Tidak ada profiling performa atau pengukuran startup.
- Build masih memiliki 3 warning TMP/IL2CPP.
