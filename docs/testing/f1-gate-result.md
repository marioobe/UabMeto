# Laporan Gate F1

Tanggal: 2026-11-03 (WITA); diperbarui 2026-11-06 (WITA)
Lingkungan: Windows, Unity `6000.6.0f1`, Android build-tools `36.1.0`.
Status gate: **TIDAK LULUS.** Build, tes UI, dan verifikasi dasar perangkat
fisik lulus; syarat konten tervalidasi, alur pelajaran rilis, aksesibilitas
penuh, dan sisa gate perangkat belum terpenuhi.

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

## 1. Ringkasan keputusan

| Area | Hasil |
|---|---|
| Build Android F1 | **PASS** — `Builds/Android/uabmeto-f1.apk` |
| Build pratinjau | **PASS** — `Builds/Android/uabmeto-f1-preview.apk` (demo, memuat `in_review`) |
| Tes EditMode | **PASS** — 62 lulus, 0 gagal, 0 dilewati |
| Tes PlayMode | **PASS** — 19 lulus, 0 gagal, 0 dilewati |
| Validator konten | **PASS** — 36/36 tes; CLI 0 error, 4 warning audio |
| Kontras F1 | **PASS** — 12/12 pasangan ≥ 4.5:1 |
| Target sentuh F1 | **PASS (UI F1)** — 4/4 tes aksesibilitas/layout |
| Materi Topik 1 | **IN_REVIEW** — 3 pelajaran, 50 item elisitasi `SPK-001`, self-review |
| Konten tervalidasi | **BLOCKED** — belum ada pelajaran `validated` |
| Perangkat fisik | **PARTIAL** — D1-01, D1-02, D1-07 lulus; D1-03..D1-06 belum |
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

### 2.3 Pemeriksaan isi APK

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

Kesimpulan: build rilis **tidak** memuat materi `in_review`, teks Dawan,
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

## 3. Tes yang dijalankan

### 3.1 EditMode

Hasil: **62/62 lulus, 0 gagal, 0 dilewati.** Cakupan:

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

### 3.2 PlayMode

Hasil: **19/19 lulus, 0 gagal, 0 dilewati.**

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

PlayMode berjalan di Editor, bukan perangkat. Tes ini tidak membuktikan
TalkBack, keyboard Android, lifecycle, atau rotasi perangkat. Percobaan
pertama dengan D3D12 berhenti karena `d3d12: Unrecoverable GPU device
error` sebelum hasil tes ditulis; percobaan ulang dengan `-force-d3d11`
menghasilkan 19/19 lulus. Log: `Builds/logs/unity-playmode-layout4.log`
(hasil: `Builds/logs/playmode-layout4-results.xml`).

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
| `content/lessons/lesson-001-salam.json` | `in_review` | 12 | 4 baris | 2 MCQ | 2 |
| `content/lessons/lesson-002-salam-bertemu.json` | `in_review` | 11 | 4 baris | 2 MCQ | 2 |
| `content/lessons/lesson-003-salam-sopan.json` | `in_review` | 10 | 4 baris | 2 MCQ | 2 |

Catatan penting:

- Semua status tetap **`in_review`**. `reviewedBy` berisi
  `"SPK-001 (self-review)"`; ini **bukan** reviewer independen, sehingga
  tidak boleh naik ke `validated`.
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
- **D1-03 BLOCKED** — alur pelajaran rilis tidak dapat dijalankan karena
  tidak ada pelajaran `validated`; fixture sengaja tidak ada di APK rilis.
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

## 6. Status kriteria F1

