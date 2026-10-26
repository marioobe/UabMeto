# Status Desain

Status: **F0.5 + persiapan remediasi.** Arah visual dan token dapat dipakai.
Tidak ada layar yang final.

## 1. Ringkasan

| Artefak | Status | Boleh jadi acuan? |
|---|---|---|
| `design-tokens.md` | dikunci | ya, untuk warna/tipografi/komponen |
| `visual-direction.md` | dikunci | ya, untuk arah umum |
| `screens.md` | dikunci | ya, untuk inventaris & state |
| `content-binding.md` | kontrak, ada usulan perubahan skema | ya, dengan catatan §3 |
| `accessibility.md` | spesifikasi; **belum lulus** | ya, sebagai target |
| `design-review-stitch.md` | catatan temuan | ya, sebagai daftar kerja |
| `remediation.md` | daftar kerja | ya, sumber status |
| `reference/stitch-2026-10-26/` | referensi | hanya komposisi; teks tidak |

## 2. Status per layar

Tidak ada layar yang boleh disebut final selama temuan P0 pada layar itu
belum ditutup.

| Layar | Temuan P0 | Status |
|---|---|---|
| Masuk | REM-P0-02, REM-P0-03, REM-P0-04 | belum final |
| Daftar | REM-P0-04, REM-P1-04 | belum final |
| Beranda | REM-P0-01, REM-P0-03, REM-P0-04, REM-P0-05 | belum final |
| Pembuka pelajaran | REM-P0-03, REM-P0-05 | belum final |
| Kartu kosakata | REM-P0-03, REM-P0-04, REM-P0-06 | belum final |
| Latihan pilihan ganda | REM-P0-03, REM-P0-05 | belum final |
| Ringkasan pelajaran | REM-P0-03, REM-P0-05 | belum final |
| Tutor AI | REM-P0-02, REM-P0-03, REM-P0-04, REM-P0-05, REM-P0-07 | belum final |
| Peringkat | REM-P0-02, REM-P0-04 | belum final |
| Profil | REM-P0-02, REM-P0-04, REM-P0-05, REM-P1-05 | belum final |

## 3. Aturan penamaan

- Folder `reference/` berarti bahan rujukan, bukan hasil akhir.
- Kata "approved" tidak dipakai lagi untuk gambar layar.
- Yang disetujui hanya: arah visual, token, dan inventaris layar.
- Layar dinyatakan final hanya setelah semua P0-nya berstatus `CLOSED`
  di `remediation.md` dan diverifikasi pada build.
