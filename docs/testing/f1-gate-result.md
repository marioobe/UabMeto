# Laporan Gate F1

Tanggal: 2026-11-03 (WITA); diperbarui 2026-11-09 (WITA)
Lingkungan: Windows, Unity `6000.6.0f1`, Android build-tools `36.1.0`.
Status gate: **TIDAK LULUS.** Build, tes UI, dan verifikasi dasar perangkat
fisik lulus; syarat reviewer independen, audio/consent, aksesibilitas penuh,
dan sisa gate perangkat belum terpenuhi.

Pembaruan 2026-11-05: tiga pelajaran Topik 1 sudah diisi dari elisitasi
`SPK-001` dan berstatus **`in_review`** (bukan `validated`). Aplikasi punya
mode pratinjau berlabel "Pratinjau — belum divalidasi" yang tidak mencatat
progres atau XP. Mode ini tidak mengubah status gate: belum ada reviewer
independen, audio, atau consent.

Pembaruan 2026-11-06: stabilisasi layout F1. Tinggi kartu sekarang dihitung
dari isi (bukan angka tetap), semua daftar gulir kembali ke atas saat layar
dibangun ulang, dan kanvas memakai acuan responsif portrait 360×640 /
landscape 640×360. Lima tes PlayMode baru mengunci perilaku ini. Build rilis
dan pratinjau dibangun ulang; status gate tetap **TIDAK LULUS** karena
blocker konten, audio, aksesibilitas, dan perangkat tidak berubah.

Pembaruan 2026-11-06 (versi 0.2.0): tiga pelajaran Topik 1 dinyatakan
**`validated` atas keputusan pemilik proyek**. Ini **override pemilik, bukan
peninjauan reviewer independen**, dan dicatat apa adanya di `notes` serta
`validation.reviewedBy`. APK rilis `0.2.0` memuat ketiga pelajaran dan alur
pelajaran rilis diuji di OPPO `CPH1803`. Sinkron XP ke Worker lokal berhasil
di perangkat (10 XP, `devMode: false`), dan tab Peringkat menampilkan data
server. Gate F1 tetap **TIDAK LULUS** karena reviewer independen, audio dan
consent, D1-04/D1-05/D1-06, serta aksesibilitas penuh belum terpenuhi.

Pembaruan 2026-11-08 (versi 0.2.1): tiga pelajaran Topik 1 kini dapat dibuka
berurutan lewat layar **daftar pelajaran per topik**. "Lanjutkan belajar"
maju ke pelajaran berikutnya yang belum selesai, pelajaran selesai diberi
badge `SELESAI` dan tetap bisa diulang sebagai latihan tanpa menambah XP.
Banner offline sekarang dibedakan: "Offline" hanya saat perangkat benar-benar
tanpa internet, sedangkan kegagalan server lokal dilaporkan sebagai
"Server lokal tidak dapat dihubungi. Hasil tetap tersimpan." Alur 1→2→3,
replay tanpa XP ganda, banner gagal-sinkron, dan pemulihan sinkron diuji di
OPPO `CPH1803`. Tes EditMode **87/87** dan PlayMode **31/31** lulus. Gate F1
tetap **TIDAK LULUS**; blocker tidak berubah.

Pembaruan 2026-11-09 (versi 0.3.0): **Tutor AI teks** tersedia di tab
Tutor AI. Jawaban hanya diambil dari korpus tiga pelajaran Topik 1 yang
`validated`, selalu dalam Bahasa Indonesia, menyebut pelajaran sumber, dan
menolak dengan "Belum dapat dipastikan…" bila referensi tidak ada. Kuota
10 pertanyaan/akun/hari dihitung di server. Provider lokal memakai `mock`
karena mesin ini belum memiliki kredensial Cloudflare; model Workers AI
sungguhan **belum diuji**. Tes EditMode **93/93**, PlayMode **39/39**,
backend **25/25**, dan eval Tutor **25/25** lulus. Alur jawaban, refusal,
kuota berkurang, server tidak terjangkau, pemulihan, dan kuota habis diuji
di OPPO `CPH1803`. Perbaikan keyboard Android juga diuji di sana: panel tulis
naik ke atas keyboard, teks terlihat, dan satu ketukan Kirim mengirim lalu
menutup keyboard. Rincian dan batasan:
`docs/testing/tutor-ai-quality.md`. Gate F1 tetap **TIDAK LULUS**; gate AI
produksi juga belum lulus.

## 1. Ringkasan keputusan

| Area | Hasil |
|---|---|
| Build Android F1 | **PASS** — `Builds/Android/uabmeto-f1.apk` |
| Build pratinjau | **PASS** — `Builds/Android/uabmeto-f1-preview.apk` (demo, memuat `in_review`) |
| Tes EditMode | **PASS** — 93 lulus, 0 gagal, 0 dilewati |
| Tes PlayMode | **PASS** — 39 lulus, 0 gagal, 0 dilewati |
| Tes backend | **PASS** — 25 lulus (termasuk 14 tes Tutor AI) |
| Eval Tutor AI | **PASS (mock)** — 25/25 kasus emas; model nyata belum diuji |
| Validator konten | **PASS** — 0 error, 4 warning audio di `content/samples/` |
| Kontras F1 | **PASS** — 12/12 pasangan ≥ 4.5:1 |
| Target sentuh F1 | **PASS (UI F1)** — 4/4 tes aksesibilitas/layout |
| Materi Topik 1 | **VALIDATED (override pemilik)** — 3 pelajaran, 50 item elisitasi `SPK-001` |
| Konten tervalidasi | **ADA** — 3 pelajaran Topik 1, tanpa reviewer independen |
| Perangkat fisik | **PARTIAL** — D1-01, D1-02, D1-03 (tiga pelajaran, 0.2.1), Tutor AI teks (0.3.0), D1-07 lulus; D1-04..D1-06 belum |
| Gate F1 | **TIDAK LULUS** |

