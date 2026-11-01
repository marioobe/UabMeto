# Laporan Gate F1

Tanggal: 2026-11-01 (WITA)
Lingkungan: Windows, Unity `6000.6.0f1`, Android build-tools `36.1.0`.
Status gate: **TIDAK LULUS.** Build dan tes UI lulus, tetapi syarat konten
tervalidasi, perangkat fisik, dan sebagian aksesibilitas belum terpenuhi.

## 1. Ringkasan keputusan

| Area | Hasil |
|---|---|
| Build Android F1 | **PASS** — `Builds/Android/uabmeto-f1.apk` |
| Tes EditMode | **PASS** — 56 lulus, 0 gagal, 0 dilewati |
| Tes PlayMode | **PASS** — 9 lulus, 0 gagal, 0 dilewati |
| Validator konten | **PASS** — 36/36 tes; CLI 0 error, 4 warning audio |
| Kontras F1 | **PASS** — 12/12 pasangan ≥ 4.5:1 |
| Target sentuh F1 | **PASS (UI F1)** — 2/2 tes, semua `Selectable` aktif ≥ 48 dp |
| Konten tervalidasi | **BLOCKED** — belum ada pelajaran `validated` |
| Perangkat fisik | **BLOCKED** — tidak ada perangkat terhubung |
| Gate F1 | **TIDAK LULUS** |

## 2. Build

- Path: `D:\01_Uab_Meto\Builds\Android\uabmeto-f1.apk`
- Ukuran: **28.272.700 byte**
- Hasil: `Succeeded`, 0 error, 4 warning dari `BuildSummary`
- Durasi build: 553,8 detik
- Paket: `id.uabmeto.app`
- Versi: `0.1.0` (`versionCode 1`)
- `minSdkVersion`: 26
- `targetSdkVersion`: 36
- ABI: `arm64-v8a` saja
- Backend: IL2CPP Release
- Ringkasan mesin: `Builds/Android/build-summary.json`
- Log: `Builds/logs/f1-build.log`

Warning build yang tercatat: pragma shader TMP yang deprecated, argumen
clang yang tidak terpakai, dan catatan IL2CPP bahwa beberapa metode TMP
besar diberi berkas C++ sendiri. Tidak ada error kompilasi. Karena kriteria
F1-25 menyebut "tanpa warning", butir itu **belum** boleh ditutup.

Pemeriksaan APK:

- `aapt dump badging` mengonfirmasi paket, versi, minSdk 26, targetSdk 36,
  dan `native-code: 'arm64-v8a'`.
- Tidak ada `lesson-001-salam`, `PLACEHOLDER-DWN`, atau teks fixture
  `FIXTUR PENGEMBANGAN` di isi APK.
- `topics.json` ada di aset build; tidak ada pelajaran tervalidasi karena
  `Resources/Content/lessons` memang kosong.
- Pemindaian APK untuk pola kredensial (`AIza...`, private key,
  `service_account`, `client_secret`, `SUPABASE_SERVICE_ROLE`, `sk-...`)
  menghasilkan 0 temuan. Ini bukan jaminan keamanan menyeluruh.

## 3. Tes yang dijalankan

### 3.1 EditMode

Perintah:

```powershell
Unity.exe -batchmode -nographics -projectPath app `
  -runTests -testPlatform EditMode `
  -testResults Builds/logs/editmode-topic.xml `
  -logFile Builds/logs/editmode-topic.log
```

Hasil: **56/56 lulus, 0 gagal, 0 dilewati.** Cakupan baru:

- `TopicPathTests` — 12 ID topik persis PRD, tanpa topik ujian, tanpa label
  dialek luar TTU di UI/registry.
- `ContentRepositoryTests` — empty state, validated/published, penolakan
  UNVALIDATED, placeholder, dan contoh kosong.
- `LocalStoreTests` — nickname 3–20, persistensi, XP tertunda idempoten,
  outbox dan rekonsiliasi.
- `PublishGateTests`, `SyncReconcilerTests`, `XpAwardPolicyTests`,
  `FontGlyphTests` tetap lulus.

### 3.2 PlayMode

Perintah:

```powershell
Unity.exe -batchmode -projectPath app `
  -runTests -testPlatform PlayMode `
  -testResults Builds/logs/playmode-a11y.xml `
  -logFile Builds/logs/playmode-a11y.log
```

Hasil: **9/9 lulus, 0 gagal, 0 dilewati.**

- `Shell_HasFourTabsAndTwelveTopics`
- `FixtureFlow_EnqueuesCompletion`
- `AccessibilityTests.HomeAndProfile_TargetsAreAtLeast48Dp`
- `AccessibilityTests.LessonAndModals_TargetsAreAtLeast48Dp`
- 5 tes tangkapan layar (`ScreenshotTests`)

PlayMode berjalan di Editor, bukan perangkat. Tes ini tidak membuktikan
perilaku Android, lifecycle, keyboard, atau TalkBack.

### 3.3 Validator dan kontras

```text
npm test                        36/36 lulus
node src/validate.mjs ...       PASS, 0 error, 4 warning
node tools/design/contrast.mjs  12/12 pasangan PASS
```

