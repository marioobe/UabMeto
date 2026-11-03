# Laporan Gate F1

Tanggal: 2026-11-03 (WITA)
Lingkungan: Windows, Unity `6000.6.0f1`, Android build-tools `36.1.0`.
Status gate: **TIDAK LULUS.** Build, tes UI, dan verifikasi dasar perangkat
fisik lulus; syarat konten tervalidasi, alur pelajaran rilis, aksesibilitas
penuh, dan sisa gate perangkat belum terpenuhi.

## 1. Ringkasan keputusan

| Area | Hasil |
|---|---|
| Build Android F1 | **PASS** — `Builds/Android/uabmeto-f1.apk` |
| Tes EditMode | **PASS** — 56 lulus, 0 gagal, 0 dilewati |
| Tes PlayMode | **PASS** — 12 lulus, 0 gagal, 0 dilewati |
| Validator konten | **PASS** — 36/36 tes; CLI 0 error, 4 warning audio |
| Kontras F1 | **PASS** — 12/12 pasangan ≥ 4.5:1 |
| Target sentuh F1 | **PASS (UI F1)** — 4/4 tes aksesibilitas/layout |
| Konten tervalidasi | **BLOCKED** — belum ada pelajaran `validated` |
| Perangkat fisik | **PARTIAL** — D1-01, D1-02, D1-07 lulus; D1-03..D1-06 belum |
| Gate F1 | **TIDAK LULUS** |

## 2. Build

- Path: `D:\01_Uab_Meto\Builds\Android\uabmeto-f1.apk`
- Ukuran: **28.278.020 byte**
- SHA-256: `D4416E0250AD0F188A45B142BE6A1764850D4C4BD0D6BF18A0FE3AF97BA3C465`
- Hasil: `Succeeded`, 0 error, 3 warning
- Durasi build: 200,9 detik
- Paket: `id.uabmeto.app`
- Versi: `0.1.0` (`versionCode 1`)
- `minSdkVersion`: 26
- `targetSdkVersion`: 36
- ABI: `arm64-v8a` saja
- Backend: IL2CPP Release
- Ringkasan mesin: `Builds/Android/build-summary.json`
- Log: `Builds/logs/f1-build-modal-fix.log`

Tiga warning build semuanya berasal dari IL2CPP/TMP: `TMP_TextParsingUtilities`
cctor dan dua metode `GenerateTextMesh` diberi berkas C++ sendiri karena besar.
Tidak ada error kompilasi dan tidak ada warning kode proyek. Karena kriteria
F1-25 menyebut "tanpa warning", butir itu **belum** boleh ditutup.

Pemeriksaan APK:

- `aapt dump badging` mengonfirmasi paket, versi, minSdk 26, targetSdk 36,
  dan label `Uab Meto`.
- `aapt list` hanya memuat `lib/arm64-v8a/` (tidak ada ABI lain).
- Tidak ada `lesson-001-salam`, `PLACEHOLDER-DWN`, `UNVALIDATED`, atau teks
  fixture `FIXTUR PENGEMBANGAN` di isi APK.
- `topics.json` ada di aset build; `Resources/Content/lessons` kosong, jadi
  build rilis menampilkan empty state.
- Pemindaian APK untuk pola kredensial (`AIza...`, private key,
  `service_account`, `client_secret`, `SUPABASE_SERVICE_ROLE`, `sk-...`)
  menghasilkan 0 temuan. Ini bukan jaminan keamanan menyeluruh.

## 3. Tes yang dijalankan

### 3.1 EditMode

Hasil: **56/56 lulus, 0 gagal, 0 dilewati.** Cakupan:

- `TopicPathTests` — 12 ID topik persis PRD, tanpa topik ujian, tanpa label
  dialek luar TTU di UI/registry.
- `ContentRepositoryTests` — empty state, validated/published, penolakan
  UNVALIDATED, placeholder, dan contoh kosong.
- `LocalStoreTests` — nickname 3–20, persistensi, XP tertunda idempoten,
  outbox dan rekonsiliasi.
- `PublishGateTests`, `SyncReconcilerTests`, `XpAwardPolicyTests`,
  `FontGlyphTests` tetap lulus.

### 3.2 PlayMode

Hasil: **12/12 lulus, 0 gagal, 0 dilewati.**

- `Shell_HasFourTabsAndTwelveTopics`
- `FixtureFlow_EnqueuesCompletion`
- `PendingOutbox_ShowsSyncBanner` — banner antrean tampil dari outbox nyata
- `AccessibilityTests.HomeAndProfile_TargetsAreAtLeast48Dp`
- `AccessibilityTests.Profile_IdentityCardFitsContentAndScrollIsDraggable`
- `AccessibilityTests.NicknameDialog_FitsCardAndHasCancel`
- `AccessibilityTests.LessonAndModals_TargetsAreAtLeast48Dp`
- 5 tes tangkapan layar (`ScreenshotTests`)

PlayMode berjalan di Editor, bukan perangkat. Tes ini tidak membuktikan
TalkBack, keyboard Android, lifecycle, atau rotasi perangkat. Percobaan
pertama dengan D3D12 berhenti karena `d3d12: Unrecoverable GPU device
error` sebelum hasil tes ditulis; percobaan ulang dengan `-force-d3d11`
menghasilkan 12/12 lulus. Log: `Builds/logs/playmode-banner2.log`.