## 2. Build

### 2.1 Build rilis

- Path: `D:\01_Uab_Meto\Builds\Android\uabmeto-f1.apk`
- Ukuran: **28.294.252 byte**
- SHA-256: `B4D1CA595929535C71FD6F8922DFBFA254A4C3DFE4C848B5D3CE9B38ED9098F7`
- Hasil: `Succeeded`, 0 error, 1 warning
  (`Builds/logs/unity-build-release-layout.log`)
- Warning tunggal: `TextMeshPro/Mobile/Distance Field` memakai
  `#pragma enable_d3d11_debug_symbols` yang sudah deprecated. Ini warning
  shader TMP, bukan error dan bukan konten.
- Durasi build: 61,4 detik (inkremental)
- Paket: `id.uabmeto.app`
- Versi: `0.1.0` (`versionCode 1`)
- `minSdkVersion`: 26
- `targetSdkVersion`: 36
- ABI: `arm64-v8a` saja
- Backend: IL2CPP Release
- Ringkasan mesin: `Builds/Android/build-summary.json`

Catatan warning: build penuh sebelumnya mencatat 3 warning TMP/IL2CPP.
Build inkremental 2026-11-06 mencatat 1 warning shader TMP karena berkas lain
tidak dikompilasi ulang. Karena itu F1-25 tetap **PARTIAL** sampai build
bersih dari nol diulang tanpa cache.

### 2.2 Build pratinjau (demo, bukan rilis)

- Path: `D:\01_Uab_Meto\Builds\Android\uabmeto-f1-preview.apk`
- Ukuran: **28.300.114 byte**
- SHA-256: `491B5FB619F82521024B794B4A452BB2D6CF440B0855C8ECD0C5E7996E1971FE`
- Hasil: `Succeeded`, 0 error, 4 warning
- Build ini memuat tiga pelajaran `in_review` di `Resources/Content/preview`.
- **Jangan** disebut build rilis dan jangan dipakai untuk gate F1.

### 2.3 Pemeriksaan isi APK 0.1.0

Bagian ini mendokumentasikan APK `0.1.0`, yang tidak memuat pelajaran
`validated`. APK `0.2.0` sengaja memuat tiga pelajaran Topik 1 yang sudah
`validated`; lihat 2.4.

Pemeriksaan dilakukan dengan membuka zip APK dan memindai isi tiap entry
(termasuk `global-metadata.dat` dan aset Unity), bukan hanya nama berkas.

| String | `uabmeto-f1.apk` | `uabmeto-f1-preview.apk` |
|---|---|---|
| `lesson-001-salam` | tidak ada | ada |
| `lesson-002-salam-bertemu` | tidak ada | ada |
| `lesson-003-salam-sopan` | tidak ada | ada |
| `mail kabar on'me` | tidak ada | ada |
| `sium hit manekan` | tidak ada | ada |
| `SRC-SPK-001` | tidak ada | ada |
| `na ta'eko ten` | tidak ada | ada |
| `PRATINJAU` | hanya di `global-metadata.dat` (literal kode UI) | hanya di `global-metadata.dat` |

Kesimpulan: build rilis **0.1.0** tidak memuat materi `in_review`, teks Dawan,
ID pelajaran, atau ID sumber. String label `PRATINJAU` tetap ada karena
merupakan literal kode UI, bukan konten; saat tidak ada pelajaran
pratinjau, kartu pratinjau tidak pernah dirender.

String `UNVALIDATED`, `PLACEHOLDER`, dan `FIXTUR PENGEMBANGAN` juga hanya
muncul di `global-metadata.dat` sebagai literal kode (pemeriksaan status dan
label UI fixture). Tidak ada token placeholder atau teks fixture di aset
konten, dan tidak ada pelajaran fixture di APK (`lesson-fixture-salam`
absen).

Pemeriksaan lain:

- `aapt dump badging` mengonfirmasi paket, versi, minSdk 26, targetSdk 36,
  dan label `Uab Meto`.
- `aapt list` hanya memuat `lib/arm64-v8a/` (tidak ada ABI lain).
- `topics.json` ada di aset build; `Resources/Content/lessons` dan
  `Resources/Content/preview` hanya berisi `.gitkeep` setelah
  `F1Setup.SyncContent(false)`, jadi build rilis menampilkan empty state.
- Pemindaian APK untuk pola kredensial (`AIza...`, private key,
  `service_account`, `client_secret`, `SUPABASE_SERVICE_ROLE`, `sk-...`)
  menghasilkan 0 temuan. Ini bukan jaminan keamanan menyeluruh.

### 2.4 Build rilis 0.2.0 (2026-11-07)

- Path: `Builds/Android/uabmeto-f1.apk`
- Ukuran: **28.900.298 byte**
- SHA-256: `EA0E0E7500E4169705CEC6DFB50C64E6D3BEC6BDA6549B282FAEE9796290C6BD`
- Hasil: `Succeeded`, 0 error, 6 warning
- Durasi build: 257,4 detik (build inkremental)
- Versi: `0.2.0` (`versionCode 2`)
- Isi: tiga pelajaran Topik 1 berstatus `validated` (override pemilik)
- Folder `Resources/Content/preview` kosong (hanya `.gitkeep`), jadi tidak
  ada materi pratinjau yang ikut ke APK rilis
- Ringkasan mesin: `Builds/Android/build-summary.json`

