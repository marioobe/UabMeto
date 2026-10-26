# Laporan Gate F0

Tanggal: 2026-10-26
Status: **F0 selesai pada tingkat lokal. F1 belum dimulai.**

## 1. Lingkup yang dikerjakan

Git lokal, struktur proyek, proyek Unity, skema & validator konten,
dokumentasi arsitektur/ADR/setup/keamanan, arah visual, matriks traceability,
logika idempotensi, dan build APK lokal.

Tidak dilakukan: pembuatan akun layanan, deployment, unggahan eksternal,
remote Git, integrasi Firebase/Cloudflare/Supabase, materi Dawan tervalidasi.

## 2. File dan folder yang dibuat

### Root

- `.gitignore`
- `.gitattributes`
- `README.md`
- `PRD.md` (tidak diubah)

### `app/` — proyek Unity 6000.6.0f1

Kode:

- `Assets/_Project/Scripts/Core/UabMeto.Core.asmdef`
- `Assets/_Project/Scripts/Core/Content/ValidationStatus.cs`
- `Assets/_Project/Scripts/Core/Content/LessonValidation.cs`
- `Assets/_Project/Scripts/Core/Content/PublishGate.cs`
- `Assets/_Project/Scripts/Core/Sync/SyncOperationState.cs`
- `Assets/_Project/Scripts/Core/Sync/SyncAction.cs`
- `Assets/_Project/Scripts/Core/Sync/SyncOperationRecord.cs`
- `Assets/_Project/Scripts/Core/Sync/SyncReconciler.cs`
- `Assets/_Project/Scripts/App/UabMeto.App.asmdef`
- `Assets/_Project/Scripts/App/BootstrapScreen.cs`
- `Assets/_Project/Editor/UabMeto.Editor.asmdef`
- `Assets/_Project/Editor/ProjectConfigurator.cs`
- `Assets/_Project/Editor/BootstrapSceneBuilder.cs`
- `Assets/_Project/Editor/BuildScript.cs`
- `Assets/_Project/Tests/EditMode/UabMeto.Tests.EditMode.asmdef`
- `Assets/_Project/Tests/EditMode/PublishGateTests.cs`
- `Assets/_Project/Tests/EditMode/SyncReconcilerTests.cs`

Dihasilkan Unity:

- `Assets/_Project/Scenes/Bootstrap.unity`
- `Packages/manifest.json` (ditambah `com.unity.test-framework`, `com.unity.ugui`)
- `ProjectSettings/*`, file `.meta` (otomatis)

### `content/`

- `schema/lesson.schema.json`
- `templates/lesson.template.json`
- `samples/lesson-001.UNVALIDATED.json`
- `references/README.md`
- `references/sources.template.csv`
- `audio/recording-needs.csv`

### `tools/content-validator/`

- `package.json`
- `src/rules.mjs`
- `src/validate.mjs`
- `test/rules.test.mjs`

### `docs/`

- `planning/f0-plan.md`
- `architecture/overview.md`
- `architecture/worker-firestore-auth.md`
- `architecture/sync-idempotency.md`
- `adr/0001-content-as-source-of-truth.md`
- `adr/0002-local-first-server-authoritative.md`
- `adr/0003-d1-quota-leaderboard.md`
- `adr/0004-admin-authorization.md`
- `adr/0005-supabase-storage.md`
- `adr/0006-asr-vs-pronunciation.md`
- `adr/0007-ogg-vorbis-baseline.md`
- `setup/firebase.md`
- `setup/google-oauth.md`
- `setup/cloudflare.md`
- `setup/supabase.md`
- `setup/templates/google-services.json.example`
- `security/secrets.md`
- `content/schema.md`
- `content/validation-workflow.md`
- `content/audio-spec.md`
- `testing/traceability.md`
- `design/visual-direction.md`
- `design/screens.md`
- `design/stitch-prompts.md`
- `testing/f0-gate-report.md` (dokumen ini)

### Placeholder backend/admin

- `backend/README.md`, `backend/.dev.vars.example`
- `admin/README.md`, `admin/.env.example`

### Output build (diabaikan Git)

- `Builds/Android/uabmeto-f0.apk`
- `Builds/Android/build-summary.json`
- `Builds/logs/editmode-results*.xml`

## 3. Hasil build APK

| Item | Nilai |
|---|---|
| Path | `D:\01_Uab_Meto\Builds\Android\uabmeto-f0.apk` |
| Ukuran file | 21.083.872 byte (20,1 MB) |
| Hasil | Succeeded, 0 error, 0 warning |
| Package | `id.uabmeto.app` |
| versionName / versionCode | `0.1.0` / `1` |
| minSdkVersion | 26 |
| targetSdkVersion | 36 |
| ABI | `arm64-v8a` |
| Backend skrip | IL2CPP (Release), ARM64 |
| Durasi build final | 58,1 detik (inkremental) |

Catatan: `build-summary.json` mencatat `reportedTotalSizeBytes` 329.848.277
(nilai internal Unity, termasuk artefak antara) dan `apkSizeBytes`
21.083.872 (ukuran file APK sebenarnya). Yang dipakai sebagai ukuran rilis
adalah `apkSizeBytes`.

