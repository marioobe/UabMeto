# Referensi Visual Stitch — 2026-10-26

Status: **referensi, bukan spesifikasi final.**
Arah visual dan design token boleh dipakai sebagai acuan. Layar-layar di
folder ini **belum final** karena masih memuat temuan P0.

Berkas berasal dari paket `stitch_uab_meto_design_system.zip` (diterima
2026-10-26). Paket zip asli tidak disimpan di repositori.

## Boleh dipakai untuk

- komposisi, hierarki, dan jarak antar elemen;
- palet dan tipografi (melalui `../../design-tokens.md`);
- bentuk kartu, chip, tombol, dan bilah progres;
- pola navigasi empat tab.

## Tidak boleh dipakai untuk

- menyalin teks ke kode atau konten;
- menganggap angka, status, atau nama di gambar sebagai data nyata;
- menyatakan layar sudah final;
- menyatakan aksesibilitas sudah lulus.

## Mengapa belum final

Setiap layar memuat setidaknya satu temuan P0: teks Dawan tanpa sumber,
klaim/angka karangan, konfigurasi yang bertentangan, atau panel debug.
Daftar lengkap dan status per layar ada di `../../remediation.md` dan
`../../README.md`.

## Berkas

| Berkas | Layar | Temuan P0 |
|---|---|---|
| `layar_masuk_uab_meto.png` | Masuk | DWN-01, CLM |
| `layar_daftar_uab_meto.png` | Daftar | CLM, REM-P1-04 |
| `beranda_uab_meto.png` | Beranda | DWN-02, CLM-08, CLM-10, REM-P0-01 |
| `pembuka_pelajaran_uab_meto.png` | Pembuka | DWN-03, CLM-08 |
| `kartu_kosakata_uab_meto.png` | Kosakata | DWN-04, CLM-05, REM-P0-06 |
| `latihan_pilihan_ganda_uab_meto.png` | Latihan | DWN-05, CLM-08 |
| `ringkasan_pelajaran_uab_meto.png` | Ringkasan | DWN-06 |
| `tutor_ai_uab_meto.png` | Tutor AI | DWN-07, CLM-01, CLM-07 |
| `peringkat_uab_meto.png` | Peringkat | CLM-09 |
| `profil_uab_meto.png` | Profil | CLM-03, CLM-04, CLM-05, CLM-06 |
| `stitch-design-system.md` | Design system | REM-P0-08 |

Rincian teks Dawan ada di `../../content-inventory.md`.
Rincian klaim ada di `../../content-inventory.md`.

## Catatan

- Logo pada mockup memakai URL pihak ketiga; ganti dengan aset milik
  proyek.
- Layar ujian akhir, lupa kata sandi, splash, dan landscape belum ada.
- `stitch-design-system.md` disimpan untuk jejak asal. Nilainya sudah
  digantikan oleh `../../design-tokens.md`.