Perubahan isi dibanding `0.1.0` disengaja: pelajaran Topik 1 sekarang
`validated`, sehingga teks Dawan-nya masuk ke build rilis dan dapat memberi
XP. Ini **bukan** bukti reviewer independen; lihat catatan override di
`content/lessons/*.json`.

Build ini juga memuat perbaikan rekonsiliasi XP tertunda: setelah server
mengonfirmasi hasil, angka "XP tertunda" turun mengikuti sisa outbox, baik
saat aplikasi dibuka maupun setelah sinkron. Di OPPO `CPH1803`, Beranda
menampilkan **XP tertunda 0** dan **Selesai 1** setelah sinkron sukses.
Sebelumnya angka tetap 10 karena `pendingXp` tidak pernah dikurangi.

Build final juga memperbaiki statistik Profil: "Antrean" memakai
`Outbox.PendingCount`, bukan `Outbox.Count`, sehingga tidak lagi menampilkan
angka antrean setelah semua hasil terkonfirmasi.

### 2.5 Build rilis 0.2.1 (2026-11-07)

- Path: `Builds/Android/uabmeto-f1.apk`
- Ukuran: **28.907.506 byte**
- SHA-256: `8E46C47E16E3B586818F6CDA9D9F7620E6F9B6533AEDA4AE3B8D3F22796BC6A3`
- Hasil: `Succeeded`, 0 error, 4 warning
- Durasi build: 243,2 detik (build inkremental)
- Versi: `0.2.1` (`versionCode 3`)
- Ringkasan mesin: `Builds/Android/build-summary.json`
- Isi: tiga pelajaran Topik 1 `validated` (override pemilik), layar daftar
  pelajaran, navigasi pelajaran berikutnya, pencegahan XP ganda saat replay,
  dan banner kegagalan server yang terpisah dari status offline
- Folder `Resources/Content/preview` kosong (hanya `.gitkeep`), jadi tidak
  ada materi pratinjau di APK rilis

Catatan: 4 warning berasal dari shader/`AndroidSDKTools`; tidak ada error
kompilasi. F1-25 tetap **PARTIAL** karena build belum bersih dari nol.

### 2.6 Build rilis 0.3.0 (2026-11-09/10)

- Path: `Builds/Android/uabmeto-f1.apk`
- Ukuran: **28.932.254 byte**
- SHA-256: `BA99B129FF5CEDD5830DC443F162B2F6639540054C76DF9A57281B95C00FE713`
- Hasil: `Succeeded`, 0 error, 3 warning (shader TMP `enable_d3d11_debug_symbols`)
- Durasi build: 144,8 detik (inkremental)
- Versi: `0.3.0` (`versionCode 4`), terverifikasi lewat `aapt2 dump badging`
- Ringkasan mesin: `Builds/Android/build-summary.json`
- Isi: tab **Tutor AI teks**, kontrak `POST /v1/tutor/ask`, kuota Tutor AI di
  klien, perbaikan keyboard Android, serta seluruh isi 0.2.1
- Folder `Resources/Content/preview` hanya berisi `.gitkeep`; tidak ada materi
  pratinjau yang ikut ke APK rilis
- Versi dinaikkan lewat `ProjectConfigurator.BundleVersion` dan
  `bundleVersionCode`, karena `Apply()` menimpa `ProjectSettings` saat build
- Build ini menggantikan build 0.3.0 antara (`ABC76522…`, 28.926.554 byte)
  yang belum memuat perbaikan keyboard; UI Tutor tidak berubah, hanya
  `KeyboardInset.cs` dan pemanggilan `DeactivateInputField()`

## 3. Tes yang dijalankan

### 3.1 EditMode

Hasil: **93/93 lulus, 0 gagal, 0 dilewati** (2026-11-09,
`Builds/logs/editmode-results.xml`).

- 19 tes `ApiClient`/`SyncService`: parsing respons, XP resmi server,
  penolakan konten belum valid, konflik, transport gagal, dan batas percobaan.
- 6 tes `TutorApiClientTests`: pertanyaan + riwayat, sumber, kuota, penolakan
  pertanyaan kosong, kode `quota_exceeded`, `provider_error`, dan
  `network_error`.
- `TopicPathTests` — 12 ID topik persis PRD, tanpa topik ujian, tanpa label
  dialek luar TTU di UI/registry.
- `ContentRepositoryTests` — empty state, validated/published, penolakan
  UNVALIDATED, placeholder, contoh kosong, serta gate pratinjau:
  `in_review` hanya masuk `PreviewLessons`, `draft`/placeholder/tanpa sumber
  ditolak, dan pratinjau bisa dimatikan lewat satu parameter.
- `LocalStoreTests` — nickname 3–20, persistensi, XP tertunda idempoten,
  outbox dan rekonsiliasi.
- `PublishGateTests`, `SyncReconcilerTests`, `XpAwardPolicyTests`,
  `FontGlyphTests` tetap lulus.
- `BannerPolicyTests` (5 tes) — "Offline" hanya saat tidak ada internet;
  kegagalan server lokal tidak diberi label offline; hasil yang tetap
  tersimpan tidak dihapus dari outbox.

### 3.2 PlayMode

Hasil: **39/39 lulus, 0 gagal, 0 dilewati** (2026-11-09,
`Builds/logs/playmode-results.xml`).

- 8 tes `TutorViewTests`: kuota awal, chip pertanyaan, jawaban + sumber,
  refusal, kuota habis (tombol kirim nonaktif), `quota_exceeded` dari server,
  galat jaringan, dan state tanpa mode online.
