# Uab Meto

Aplikasi pembelajaran Bahasa Dawan (Uab Meto) varian Timor Tengah Utara (TTU).
Platform: Android, Unity 6 (`6000.6.0f1`).

> **Status: F1 — UI inti dan build APK sudah ada; gate F1 belum lulus.**
> Belum ada integrasi layanan produksi dan belum ada materi Bahasa Dawan
> tervalidasi. Tiga pelajaran Topik 1 sudah berisi materi hasil elisitasi
> `SPK-001` dan berstatus `in_review`; aplikasi hanya menampilkannya sebagai
> pratinjau berlabel tanpa progres/XP. APK sudah dipasang dan diuji dasar pada
> satu perangkat Android fisik portrait; alur pelajaran rilis, landscape
> perangkat, offline, TalkBack, dan skala font belum diuji. Lihat
> `docs/testing/f1-gate-result.md` dan `docs/testing/f1-device-evidence.md`.

## Desain

| Path | Keterangan |
|---|---|
| `docs/design/README.md` | status desain & status per layar |
| `docs/design/design-review-stitch.md` | tinjauan & keputusan atas mockup Stitch |
| `docs/design/remediation.md` | 18 butir remediasi ber-ID |
| `docs/design/design-tokens.md` | token warna, tipografi, komponen |
| `docs/design/screens.md` | inventaris layar, state, landscape |
| `docs/design/topic-scope.md` | 12 topik resmi & topik yang dikeluarkan |
| `docs/design/content-inventory.md` | 8 teks Dawan & 10 klaim yang dilarang |
| `docs/design/content-binding.md` | pemetaan UI ke JSON/Firestore/Worker |
| `docs/design/accessibility.md` | kontras terukur & audit target sentuh |
| `docs/design/reference/` | referensi visual Stitch (bukan final) |
| `docs/testing/f1-gate-criteria.md` | syarat lulus F1 dan gate perangkat |

## Isi repository

| Path | Keterangan |
|---|---|
| `PRD.md` | Product Requirements Document (acuan kebutuhan) |
| `docs/` | Arsitektur, ADR, panduan setup, keamanan, desain, traceability |
| `content/` | Skema, template, contoh `UNVALIDATED`, daftar kebutuhan rekaman |
| `tools/content-validator/` | Validator konten berbasis Node.js |
| `app/` | Proyek Unity (klien Android) |
| `backend/` | Cloudflare Worker — **belum diimplementasikan (F3)** |
| `admin/` | Dashboard admin — **belum diimplementasikan (F6)** |
| `Builds/` | Output build lokal (tidak di-commit) |

## Prasyarat

- Unity `6000.6.0f1` dengan modul Android (SDK/NDK/OpenJDK bawaan Unity)
- Node.js 20+ (validator konten)
- Git

## Menjalankan validator konten

```powershell
cd tools/content-validator
npm test
node src/validate.mjs --root ../../content
```

Validator menolak penerbitan konten yang belum tervalidasi. Lihat
`docs/content/validation-workflow.md`.

## Build APK lokal

```powershell
& "D:\Unity2022\6000.6.0f1\Editor\Unity.exe" `
  -batchmode -nographics -quit `
  -projectPath "D:\01_Uab_Meto\app" `
  -executeMethod UabMeto.Editor.BuildScript.BuildAndroid `
  -logFile "C:\Users\ACER\AppData\Local\Temp\opencode\unity-build.log"
```

Output rilis: `Builds/Android/uabmeto-f1.apk` (diabaikan Git). Build ini
menyalin **hanya** pelajaran `validated`/`published` dan menghapus materi
`in_review` dari `Resources/Content/preview`.

Untuk demo materi `in_review`, buat APK pratinjau terpisah:

```powershell
& "D:\Unity2022\6000.6.0f1\Editor\Unity.exe" `
  -batchmode -nographics -quit `
  -projectPath "D:\01_Uab_Meto\app" `
  -executeMethod UabMeto.Editor.BuildScript.BuildAndroidPreview `
  -logFile "C:\Users\ACER\AppData\Local\Temp\opencode\unity-build-preview.log"
```

Output: `Builds/Android/uabmeto-f1-preview.apk`. **Jangan** disebut build
rilis; materi di dalamnya masih `in_review`.

## Menjalankan EditMode test

```powershell
& "D:\Unity2022\6000.6.0f1\Editor\Unity.exe" `
  -batchmode -nographics `
  -projectPath "D:\01_Uab_Meto\app" `
  -runTests -testPlatform EditMode `
  -testResults "Builds/logs/editmode-results.xml" `
  -logFile "C:\Users\ACER\AppData\Local\Temp\opencode\unity-tests.log"
```

## Mode online lokal (pengembangan)

Backend Worker + D1 lokal untuk menguji alur sinkronisasi tanpa cloud:

```powershell
cd backend
npm install
npm run seed
npm run db:init
npm run dev
```

Kontrak dan batasan: `docs/architecture/local-online-mode.md`.
Ini **bukan** backend produksi: tidak ada Firebase, Firestore, deployment,
atau kredensial nyata. `workers_dev = false` dan jangan di-deploy.

## Aturan keamanan

- Tidak ada API key, service account, token, atau kata sandi di repository,
  source Unity, APK, maupun frontend.
- Konfigurasi rahasia hanya lewat secret manager layanan (mis. `wrangler secret put`).
- Git bersifat **lokal**; tidak ada remote atau push pada tahap ini.