### Kegagalan build yang terjadi

1. Build pertama gagal: `Target architecture not specified`, karena
   konfigurasi diterapkan sebelum target platform Android aktif.
2. Build kedua tetap gagal meski log menunjukkan `arch=ARM64`. Setelah
   backend diubah dari Mono ke IL2CPP, build berhasil. Penyebab pasti
   kombinasi Mono+ARM64 tidak diisolasi lebih lanjut; IL2CPP adalah pilihan
   yang juga dibutuhkan untuk rilis.
3. Build sukses pertama menulis APK ke `app/Builds/` karena path relatif.
   Path diperbaiki menjadi `Builds/` di root repo, lalu build dijalankan ulang.

## 4. Hasil pengujian yang benar-benar dijalankan

| Pengujian | Perintah | Hasil |
|---|---|---|
| Validator unit test | `npm test` di `tools/content-validator` | **16/16 lulus** |
| Validator CLI | `node src/validate.mjs --root ../../content` | **PASS**, 0 error, 2 warning |
| Unity EditMode | `-runTests -testPlatform EditMode` | **22/22 lulus** |
| Build APK | `-executeMethod UabMeto.Editor.BuildScript.BuildAndroid` | **Succeeded** |
| Inspeksi APK | `aapt2 dump badging` | package/min/target/ABI sesuai |
| Pengujian perangkat fisik | — | **BELUM DIJALANKAN (tidak ada device)** |

Warning validator (diharapkan, bukan kegagalan):

- `exercises[2].content.audioRef` belum ada.
- `exercises[3].content.targetAudioRef` belum ada.

Keduanya wajar karena belum ada rekaman; status contoh `UNVALIDATED`.

### Yang belum dapat dijalankan

- Pengujian pada perangkat Android (ADB tidak menemukan device).
- Pengujian integrasi Firebase, Cloudflare, Supabase (akun belum ada).
- Pengujian audio offline (belum ada file audio).
- Pengujian AI/ASR (belum ada layanan).

## 5. Keputusan yang dikunci

1. Konten JSON berskema sebagai sumber kebenaran; tidak ada materi di kode.
2. Konten `UNVALIDATED` hanya di `content/samples/` dan wajib placeholder.
3. Penerbitan hanya dari status `validated` + metadata lengkap.
4. Format audio baseline **Ogg Vorbis `.ogg`**; validator menolak format lain.
5. Klien tidak menulis langsung ke Firestore; Worker berwenang.
6. Firestore = sumber kebenaran progres; D1 = kuota, idempotency,
   proyeksi leaderboard.
7. Autentikasi Worker→Firestore memakai service account via secret.
8. Admin memakai allowlist UID di Worker, bukan flag klien/custom claim otomatis.
9. ASR dan penilaian pelafalan dipisah; hasil tidak yakin → `not_assessable`.
10. Build Android memakai IL2CPP + ARM64, minSdk 26, targetSdk 36.
11. Git lokal saja; tanpa remote.

## 6. Keputusan yang masih tertunda

| Topik | Ditunda sampai |
|---|---|
| Model AI akhir | F4 |
| Ambang keyakinan ASR | F4 (setelah uji kelayakan) |
| Regenerasi hati | F2 |
| Batas giliran percakapan | F4 |
| Bitrate/sample rate audio final | setelah rekaman nyata |
| Strategi keystore rilis | sebelum publikasi |
| Stack dashboard admin | F6 |
| Batas toleransi keterlambatan leaderboard | F3 |
| Vector search vs keyword retrieval | setelah korpus > 36 pelajaran |

## 7. Risiko yang ditemukan

| Risiko | Dampak | Tindakan |
|---|---|---|
| Belum ada perangkat uji | APK belum terbukti berjalan di Android | Gate D0 terpisah; siapkan device |
| Korpus & penutur belum ada | F7 terhambat, materi tidak bisa tervalidasi | Mulai pengumpulan sumber & rekaman |
| Kombinasi Mono+ARM64 gagal | Tidak bisa memakai Mono untuk build | Dikunci ke IL2CPP |
| APK 20,1 MB tanpa audio | Target 150 MB belum teruji dengan audio nyata | Ukur ulang saat audio tersedia |
| `reportedTotalSizeBytes` membingungkan | Salah tafsir ukuran | Gunakan `apkSizeBytes`; didokumentasikan |
| Akun layanan belum ada | Integrasi belum bisa dimulai | Panduan setup sudah siap |
| Auto-pause Supabase | Audio baru gagal diunduh | Risiko diterima + prosedur pemulihan |

## 8. Tindakan berikutnya

1. Tinjau laporan ini.
2. Sediakan perangkat Android untuk gate D0.
3. Setujui atau revisi arah visual dan prompt Stitch.
4. Siapkan sumber materi dan penutur lokal.
5. Konfirmasi kesiapan akun Firebase/Cloudflare/Supabase (tanpa menaruh
   kredensial di repo).
6. Setelah disetujui, mulai F1 (vertical slice 3 pelajaran, mock auth
   terpisah dari auth produksi).