- 8 tes `OnlineUiTests`: kartu online aktif/nonaktif, sinkron manual, banner,
  tab Peringkat dari transport palsu, dan pesan galat koneksi.
- `Shell_HasFourTabsAndTwelveTopics`
- `FixtureFlow_EnqueuesCompletion`
- `PendingOutbox_ShowsSyncBanner` — banner antrean tampil dari outbox nyata
- `PreviewShellViewTests.Home_ShowsPreviewBadgeAndTopicStatus` — kartu
  pratinjau dan status topik berlabel "Pratinjau"
- `PreviewShellViewTests.PreviewFlow_DoesNotRecordProgressOrXp` — alur
  pratinjau selesai tanpa outbox, XP, atau penanda selesai
- `AccessibilityTests.HomeAndProfile_TargetsAreAtLeast48Dp`
- `AccessibilityTests.Profile_IdentityCardFitsContentAndScrollIsDraggable`
- `AccessibilityTests.NicknameDialog_FitsCardAndHasCancel`
- `AccessibilityTests.LessonAndModals_TargetsAreAtLeast48Dp`
- 5 tes tangkapan layar (`ScreenshotTests`)
- `LayoutTests.Home_ScrollStartsAtTop` — Beranda kembali ke posisi atas
  setelah dibangun ulang
- `LayoutTests.Lesson_ScrollStartsAtTopOnEveryStep` — pembuka dan materi
  selalu dibuka dari atas
- `LayoutTests.HomeAndLesson_TextFitsInsideItsRect` — tidak ada teks
  multi-baris yang terpotong di Beranda atau materi
- `LayoutTests.VocabularyCards_GrowWithTheirContent` — tinggi kartu kosakata
  mengikuti preferred height isinya
- `LayoutTests.NicknameModal_FitsReferenceViewport` — dialog nama muat di
  acuan 360×640 dan mendapat tinggi dari isinya
- `LessonListShellViewTests` (4 tes) — daftar pelajaran per topik, alur
  1→2→3, badge selesai, dan replay yang tidak menggandakan XP/outbox

PlayMode berjalan di Editor, bukan perangkat. Tes ini tidak membuktikan
TalkBack, keyboard Android, lifecycle, atau rotasi perangkat. Percobaan
pertama dengan D3D12 berhenti karena `d3d12: Unrecoverable GPU device
error` sebelum hasil tes ditulis; percobaan ulang dengan `-force-d3d11`
menghasilkan 19/19 lulus. Log: `Builds/logs/unity-playmode-layout4.log`
(hasil: `Builds/logs/playmode-layout4-results.xml`).

Pembaruan 0.2.0: seluruh 27 tes PlayMode lulus pada 2026-11-06
(`Builds/logs/playmode-results.xml`). Tes pratinjau menurunkan status
pelajaran nyata ke `in_review` hanya di memori, sehingga tidak bergantung
pada status berkas yang kini `validated`.

### 3.3 Validator dan kontras

```text
npm test                        36/36 lulus
node src/validate.mjs ...       PASS, 0 error, 4 warning
node tools/design/contrast.mjs  12/12 pasangan PASS
```

4 warning validator adalah audio contoh yang belum ada; bukan error dan
bukan konten terbit.

### 3.4 Materi Topik 1 dan mode pratinjau

Sumber: `content/elicitation/topic-01-salam/speaker-sheet-v2.csv`
(50 item, `SPK-001`, 2026-11-05, semua `ADA`, semua `REVIEWED`).
`SPK-001` adalah Iyo, pengembang aplikasi, yang mengisi lembar dari
pengetahuan dan pemakaian Bahasa Dawan sehari-hari — bukan salinan kamus
atau web. Pencocokan dengan kamus Tafenpah hanya pengecekan silang, bukan
sumber, dan tidak dicatat sebagai sumber.

Tiga berkas pelajaran sudah dibuat dan lolos validator:

| Berkas | Status | Kosakata | Dialog | Latihan | Contoh |
|---|---|---|---|---|---|
| `content/lessons/lesson-001-salam.json` | `validated` | 12 | 4 baris | 2 MCQ | 2 |
| `content/lessons/lesson-002-salam-bertemu.json` | `validated` | 11 | 4 baris | 2 MCQ | 2 |
| `content/lessons/lesson-003-salam-sopan.json` | `validated` | 10 | 4 baris | 2 MCQ | 2 |

Catatan penting:

- Status `validated` diberikan atas **keputusan pemilik proyek**
  (`marioobe`) pada 2026-11-06. `reviewedBy` berisi
  `"marioobe (pemilik proyek)"`, dan `notes` menyatakan ini override pemilik,
  bukan peninjauan reviewer independen. Topik berikutnya wajib ditinjau
  reviewer independen.
- `audioLicense` berisi pernyataan benar bahwa tidak ada audio pada versi ini;
  `consentRef` tetap `null` karena tidak ada rekaman.
- Sumber terdaftar sebagai `SRC-SPK-001` di `content/references/sources.csv`.
- 9 entri lembar elisitasi identik dengan kata serapan Bahasa Indonesia
  (`ELC-106`, `107`, `112`, `206`, `207`, `208`, `212`, `303`, `306`);
  catatan di konten menyatakan bentuk Dawan aslinya belum dipastikan.
- `ELC-104` dipecah menjadi `selamat pagi Am` dan `selamat pagi Aina`.
- 13 entri memakai apostrof ASCII `'`; normalisasi ortografi `’`/`ʔ`
  menunggu reviewer.
- Item yang perlu klarifikasi reviewer: `ELC-108`, `ELC-115`, `ELC-117`,
  `ELC-211`, `ELC-217`, `ELC-302`.
