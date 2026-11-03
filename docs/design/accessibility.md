# Aksesibilitas UI

Status: **F1 — UI inti sudah diukur, belum lulus penuh.** Angka kontras
diukur dari `ThemeTokens.cs` lewat `tools/design/contrast.mjs`; target sentuh
diukur lewat `AccessibilityTests` PlayMode. Uji perangkat dasar portrait
sudah dilakukan, tetapi ini belum audit TalkBack, belum uji landscape
perangkat, dan belum uji skala font 1.3×/2.0×. Daftar kerja:
`remediation.md` REM-P1-08; bukti perangkat:
`docs/testing/f1-device-evidence.md`.

## 1. Standar yang dipakai

| Aspek | Target |
|---|---|
| Kontras teks normal | ≥ 4.5:1 |
| Kontras teks besar (≥ 18.66 px tebal / ≥ 24 px) | ≥ 3:1 |
| Target sentuh | ≥ 48 × 48 dp |
| Ukuran teks minimum | 14 sp untuk informasi; 12 sp hanya metadata |
| Skala font sistem | diuji pada 1.0×, 1.3×, 2.0× |
| Status | tidak boleh bergantung pada warna saja |

## 2. Kontras

### 2.1 Lulus

| Teks | Latar | Rasio |
|---|---|---|
| `text-primary` | `canvas` | 15.65 |
| `text-primary` | `surface-card` | 13.45 |
| `text-muted` | `canvas` | 8.45 |
| `text-muted` | `surface-card` | 7.27 |
| `accent` | `canvas` | 12.53 |
| `on-accent` | `accent` | 12.53 |
| `success` | `surface-card` | 5.63 |
| `warning` | `surface-card` | 6.89 |
| `danger-text` | `surface-card` | 9.36 |
| `danger-text` | `danger-panel` | 10.15 |
| `text-primary` | `danger-panel` | 14.59 |
| `text-primary` | `success-panel` | 12.75 |
| `outline` | `canvas` | 5.81 |

### 2.2 Gagal dan wajib diperbaiki

| ID | Pemakaian di mockup | Rasio | Perbaikan |
|---|---|---|---|
| REM-P1-08.C1 | `status-danger` `#C6544C` sebagai teks di `surface-card` | 3.61 | ganti `danger-text` `#FFB4AB` |
| REM-P1-08.C2 | `status-danger` `#C6544C` sebagai teks di `canvas` | 4.20 | ganti `danger-text` |
| REM-P1-08.C3 | `status-danger` pada `danger-panel` untuk label 12 sp | 3.92 | ganti `danger-text` |
| REM-P1-08.C4 | `primary` `#FFEFD7` di atas `accent` | 1.31 | ganti `on-accent` |
| REM-P1-08.C5 | `outline` di `surface-high` untuk teks < 14 sp | 4.53 | naikkan ke `text-muted` |

Aturan: `danger-fill` hanya untuk isian, garis, dan ikon. Semua teks bahaya
memakai `danger-text`.

**Lima pasangan gagal pada mockup lama. Di UI F1, teks bahaya memakai
`danger-text`, teks di atas `accent` memakai `on-accent`, dan label kecil
memakai `text-muted`.**

### 2.3 Pengukuran ulang UI F1 (2026-11-01)

`node tools/design/contrast.mjs` membaca token langsung dari
`ThemeTokens.cs` dan menguji 12 pasangan yang dipakai UI F1. Hasil:
**12/12 PASS** pada ambang 4.5:1.

| Teks | Latar | Rasio |
|---|---|---|
| `TextPrimary` | `Canvas` | 15.65 |
| `TextMuted` | `Canvas` | 8.45 |
| `TextMuted` | `SurfaceCard` | 7.27 |
| `TextMuted` | `SurfaceHigh` | 6.58 |
| `Outline` | `SurfaceCard` | 5.00 |
| `Accent` | `Canvas` | 12.53 |
| `OnAccent` | `Accent` | 12.53 |
| `Warning` | `SurfaceCard` | 6.89 |
| `Success` | `Canvas` | 6.55 |
| `Success` | `SurfaceCard` | 5.63 |
| `DangerText` | `DangerPanel` | 10.15 |
| `OnAccent` | `Warning` | 8.02 |

`DangerFill` tidak dipakai sebagai warna teks; pasangan
`DangerFill`/`SurfaceCard` (3.61:1) tetap tidak boleh dipakai untuk teks.

## 3. Audit target sentuh mockup

| ID | Elemen | Ukuran mockup | Status |
|---|---|---|---|
| — | Tombol primer/sekunder | 48 | lulus |
| — | Tile jawaban | 56 | lulus |
| — | Tombol mikrofon | 64 | lulus |
| — | Tombol putar audio | 48 | lulus |
| — | Item bottom nav | 48–64 | lulus |
| — | Checkbox usia/ketentuan (label) | 48 | lulus |
| REM-P1-08.T1 | Tombol tampilkan sandi | 40 | gagal |
| REM-P1-08.T2 | Tombol tutup modal | 32 | gagal |
| REM-P1-08.T3 | Avatar di header | 32 | gagal (bila dapat ditekan) |
| REM-P1-08.T4 | "Bersihkan" data offline | 40 | gagal |
| REM-P1-08.T5 | "Ubah Waktu" | 44 | gagal |
| REM-P1-08.T6 | Chip pertanyaan cepat | 38 | gagal |
| REM-P1-08.T7 | Tombol kecepatan audio 1.0×/0.8× | 26 | gagal |
| REM-P1-08.T8 | Segmented "Audio Tersedia" | 24–28 | gagal |
| REM-P1-08.T9 | Tab peringkat | 40 | gagal |
| REM-P1-08.T10 | "Nanti Saja" | 40 | gagal |
| REM-P1-08.T11 | "Simpan Kata" | 40 | gagal |
| REM-P1-08.T12 | Chip state pratinjau | 30 | gagal (debug saja) |

