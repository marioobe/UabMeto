# Aksesibilitas UI

Status: **F0.5 — spesifikasi, belum diuji dan belum lulus.** Angka kontras
dihitung dari token `design-tokens.md`. Tidak ada klaim kelulusan
aksesibilitas sampai koreksi dan pengujian dilakukan. Daftar kerja:
`remediation.md` REM-P1-08.

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

**Lima pasangan gagal. Belum ada yang diperbaiki; status `OPEN`.**

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

**Dua belas target gagal. Belum ada yang diperbaiki; status `OPEN`.**

Perbaikan: semua target interaktif minimal 48 dp, termasuk area sentuh yang
lebih besar dari ikon visual. Jarak antar target minimal 8 dp.

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

## 9. Checklist sebelum build F1

- [ ] Semua pasangan warna di UI ada di tabel kontras.
- [ ] Tidak ada `danger-fill` sebagai teks.
- [ ] Tidak ada teks di bawah 14 sp kecuali metadata.
- [ ] Semua target sentuh ≥ 48 dp.
- [ ] Uji 1.3× dan 2.0× pada 360 × 640.
- [ ] Uji portrait dan landscape.
- [ ] Setiap status punya ikon + teks.
- [ ] Setiap tombol ikon punya label.