- Tidak ada audio dan tidak ada `consentRef`; kartu kosakata menampilkan
  "Audio Belum Tersedia".

Mode pratinjau:

- `ContentRepository.AllowInReviewPreview` adalah satu saklar. Bila
  `false`, pelajaran `in_review` tidak dimuat sama sekali.
- `LoadDefault()` membaca materi terbit dari `Resources/Content/lessons`
  dan materi pratinjau dari `Resources/Content/preview`.
- `F1Setup.Prepare()` (menu "Uab Meto/F1 Setup") menyalin **hanya**
  `validated`/`published` dan menghapus JSON pratinjau lama.
- `F1Setup.PreparePreview()` (menu "Uab Meto/F1 Preview Setup") menyalin
  `in_review` ke folder pratinjau untuk demo.
- `BuildScript.BuildAndroid()` selalu memanggil `SyncContent(false)`
  sebelum build, sehingga build rilis tidak bisa membawa materi
  `in_review`.
- `BuildScript.BuildAndroidPreview()` menghasilkan
  `uabmeto-f1-preview.apk` dan tidak boleh disebut build rilis.
- UI memberi label **"PRATINJAU — BELUM DIVALIDASI"** pada kartu, badge
  pelajaran, status topik, dan contoh kalimat. Ringkasan pratinjau tidak
  menampilkan XP dan tidak punya tombol simpan; keluar dari pratinjau tidak
  menulis progres, outbox, atau XP.

Gate pratinjau menolak `draft`, `UNVALIDATED`, placeholder, metadata review
kosong, sumber kosong, dan contoh yang bukan `in_review` tanpa `sourceRef`.

## 4. Tangkapan layar

Tangkapan layar Editor dibuat dengan merender canvas Unity ke `RenderTexture`,
bukan dari mockup Stitch. Salinan bukti ada di
`docs/testing/evidence/f1-editor/` (asli di `Builds/screenshots/`):

| Berkas | Isi |
|---|---|
| `f1-empty-360x640.png` | Beranda, empty state, 360×640 |
| `f1-empty-412x915.png` | Beranda, empty state, 412×915 |
| `f1-empty-915x412.png` | Beranda, landscape, 915×412 |
| `f1-fixture-home-360x640.png` | Kartu fixture pengembangan |
| `f1-fixture-home-915x412.png` | Beranda landscape (kartu di bawah lipatan) |
| `f1-lesson-intro-360x640.png` | Pembuka pelajaran |
| `f1-lesson-material-360x640.png` | Materi/kosakata |
| `f1-lesson-exercise-360x640.png` | Latihan pilihan ganda |
| `f1-lesson-feedback-360x640.png` | Umpan balik benar |
| `f1-lesson-summary-360x640.png` | Ringkasan dan XP tertunda |
| `f1-after-completion-360x640.png` | Banner antrean setelah simpan |
| `f1-lesson-intro-915x412.png` | Pembuka pelajaran landscape |

Seluruh tangkapan Editor di atas diambil ulang pada 2026-11-06 setelah
stabilisasi layout. Tangkapan landscape 915×412 memakai acuan 640×360 dan
masih berupa satu kolom yang dapat digulir, **bukan** tata letak dua kolom
40/60 di `docs/design/screens.md` §12. Kekurangan itu dicatat pada F1-10.

Tangkapan layar perangkat fisik ada di
`docs/testing/evidence/f1-device/` dan dirinci di
`docs/testing/f1-device-evidence.md`. Sepuluh berkas `f1-layout-*-oppo.png`
berasal dari verifikasi layout pratinjau di OPPO `CPH1803`; bukti ini
menunjukkan tampilan demo, **bukan** bukti gate alur pelajaran rilis.

## 5. Uji perangkat fisik

Perangkat: Xiaomi `24117RN76O`, Android 16 (API 36), layar 1080×2400,
densitas 450 dpi, terhubung lewat ADB.

- **D1-01 PASS** — APK `0.1.0` terpasang; `firstInstallTime`
  2026-11-03 13:56:17, `lastUpdateTime` 2026-11-03 14:47:55.
- **D1-02 PASS** — aplikasi dibuka, `UnityPlayerGameActivity` menjadi
  foreground, tidak ada `FATAL EXCEPTION`/`AndroidRuntime`/`E/Unity`
  exception di logcat.
- **D1-07 PASS** — tombol back dari Profil kembali ke Beranda; back dari
  dialog nama menutup dialog dan kembali ke Profil.
- **D1-03 PASS (0.2.1, OPPO `CPH1803`)** — alur tiga pelajaran rilis
  Topik 1 dijalankan penuh di APK rilis: daftar pelajaran, pelajaran 1→2→3,
  pembuka, materi, latihan, umpan balik, ringkasan, dan simpan hasil.
  Replay pelajaran 1 juga diuji; XP tidak bertambah. Portrait.
- **D1-04 NOT RUN** — mode offline belum diuji dengan mock/mode pesawat.
- **D1-05 BLOCKED (perangkat)** — perangkat/launcher memaksa orientasi
  portrait. `wm set-ignore-orientation-request true` + rotasi paksa membuat
  aplikasi di-letterbox menjadi ±885×1080, bukan landscape penuh. Uji
  landscape perangkat tidak sah; bukti landscape hanya dari Editor 915×412.
- **D1-06 NOT RUN / BLOCKED (fitur)** — skala font sistem 1.3×/2.0× belum
  diuji. Audit statis 2026-11-04 menemukan tidak ada kode yang membaca
  `AndroidConfiguration.fontScale`; uGUI/TMP tidak menerapkannya otomatis,
  jadi UI kemungkinan tidak berubah saat skala font sistem dinaikkan.