| Kriteria | Status | Catatan |
|---|---|---|
| F1-01 token | PASS | `ThemeTokens` satu sumber; UI dibangun dari token |
| F1-02 glif | PASS | 7 tes TMP untuk `ā ē ī ō ū ʼ ’ ʔ` dan varian lain |
| F1-03 empat tab | PASS | PlayMode + screenshot + perangkat |
| F1-04 12 topik | PASS | `TopicPathTests` + 12 baris PlayMode + perangkat |
| F1-05 empty state | PASS | Screenshot Editor + perangkat |
| F1-06 alur pelajaran | PASS (Editor fixture + pratinjau) | PlayMode fixture dan pratinjau; tidak ada di APK rilis |
| F1-07 outbox | PASS | `OutboxStore` + PlayMode; belum sinkron server |
| F1-08 banner | PARTIAL | Antrean nyata; banner offline belum diuji mock |
| F1-09 panel debug | PASS | Tidak ada panel; pemindaian APK 0 |
| F1-10 portrait/landscape | PASS (Editor) | Acuan responsif 360×640 / 640×360; 360×640, 412×915, 915×412 diuji. Landscape masih satu kolom, belum tata letak dua kolom 40/60 §12; landscape perangkat BLOCKED |
| F1-11 pelajaran validated | **BLOCKED** | 3 pelajaran `in_review`; belum ada berkas `validated` |
| F1-12 sumber | **PARTIAL** | `SRC-SPK-001` tercatat, tetapi berasal dari self-review |
| F1-13 reviewer | **BLOCKED** | Hanya `SPK-001 (self-review)`; belum ada reviewer independen |
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
| F1-25 tanpa warning | PARTIAL | Build inkremental 2026-11-06 mencatat 1 warning shader TMP; build penuh terakhir masih 3 warning TMP/IL2CPP |
| F1-26 ukuran APK | PASS | 28.294.252 byte |
| F1-27 kredensial | PASS (pemindaian terbatas) | Pola umum 0; bukan audit menyeluruh |

## 7. Keputusan yang dikunci

- Skema konten 1.1.0 dan gate penerbitan; konten 1.0.0 tidak dapat terbit.
- 12 topik PRD; varian TTU/Kefamenanu.
- UI uGUI + TextMeshPro; Inter/Plus Jakarta Sans/Noto Sans/Material Symbols.
- Dark theme; token tunggal; tanpa shadow/blur/gradien.
- XP klien hanya tampilan tertunda; server tetap otoritatif.
- Fixture hanya di Editor; build rilis menampilkan empty state.
- Materi `in_review` hanya untuk pratinjau berlabel; tidak mencatat progres
  atau XP dan tidak masuk build rilis.
- Hati dari PRD §3.8 **belum** diimplementasikan di F1; kartu "Selesai"
  menggantikan tampilan hati agar tidak menampilkan state palsu.

## 8. Keputusan yang tertunda

- Reviewer independen untuk 3 pelajaran Topik 1 yang masih `in_review`.
- Izin audio dan rekaman penutur untuk pelajaran pertama.
- Regenerasi/pengurangan hati dan kuota Worker.
- Sinkronisasi Firebase/D1 dan idempotensi lintas sistem nyata.
- Tutor AI, lima jenis latihan lain, ujian akhir, auth, dan hapus akun.
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
2. **Gate perangkat:** selesaikan D1-03..D1-06. D1-03 memerlukan pelajaran
   rilis; D1-05 memerlukan perangkat/launcher yang mendukung landscape atau
   keputusan produk bahwa F1 portrait-only.
3. **Aksesibilitas:** implementasikan pembacaan `AndroidConfiguration.fontScale`
   lalu uji 1.3×/2.0×, audit TalkBack, dan petakan C1–C5/T1–T12 yang tersisa.
4. **Build:** putuskan apakah 3 warning TMP/IL2CPP dapat diterima atau harus
   dihilangkan sebelum F1-25 ditutup.
5. **Offline:** tambahkan mock reachability atau uji mode pesawat untuk
   membuktikan banner offline.
6. **Audio:** rekam penutur dengan izin tertulis, simpan `consentRef`, dan
   isi `audioLicense` sebelum mengaktifkan latihan listening.