### 3.3 Validator dan kontras

```text
npm test                        36/36 lulus
node src/validate.mjs ...       PASS, 0 error, 4 warning
node tools/design/contrast.mjs  12/12 pasangan PASS
```

4 warning validator adalah audio contoh yang belum ada; bukan error dan
bukan konten terbit.

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

Tangkapan layar perangkat fisik ada di
`docs/testing/evidence/f1-device/` dan dirinci di
`docs/testing/f1-device-evidence.md`.

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
| F1-06 alur pelajaran | PASS (Editor fixture) | PlayMode fixture; tidak ada di APK rilis |
| F1-07 outbox | PASS | `OutboxStore` + PlayMode; belum sinkron server |
| F1-08 banner | PARTIAL | Antrean nyata; banner offline belum diuji mock |
| F1-09 panel debug | PASS | Tidak ada panel; pemindaian APK 0 |
| F1-10 portrait/landscape | PASS (Editor) | 360×640, 412×915, 915×412; landscape perangkat BLOCKED |
| F1-11 pelajaran validated | **BLOCKED** | Belum ada berkas `validated` |
| F1-12 sumber | **BLOCKED** | Belum ada sumber yang dapat diperiksa |
| F1-13 reviewer | **BLOCKED** | Belum ada `reviewedBy`/`reviewedAt` manusia |
| F1-14 consent audio | **BLOCKED** | Belum ada audio/consent nyata |
| F1-15 audio sintetis | PASS (tidak ada audio) | Tidak ada audio sintetis |
| F1-16 audio belum tersedia | PASS | Teks "Audio Belum Tersedia" di kartu |
| F1-17 placeholder rilis | PASS | Tidak ada token placeholder konten di APK |
| F1-18 nama orang | PASS | Tidak ada nama orang nyata di UI/konten |
| F1-19 kontras C1–C5 | PASS (F1) | C1–C5 ditutup untuk UI F1; 12/12 pasangan lulus; layar belum dibangun N/A |
| F1-20 target T1–T12 | PASS (F1) | T1–T12 ditutup untuk UI F1; 4 tes UI lulus; layar belum dibangun N/A |
| F1-21 font 1.3×/2.0× | **BLOCKED** | Belum diuji; belum ada pembacaan `fontScale` sistem |
| F1-22 status ikon+teks | PASS (F1) | Status topik/audio punya ikon + teks |
| F1-23 build IL2CPP ARM64 | PASS | Log build + `aapt list` |
| F1-24 APK di perangkat | PASS | Terpasang dan dibuka di perangkat fisik |
| F1-25 tanpa warning | PARTIAL | 0 error; 3 warning build TMP/IL2CPP |
| F1-26 ukuran APK | PASS | 28.278.020 byte |
| F1-27 kredensial | PASS (pemindaian terbatas) | Pola umum 0; bukan audit menyeluruh |

## 7. Keputusan yang dikunci

- Skema konten 1.1.0 dan gate penerbitan; konten 1.0.0 tidak dapat terbit.
- 12 topik PRD; varian TTU/Kefamenanu.
- UI uGUI + TextMeshPro; Inter/Plus Jakarta Sans/Noto Sans/Material Symbols.
- Dark theme; token tunggal; tanpa shadow/blur/gradien.
- XP klien hanya tampilan tertunda; server tetap otoritatif.
- Fixture hanya di Editor; build rilis menampilkan empty state.
- Hati dari PRD §3.8 **belum** diimplementasikan di F1; kartu "Selesai"
  menggantikan tampilan hati agar tidak menampilkan state palsu.

## 8. Keputusan yang tertunda

- Sumber, reviewer manusia, dan izin audio untuk pelajaran pertama.
- Regenerasi/pengurangan hati dan kuota Worker.
- Sinkronisasi Firebase/D1 dan idempotensi lintas sistem nyata.
- Tutor AI, lima jenis latihan lain, ujian akhir, auth, dan hapus akun.
- Audit TalkBack, uji font sistem 1.3×/2.0×, dan uji landscape perangkat.
- Build rilis tanpa warning dan signing key rilis.

## 9. Risiko dan tindakan berikutnya

1. **Blocker konten:** siapkan satu pelajaran `validated` dengan sumber,
   reviewer, dan (bila ada audio) consent. Jangan mengarang teks Dawan.
2. **Gate perangkat:** selesaikan D1-03..D1-06. D1-03 memerlukan pelajaran
   rilis; D1-05 memerlukan perangkat/launcher yang mendukung landscape atau
   keputusan produk bahwa F1 portrait-only.
3. **Aksesibilitas:** implementasikan pembacaan `AndroidConfiguration.fontScale`
   lalu uji 1.3×/2.0×, audit TalkBack, dan petakan C1–C5/T1–T12 yang tersisa.
4. **Build:** putuskan apakah 3 warning TMP/IL2CPP dapat diterima atau harus
   dihilangkan sebelum F1-25 ditutup.
5. **Offline:** tambahkan mock reachability atau uji mode pesawat untuk
   membuktikan banner offline.