4 warning validator adalah audio contoh yang belum ada; bukan error dan
bukan konten terbit.

## 4. Tangkapan layar

Semua dibuat dengan merender canvas Unity ke `RenderTexture`, bukan dari
mockup Stitch dan bukan dari perangkat. Berkas ada di
`Builds/screenshots/`:

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

Tidak ada tangkapan layar mode offline terpisah. Banner antrean sinkron
terlihat, tetapi banner offline belum diuji dengan mock jaringan.

## 5. Status kriteria F1

| Kriteria | Status | Catatan |
|---|---|---|
| F1-01 token | PASS | `ThemeTokens` satu sumber; UI dibangun dari token |
| F1-02 glif | PASS | 7 tes TMP untuk `ā ē ī ō ū ʼ ’ ʔ` dan varian lain |
| F1-03 empat tab | PASS | PlayMode + screenshot |
| F1-04 12 topik | PASS | `TopicPathTests` + 12 baris PlayMode |
| F1-05 empty state | PASS | Screenshot 360×640 dan 412×915 |
| F1-06 alur pelajaran | PASS | PlayMode fixture, lima langkah |
| F1-07 outbox | PASS | `OutboxStore` + PlayMode; belum sinkron server |
| F1-08 banner | PARTIAL | Antrean nyata; banner offline belum diuji mock |
| F1-09 panel debug | PASS | Tidak ada panel; pemindaian APK 0 |
| F1-10 portrait/landscape | PASS (editor) | 360×640, 412×915, 915×412; bukan perangkat |
| F1-11 pelajaran validated | **BLOCKED** | Belum ada berkas `validated` |
| F1-12 sumber | **BLOCKED** | Belum ada sumber yang dapat diperiksa |
| F1-13 reviewer | **BLOCKED** | Belum ada `reviewedBy`/`reviewedAt` manusia |
| F1-14 consent audio | **BLOCKED** | Belum ada audio/consent nyata |
| F1-15 audio sintetis | PASS (tidak ada audio) | Tidak ada audio sintetis |
| F1-16 audio belum tersedia | PASS | Teks "Audio Belum Tersedia" di kartu |
| F1-17 placeholder rilis | PASS | Tidak ada token placeholder konten di APK |
| F1-18 nama orang | PASS | Tidak ada nama orang nyata di UI/konten |
| F1-19 kontras C1–C5 | PARTIAL | 12/12 pasangan F1 lulus; C1–C5 lama belum dipetakan semua |
| F1-20 target T1–T12 | PARTIAL | 2 tes UI F1 lulus; 12 sub-butir layar penuh belum |
| F1-21 font 1.3×/2.0× | NOT RUN | Belum diuji; jangan klaim lulus |
| F1-22 status ikon+teks | PASS (F1) | Status topik/audio punya ikon + teks |
| F1-23 build IL2CPP ARM64 | PASS | Log build |
| F1-24 APK di perangkat | **BLOCKED** | Tidak ada perangkat |
| F1-25 tanpa warning | PARTIAL | 0 error; 4 warning build |
| F1-26 ukuran APK | PASS | 28.272.700 byte |
| F1-27 kredensial | PASS (pemindaian terbatas) | Pola umum 0; bukan audit menyeluruh |

## 6. Keputusan yang dikunci

- Skema konten 1.1.0 dan gate penerbitan; konten 1.0.0 tidak dapat terbit.
- 12 topik PRD; varian TTU/Kefamenanu.
- UI uGUI + TextMeshPro; Inter/Plus Jakarta Sans/Noto Sans/Material Symbols.
- Dark theme; token tunggal; tanpa shadow/blur/gradien.
- XP klien hanya tampilan tertunda; server tetap otoritatif.
- Fixture hanya di Editor; build rilis menampilkan empty state.
- Hati dari PRD §3.8 **belum** diimplementasikan di F1; kartu "Selesai"
  menggantikan tampilan hati agar tidak menampilkan state palsu.

## 7. Keputusan yang tertunda

- Sumber, reviewer manusia, dan izin audio untuk pelajaran pertama.
- Regenerasi/pengurangan hati dan kuota Worker.
- Sinkronisasi Firebase/D1 dan idempotensi lintas sistem nyata.
- Tutor AI, lima jenis latihan lain, ujian akhir, auth, dan hapus akun.
- Audit TalkBack dan uji font sistem 1.3×/2.0×.
- Build rilis tanpa warning dan signing key rilis.

## 8. Risiko dan tindakan berikutnya

1. **Blocker konten:** siapkan satu pelajaran `validated` dengan sumber,
   reviewer, dan (bila ada audio) consent. Jangan mengarang teks Dawan.
2. **Blocker perangkat:** sambungkan perangkat Android fisik, lalu jalankan
   D1-01..D1-07. Emulator tidak menggantikan gate ini.
3. **Aksesibilitas:** ulangi pengukuran pada perangkat, uji font 1.3×/2.0×,
   dan petakan C1–C5/T1–T12 yang tersisa.
4. **Build:** putuskan apakah 4 warning dapat diterima atau harus dihilangkan
   sebelum F1-25 ditutup.
5. **Offline:** tambahkan mock reachability atau uji mode pesawat untuk
   membuktikan banner offline.