UI perangkat yang diperiksa: Beranda empty state, 12 topik, scroll,
tab Tutor AI/Peringkat/Profil, kartu identitas, dialog nama, modal topik,
dan tombol back. Tiga bug layout/input ditemukan dan diperbaiki:

1. `ScrollRect` tidak menerima drag karena viewport tidak punya Graphic
   raycast target. Ditambahkan area transparan dan tes PlayMode.
2. Kartu identitas Profil memakai tinggi tetap 170 dp sehingga teks
   bertumpuk dan avatar melebar. Diganti layout dinamis; tes PlayMode
   memeriksa tinggi, avatar, dan tumpang tindih.
3. Kartu modal memakai `ContentSizeFitter` pada panel tanpa layout group
   sehingga isi bertumpuk. Kartu modal sekarang memakai `VerticalLayoutGroup`
   + fitter; tombol `Batal` ditambahkan dan diuji.

### 5.1 Verifikasi layout pratinjau di OPPO (demo)

Perangkat: OPPO `CPH1803`, Android 8.1 (API 27), `arm64-v8a`, layar
720×1520, densitas 320 dpi. APK yang dipasang adalah
`uabmeto-f1-preview.apk` (SHA-256 `491B5F...71FE`), sehingga verifikasi ini
memakai materi `in_review` dan **bukan** bukti gate pelajaran rilis.

Stabilisasi layout 2026-11-06 menambahkan empat perbaikan yang diperiksa di
perangkat ini:

1. Tinggi kartu kosakata, contoh, dialog, opsi, umpan balik, ringkasan,
   banner offline, kartu Beranda/Profil, dan modal dihitung dari isi;
   tidak ada lagi tinggi tetap yang memotong teks.
2. Semua daftar gulir kembali ke posisi atas setelah pindah tab, membuka
   pelajaran, atau berpindah langkah.
3. `DestroyChildren` menonaktifkan objek lama sebelum `Destroy`, dan
   `RebuildLayout` memakai tiga lintasan agar tidak ada layout basi satu
   frame.
4. Acuan kanvas responsif: portrait 360×640, landscape 640×360, keduanya
   dikunci ke lebar.

Layar yang diperiksa dengan tangkapan layar: Beranda (banner, statistik,
kartu pratinjau), modal pratinjau, pembuka pelajaran, materi bagian atas,
dialog bagian bawah, latihan, umpan balik, dan ringkasan. Gulir Beranda ke
bawah lalu pindah ke Profil dan kembali ke Beranda juga diperiksa; daftar
kembali ke atas. Bukti: `docs/testing/evidence/f1-device/f1-layout-*-oppo.png`
(10 berkas).

Garis putih melengkung di tepi kiri bukan bagian aplikasi: garis yang sama
terlihat di home screen sistem (panel tepi OEM). Ini tidak dihitung sebagai
temuan UI aplikasi.

### 5.2 Verifikasi rilis 0.2.0 dan sinkron lokal di OPPO

Tanggal: 2026-11-06. Perangkat: OPPO `CPH1803` (`26b4c7b0`), Android 8.1
(API 27), 720×1520. APK rilis `uabmeto-f1.apk` 0.2.0
(SHA-256 `EA0E0E75...90C6BD`), dipasang dengan `adb install -r` tanpa
uninstall.

| Uji | Hasil |
|---|---|
| `adb install -r` | `Success`; `versionName=0.2.0`, `versionCode=2` |
| Peluncuran | `UnityPlayerGameActivity` foreground, tanpa crash |
| Pelajaran rilis | Beranda menampilkan "PELAJARAN TERVALIDASI" dan "Sapaan waktu" |
| Alur pelajaran | Pembuka → materi → latihan → umpan balik → ringkasan selesai |
| Simpan hasil | Outbox berisi 1 hasil, 10 XP tertunda |
| Mode online | Kartu Profil "Aktif · http://127.0.0.1:8787" (adb reverse) |
| Sinkron manual | "Sinkron selesai: 1 hasil terkirim." |
| Server | `POST /v1/sync 200`; `applied_effects` 10 XP untuk `lesson-001-salam` |
| Peringkat | "Minggu 2026-11-02 · 3 peserta"; rio 10 XP ditandai aksen |

Catatan:

- Koneksi memakai `adb reverse tcp:8787 tcp:8787` karena Wi-Fi HP tidak
  terhubung ke hotspot PC. Ini **bukan** bukti konektivitas Wi-Fi/LAN.
- Banner masih menampilkan "Offline" karena `internetReachability` melihat
  Wi-Fi seluler mati; sinkron tetap berhasil lewat adb reverse. Ini temuan
  UX yang belum diperbaiki: banner offline tidak membedakan "tidak ada
  internet" dari "server lokal tidak terjangkau".
- Bukti: `docs/testing/evidence/f1-device/f1-release-0.2.0-*.png`
  (Beranda, Profil online, sinkron selesai, ringkasan, Peringkat).

### 5.3 Verifikasi rilis 0.2.1: alur tiga pelajaran dan banner (OPPO)

Tanggal: 2026-11-07/08. Perangkat: OPPO `CPH1803` (`26b4c7b0`), Android 8.1
(API 27), 720×1520. APK rilis 0.2.1 (`versionName=0.2.1`, `versionCode=3`,
SHA-256 `8E46C47E...96BC6A3`) dipasang dengan `adb install -r`;
`firstInstallTime` tetap dan tanda tangan `5a7f688` sama dengan 0.1.0/0.2.0.

