# Font dan Ikon — Status F1

Status: **aset tersedia, uji glif dasar lulus.** Ini bukan klaim cakupan
seluruh karakter Dawan dan bukan klaim aksesibilitas lulus.

## 1. Font yang dipakai

| Peran | Font | Berkas | Lisensi |
|---|---|---|---|
| Teks UI utama | Inter | `app/Assets/ThirdParty/Fonts/Inter/` | SIL OFL 1.1 |
| Judul | Plus Jakarta Sans | `app/Assets/ThirdParty/Fonts/PlusJakartaSans/` | SIL OFL 1.1 |
| Fallback IPA | Noto Sans | `app/Assets/ThirdParty/Fonts/NotoSans/` | SIL OFL 1.1 |
| Ikon | Material Symbols Outlined | `app/Assets/ThirdParty/Icons/` | Apache 2.0 |

Sumber, tanggal unduh, dan SHA-256 tercatat di `SOURCE.md` masing-masing
folder. Tidak ada permintaan jaringan saat aplikasi berjalan; font ikut
di dalam APK.

## 2. TMP font asset

Dibuat oleh `UabMeto.Editor.FontAssetBuilder` ke
`app/Assets/_Project/Resources/Fonts/`:

- Inter Regular, SemiBold, Bold
- Plus Jakarta Sans Regular, Bold
- Noto Sans Regular
- Material Symbols Outlined

Konfigurasi: sampling 90, padding 9, atlas 1024, `SDFAA`, dynamic,
multi-atlas aktif.

Rantai fallback:

- Plus Jakarta Sans Regular → Inter Regular → Noto Sans
- Plus Jakarta Sans Bold → Inter Bold → Noto Sans
- Inter Regular/SemiBold/Bold → Noto Sans

## 3. Uji glif yang benar-benar dijalankan

Perintah:

```powershell
Unity.exe -batchmode -nographics -projectPath app `
  -runTests -testPlatform EditMode `
  -testResults Builds/logs/editmode-font.xml `
  -logFile Builds/logs/editmode-font.log
```

Hasil terbaru F1 (2026-11-12): **99/99 tes EditMode lulus, 0 gagal, 0 skip**
(`app/Builds/logs/editmode-font.xml`). Tujuh di antaranya adalah
`FontGlyphTests`, enam `UiScaleTests` menguji pembacaan/pembatasan skala font.

| Uji | Hasil |
|---|---|
| Inter sumber memuat `ā` U+0101, `’` U+2019, `ʔ` U+0294 | lulus |
| TMP Inter dapat menambahkan ketiga karakter | lulus |
| PJS langsung tidak memuat `ā`, `’`, `ʼ` U+02BC, `ʔ` | lulus (memang tidak ada) |
| Fallback PJS → Inter menyelesaikan keempat karakter | lulus |
| `TextMeshPro` merender `ā’ʔ` dengan Inter | lulus |
| `TextMeshPro` merender `ʔ` dengan PJS + fallback | lulus |
| Material Symbols memuat codepoint `home` | lulus |

Catatan penting:

- `UnityEngine.Font.HasCharacter` sempat melaporkan `true` untuk PJS pada
  `ʔ`, padahal `fontTools` dan TMP melaporkan tidak ada. Karena itu uji
  yang dipakai adalah TMP `HasCharacter(..., false, false)` dan render
  `TextMeshPro`, bukan `Font.HasCharacter`.
- Uji ini hanya membuktikan karakter yang diwajibkan. Korpus Dawan penuh
  belum diuji karena belum ada konten tervalidasi.
- Aksesibilitas belum diuji dan belum boleh diklaim lulus.

## 4. Aturan pemakaian

- Jangan memuat font dari URL saat runtime.
- Jangan mengganti berkas font tanpa memperbarui `SOURCE.md`, hash, dan
  menjalankan ulang `FontGlyphTests`.
- Teks Dawan memakai Inter; judul memakai PJS dengan fallback.
- Ikon memakai codepoint dari `docs/design/icon-inventory.md`, bukan
  ligatur.
