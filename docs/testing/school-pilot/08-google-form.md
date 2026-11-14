# Struktur Google Form untuk Rekap

Google Form **bukan** alat pengisian utama di sekolah. Kuesioner siswa
dan guru diisi di kertas; Form dipakai tim untuk merekap setelah pulang.
Dengan begitu sesi APK tetap offline dan tidak bergantung pada internet
sekolah.

Buat dua Form dalam satu Google Sheet:

1. **Form A — Rekap Skor Tes Lisan** (diisi tim)
2. **Form B — Rekap Kuesioner Siswa** (diisi tim dari kertas)

Kuesioner guru cukup dimasukkan ke Sheet yang sama; tidak perlu Form
terpisah.

## Pengaturan umum

- Jangan mengumpulkan email.
- Jangan meminta nama siswa; gunakan `P01`/`P02`.
- Batasi akses Form dan Sheet hanya untuk tim pengembang/guru.
- Matikan "Kirim salinan jawaban ke responden".
- Untuk Form B, matikan "Batasi 1 jawaban" karena tim yang mengisi,
  bukan siswa.
- Tautkan Form ke Google Sheet; beri nama `UabMeto-SMP-2026`.

## Form A — Rekap Skor Tes Lisan

Judul: **Rekap Skor Tes Lisan Uab Meto — SMP Putri Kefamenanu**

Deskripsi: "Diisi tim setelah pretes/postes. Gunakan kode peserta,
jangan nama."

| No | Pertanyaan | Jenis | Opsi/nilai |
|---|---|---|---|
| 1 | Kode peserta | Pilihan ganda | `P01`, `P02` |
| 2 | Sesi | Pilihan ganda | `Pretes`, `Postes` |
| 3 | Tanggal | Tanggal | |
| 4 | Ruang | Pilihan ganda | `A`, `B` |
| 5 | Penguji | Isian singkat | inisial, bukan nama lengkap |
| 6 | Skor Bagian A | Isian singkat | angka 0–10 |
| 7 | Skor Bagian B | Isian singkat | angka 0–10 |
| 8 | Total | Isian singkat | angka 0–20 |
| 9 | Catatan | Paragraf | bantuan/kendala |

Jika ingin mencatat butir per butir, tambahkan 20 pertanyaan
"0/1" dengan nama `A1`–`A10`, `B1`–`B10`. Untuk 2 siswa, cara ini
opsional; lembar kertas sudah memuat butir.

## Form B — Rekap Kuesioner Siswa

Judul: **Rekap Kuesioner Siswa Uab Meto — SMP Putri Kefamenanu**

| No | Pertanyaan | Jenis | Opsi |
|---|---|---|---|
| 1 | Kode peserta | Pilihan ganda | `P01`, `P02` |
| 2 | Kelas | Isian singkat | IX |
| 3 | Perangkat | Pilihan ganda | `OPPO`, `HP siswa` |
| 4 | Pernah belajar Dawan? | Pilihan ganda | `Pernah`, `Tidak pernah` |
| 5 | Bahasa Dawan di rumah? | Pilihan ganda | `Sering`, `Kadang`, `Tidak` |
| 6 | Pernah memakai aplikasi? | Pilihan ganda | `Sudah`, `Belum` |
| 7 | Butir 1–15 | Skala linier 1–5 | sesuai `06-kuesioner-siswa.md` |
| 8 | Paling membantu | Paragraf | |
| 9 | Paling sulit | Paragraf | |
| 10 | Saran | Paragraf | |

Untuk butir 1–15, gunakan skala 1–5 dengan label:
1 = sangat tidak setuju, 2 = tidak setuju, 3 = netral, 4 = setuju,
5 = sangat setuju. Salin pernyataan persis dari
`06-kuesioner-siswa.md`.

## Setelah data masuk

- Periksa apakah ada kode ganda atau skor di luar 0–20.
- Jangan menambahkan nama ke Sheet.
- Simpan salinan Sheet di folder tim; hapus akses publik.
- Setelah laporan selesai, arsipkan atau hapus data sesuai kebijakan
  tim/sekolah.

## Catatan

Google Form tidak menggantikan lembar kertas. Jika internet sekolah
tidak stabil, jangan memaksa siswa mengisi Form di HP; isi kertas lebih
aman dan tidak mengganggu waktu.