| Uji | Hasil |
|---|---|
| Daftar pelajaran | Kartu Topik 1 membuka daftar 3 pelajaran; meta `Pelajaran N · ±menit · XP` |
| Alur 1→2→3 | Beranda "Lanjutkan belajar" maju ke pelajaran 2 lalu 3; tiap pelajaran selesai penuh |
| Badge selesai | Pelajaran selesai berbadge `SELESAI` dan tombol "Ulangi latihan" |
| Statistik | Profil: **Selesai 3**, **XP tertunda 20**, **Antrean 2** sebelum sinkron |
| Replay | Pelajaran 1 diulang sampai ringkasan "Latihan ulang selesai"; XP dan Selesai tidak bertambah |
| Server mati | `adb reverse` dilepas → sinkron gagal → banner "Server lokal tidak dapat dihubungi. Hasil tetap tersimpan." |
| Pemulihan | `adb reverse` dipasang lagi → sinkron sukses → XP tertunda 0, Antrean 0 |
| Peringkat | Data server termuat: rio 40 XP, Aina 20 XP, Penguji 10 XP |

Catatan:

- Koneksi tetap memakai `adb reverse tcp:8787 tcp:8787`; ini bukan bukti
  Wi-Fi/LAN dan bukan uji mode pesawat. D1-04 (mock reachability) tetap
  **NOT RUN**.
- Banner "Offline" tidak lagi muncul hanya karena `internetReachability`
  bernilai salah saat server lokal terjangkau; kegagalan server dilaporkan
  dari hasil sinkron.
- Bukti: `docs/testing/evidence/f1-device/f1-release-0.2.1-*.png`
  (Beranda, daftar selesai, pembuka pelajaran 3, ringkasan, ringkasan replay,
  banner gagal sinkron, sinkron sukses, statistik Profil, Peringkat).

### 5.4 Verifikasi Tutor AI rilis 0.3.0 (OPPO)

Tanggal: 2026-11-09/10 (WITA). Perangkat dan koneksi sama; APK 0.3.0 final
(`versionName=0.3.0`, `versionCode=4`, SHA-256 `BA99B129...FE713`) dipasang
dengan `adb install -r`. Worker lokal berjalan dengan `TUTOR_PROVIDER=mock`;
tidak ada kredensial Cloudflare, jadi model Workers AI nyata tidak dipanggil.
Kuota direset di D1 lokal agar rangkaian bukti dimulai dari "Sisa 10 dari 10".

| Uji | Hasil |
|---|---|
| Tab Tutor AI | Judul, penjelasan batas, "Sisa 10 dari 10", 4 chip pertanyaan |
| Keyboard | Ketuk kolom tulis → keyboard muncul; panel tulis dan tombol Kirim bergeser ke atas keyboard dan teks yang diketik terlihat |
| Kirim satu ketuk | Dengan keyboard terbuka, satu ketukan Kirim mengirim pertanyaan, menutup keyboard, dan menampilkan jawaban |
| Pertanyaan dari korpus | "Apa arti kata Noka?" → jawaban pagi, sumber "Sapaan waktu", status "materi tervalidasi" |
| Pertanyaan di luar korpus | "Siapa presiden Indonesia" → "Belum dapat dipastikan…", tanpa sumber |
| Kuota | Setelah dua pertanyaan "Sisa 8 dari 10"; server mencatat `used: 2` |
| Server tidak terjangkau | `adb reverse` dilepas → pesan `danger-text`, tanpa jawaban palsu |
| Pemulihan | `adb reverse` dipasang lagi → jawaban "faen = pulang" dari "Bertemu dan berpisah" |
| Kuota habis | 10 pertanyaan → "Sisa 0 dari 10", status kuota habis, tombol Kirim nonaktif |

Catatan:

- Semua jawaban berasal dari provider `mock`, yaitu potongan chunk
  tervalidasi. Ini bukan bukti kualitas model bahasa.
- Tidak ada audio, ASR, atau skor pelafalan pada versi ini.
- Bukti: `docs/testing/evidence/f1-device/f1-release-0.3.0-tutor-*.png`
  (8 berkas: tab, keyboard, jawaban, refusal, kuota, offline, pemulihan,
  kuota habis).
- Rincian kualitas dan batasan: `docs/testing/tutor-ai-quality.md`.

## 6. Status kriteria F1

