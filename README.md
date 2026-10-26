# Uab Meto

Aplikasi pembelajaran Bahasa Dawan (Uab Meto) varian Timor Tengah Utara (TTU).
Platform: Android, Unity 6 (`6000.6.0f1`).

> **Status: F0.5 — arah desain dikunci.**
> Belum ada integrasi layanan produksi, belum ada materi Bahasa Dawan tervalidasi,
> dan belum ada pengujian pada perangkat fisik. UI Unity F1 belum dibangun.

## Desain

| Path | Keterangan |
|---|---|
| `docs/design/design-review-stitch.md` | tinjauan & keputusan atas mockup Stitch |
| `docs/design/design-tokens.md` | token warna, tipografi, komponen |
| `docs/design/screens.md` | inventaris layar, state, landscape |
| `docs/design/content-binding.md` | pemetaan UI ke JSON/Firestore/Worker |
| `docs/design/accessibility.md` | kontras terukur & audit target sentuh |
| `docs/design/approved/` | referensi visual yang disetujui |

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

Output: `Builds/Android/uabmeto-f0.apk` (diabaikan Git).

## Menjalankan EditMode test

```powershell
& "D:\Unity2022\6000.6.0f1\Editor\Unity.exe" `
  -batchmode -nographics `
  -projectPath "D:\01_Uab_Meto\app" `
  -runTests -testPlatform EditMode `
  -testResults "Builds/logs/editmode-results.xml" `
  -logFile "C:\Users\ACER\AppData\Local\Temp\opencode\unity-tests.log"
```

## Aturan keamanan

- Tidak ada API key, service account, token, atau kata sandi di repository,
  source Unity, APK, maupun frontend.
- Konfigurasi rahasia hanya lewat secret manager layanan (mis. `wrangler secret put`).
- Git bersifat **lokal**; tidak ada remote atau push pada tahap ini.