**Dua belas target gagal pada mockup lama.**

Perbaikan: semua target interaktif minimal 48 dp, termasuk area sentuh yang
lebih besar dari ikon visual. Jarak antar target minimal 8 dp.

### 3.1 Pengukuran ulang UI F1 (2026-11-01)

`AccessibilityTests` (PlayMode) mengukur semua `Selectable` aktif di:

- Beranda,
- Profil,
- pembuka, materi, dan latihan pelajaran,
- dialog nama panggilan.

Hasil: **4/4 tes lulus, 0 target < 48 dp.** Tes tambahan memeriksa kartu
identitas Profil (tinggi, avatar, tumpang tindih), kartu modal nama
(tinggi, urutan tanpa tumpang tindih, tombol Batal/Simpan), dan viewport
scroll yang dapat menerima drag. Uji perangkat portrait menemukan dan
memvalidasi perbaikan tiga bug layout/input ini. Ini belum mencakup layar
yang belum dibangun (auth, ujian, Tutor AI, Peringkat) dan bukan audit
TalkBack. Sub-butir T1–T12 tetap belum semuanya dipetakan ke UI F1.

## 4. Skala font

Mockup memakai tinggi tetap (`h-12`, `h-16`, `h-20`) yang akan memotong teks
saat pengguna memperbesar font.

Aturan implementasi:

1. Gunakan `min-height`, bukan `height`, untuk kontrol berteks.
2. `ContentSizeFitter` + `LayoutGroup` untuk kartu dan chip.
3. Teks panjang memakai `TextMeshPro` dengan `overflow = Ellipsis` pada
   batas kata, bukan tengah token.
4. Uji 1.3× dan 2.0× pada 360 dp; tidak boleh ada teks terpotong atau
   tombol keluar layar.
5. Label tombol tidak boleh turun menjadi dua baris; bila perlu, perpendek
   teks chrome.

**Audit statis 2026-11-04:** tidak ada kode proyek yang membaca
`UnityEngine.Android.AndroidConfiguration.fontScale` atau mengalikan ukuran
TMP dengan skala font sistem. uGUI/TMP tidak menerapkan skala font Android
secara otomatis. Karena itu, sampai fitur ini diimplementasikan dan diuji,
D1-06/F1-21 tidak boleh diklaim lulus; mengubah font sistem kemungkinan
besar tidak mengubah UI aplikasi. Perangkat juga sedang tidak terhubung,
jadi uji perangkat D1-06 tetap **NOT RUN**.

## 5. Ketergantungan warna

| Status | Ikon | Teks |
|---|---|---|
| Benar | centang | "Benar" |
| Salah | silang | "Belum tepat" |
| Terkunci | gembok | "Terkunci" |
| Selesai | centang | "Selesai" |
| Offline | awan coret | "Mode offline" |
| Kuota habis | jam pasir | "Kuota habis" |
| Belum dapat dinilai | gelombang suara coret | "Belum dapat dinilai" |

Jangan pernah menyampaikan status hanya dengan warna.

## 6. Label aksesibilitas

- Setiap tombol ikon wajib punya label teks tersembunyi.
- Ikon dekoratif ditandai sebagai dekoratif agar tidak dibacakan.
- Urutan fokus mengikuti urutan visual.
- Modal memerangkap fokus dan mengembalikannya saat ditutup.
- Pesan galat diumumkan saat muncul, bukan hanya diwarnai.

## 7. Gerak

- Hormati pengaturan "kurangi gerak".
- Tidak ada animasi loop kecuali indikator memuat.
- Tidak ada animasi yang menahan input.
- Transisi maksimal 250 ms.

## 8. Risiko yang diketahui

| Risiko | Dampak | Rencana |
|---|---|---|
| Unity tidak memetakan otomatis ke TalkBack | Pembaca layar terbatas | uji di F1; bila gagal, catat sebagai keterbatasan rilis dan jangan klaim aksesibel |
| Font Dawan belum diuji | Glif hilang | uji cakupan Unicode sebelum prefab |
| Teks Indonesia panjang | Tombol pecah | uji panjang maksimum per kunci chrome |
| Landscape tanpa mockup | Layout rusak | spesifikasi di `screens.md` §12 + uji screenshot |
| Perangkat/launcher portrait-only | Rotasi paksa hanya letterbox; landscape perangkat tidak sah | catat sebagai batasan perangkat; uji landscape di perangkat lain atau putuskan F1 portrait-only |

## 9. Checklist sebelum build F1

- [x] Semua pasangan warna di UI F1 ada di tabel kontras (12/12 PASS).
- [x] Tidak ada `danger-fill` sebagai teks di UI F1.
- [ ] Tidak ada teks di bawah 14 sp kecuali metadata — belum diaudit penuh.
- [x] Semua target sentuh di layar F1 yang diuji ≥ 48 dp (4/4 tes).
- [ ] Uji 1.3× dan 2.0× pada 360 × 640 — belum dijalankan; belum ada
  pembacaan `fontScale` sistem.
- [x] Uji portrait dan landscape di Editor (360×640, 412×915, 915×412).
- [x] APK dipasang dan UI dasar portrait diperiksa di perangkat fisik.
- [ ] Landscape perangkat — terhalang portrait lock OEM.
- [x] Setiap status F1 punya ikon + teks.
- [x] Setiap tombol ikon F1 punya label aksesibilitas.
- [ ] Audit TalkBack — belum dijalankan.
- [ ] Uji keyboard Android dan skala font sistem — belum dijalankan.