| Kriteria | Status | Catatan |
|---|---|---|
| F1-01 token | PASS | `ThemeTokens` satu sumber; UI dibangun dari token |
| F1-02 glif | PASS | 7 tes TMP untuk `ā ē ī ō ū ʼ ’ ʔ` dan varian lain |
| F1-03 empat tab | PASS | PlayMode + screenshot + perangkat |
| F1-04 12 topik | PASS | `TopicPathTests` + 12 baris PlayMode + perangkat |
| F1-05 empty state | PASS | Screenshot Editor + perangkat |
| F1-06 alur pelajaran | PASS (APK rilis 0.2.1) | Daftar pelajaran per topik; 3 pelajaran Topik 1 dibuka berurutan. Replay tidak memberi XP. Satu perangkat, portrait |
| F1-07 outbox | PASS | Outbox + sinkron server lokal teruji di perangkat (tiga hasil, 20 XP tertunda) |
| F1-08 banner | PARTIAL | Antrean nyata dan banner gagal-server teruji di perangkat; D1-04 offline mock/mode pesawat belum |
| F1-09 panel debug | PASS | Tidak ada panel; pemindaian APK 0 |
| F1-10 portrait/landscape | PASS (Editor) | Acuan responsif 360×640 / 640×360; 360×640, 412×915, 915×412 diuji. Landscape masih satu kolom, belum tata letak dua kolom 40/60 §12; landscape perangkat BLOCKED |
| F1-11 pelajaran validated | **PASS (override pemilik)** | 3 berkas `validated`; bukan reviewer independen |
| F1-12 sumber | **PARTIAL** | `SRC-SPK-001` tercatat, tetapi berasal dari elisitasi pengembang sendiri |
| F1-13 reviewer | **BLOCKED** | Override pemilik, bukan reviewer independen |
| F1-14 consent audio | **BLOCKED** | Belum ada audio/consent nyata |
| F1-15 audio sintetis | PASS (tidak ada audio) | Tidak ada audio sintetis |
| F1-16 audio belum tersedia | PASS | Teks "Audio Belum Tersedia" di kartu |
| F1-17 placeholder rilis | PASS | Tidak ada token placeholder di aset konten APK; literal `[PLACEHOLDER` hanya ada di kode pemeriksaan |
| F1-18 nama orang | PASS | Tidak ada nama orang nyata di UI/konten |
| F1-19 kontras C1–C5 | PASS (F1) | C1–C5 ditutup untuk UI F1; 12/12 pasangan lulus; layar belum dibangun N/A |
| F1-20 target T1–T12 | PASS (F1) | T1–T12 ditutup untuk UI F1; 4 tes UI lulus; layar belum dibangun N/A |
| F1-21 font 1.3×/2.0× | **BLOCKED** | Belum diuji; belum ada pembacaan `fontScale` sistem |
| F1-22 status ikon+teks | PASS (F1) | Status topik/audio punya ikon + teks |
| F1-23 build IL2CPP ARM64 | PASS | Log build + `aapt list` |
| F1-24 APK di perangkat | PASS | Terpasang dan dibuka di perangkat fisik |
| F1-25 tanpa warning | PARTIAL | Build 0.3.0 mencatat 3 warning shader TMP; bukan build bersih dari nol |
| F1-26 ukuran APK | PASS | 28.932.254 byte |
| F1-27 kredensial | PASS (pemindaian terbatas) | Pola umum 0; bukan audit menyeluruh |

## 7. Keputusan yang dikunci

- Skema konten 1.1.0 dan gate penerbitan; konten 1.0.0 tidak dapat terbit.
- 12 topik PRD; varian TTU/Kefamenanu.
- UI uGUI + TextMeshPro; Inter/Plus Jakarta Sans/Noto Sans/Material Symbols.
- Dark theme; token tunggal; tanpa shadow/blur/gradien.
- XP klien hanya tampilan tertunda; server tetap otoritatif.
- Fixture hanya di Editor; build rilis hanya memuat pelajaran `validated`.
  Versi 0.2.1 memuat tiga pelajaran Topik 1 yang `validated` atas override
  pemilik.
- Pelajaran boleh diulang untuk latihan, tetapi replay tidak menambah XP
  lokal maupun mengantrekan hasil baru. XP asli tetap dihitung server.
- Materi `in_review` hanya untuk pratinjau berlabel; tidak mencatat progres
  atau XP dan tidak masuk build rilis.
- Hati dari PRD §3.8 **belum** diimplementasikan di F1; kartu "Selesai"
  menggantikan tampilan hati agar tidak menampilkan state palsu.

## 8. Keputusan yang tertunda

- Reviewer independen untuk Topik 1 (saat ini override pemilik) dan semua
  topik berikutnya.
- Izin audio dan rekaman penutur untuk pelajaran pertama.
- Regenerasi/pengurangan hati dan kuota Worker.
- Sinkronisasi Firebase/D1 dan idempotensi lintas sistem nyata.
- Tutor AI teks, lima jenis latihan lain, ujian akhir, auth, dan hapus akun.
  Tutor AI teks sudah ada di 0.3.0 untuk korpus Topik 1; percakapan suara,
  pelafalan/ASR, dan tutor adaptif tetap tertunda.
- Uji model Workers AI nyata setelah kredensial Cloudflare tersedia.
- Audit TalkBack, uji font sistem 1.3×/2.0×, dan uji landscape perangkat.
- Tata letak landscape dua kolom 40/60 sesuai `docs/design/screens.md` §12;
  F1 masih memakai satu kolom yang dapat digulir.
- Build rilis tanpa warning dan signing key rilis.

## 9. Risiko dan tindakan berikutnya

1. **Blocker konten:** mintalah reviewer independen memeriksa
   `speaker-sheet-v2.csv` dan tiga berkas `content/lessons/*.json`. Setelah
   tanda tangan, ubah status ke `validated` dengan `reviewedBy`/`reviewedAt`
   yang benar, lalu jalankan `Uab Meto/F1 Setup` dan build rilis. Jangan
   mengarang teks Dawan atau menaikkan status tanpa bukti.
2. **Gate perangkat:** selesaikan D1-04..D1-06. D1-03 sudah lulus dengan
   tiga pelajaran rilis 0.2.1; D1-05 memerlukan perangkat/launcher yang
   mendukung landscape atau keputusan produk bahwa F1 portrait-only.
3. **Aksesibilitas:** implementasikan pembacaan `AndroidConfiguration.fontScale`
   lalu uji 1.3×/2.0×, audit TalkBack, dan petakan C1–C5/T1–T12 yang tersisa.
4. **Build:** putuskan apakah 3 warning TMP/IL2CPP dapat diterima atau harus
   dihilangkan sebelum F1-25 ditutup.
5. **Offline:** tambahkan mock reachability atau uji mode pesawat untuk
   membuktikan banner offline.
6. **Audio:** rekam penutur dengan izin tertulis, simpan `consentRef`, dan
   isi `audioLicense` sebelum mengaktifkan latihan listening.
