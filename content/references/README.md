# Referensi & Izin Materi

Folder ini menyimpan catatan sumber untuk materi yang akan divalidasi.

## Aturan

1. Setiap pelajaran yang berstatus `validated` wajib merujuk minimal satu
   entri di sini melalui `validation.sources[].id`.
2. Sumber harus dapat diperiksa ulang: buku, artikel, wawancara, atau
   dokumentasi komunitas.
3. Jangan menyalin materi berhak cipta tanpa izin tertulis.
4. Untuk rekaman, simpan bukti izin penutur (bukan rekamannya) di luar
   repository jika berisi data pribadi; catat hanya referensi ID-nya.
5. Jangan mengarang sitasi.

## Format sitasi

Gunakan `content/references/sources.template.csv` sebagai awal. Kolom:

| Kolom | Isi |
|---|---|
| `id` | `SRC-0001`, unik |
| `citation` | Sitasi lengkap |
| `url` | Tautan bila ada |
| `license` | Lisensi/izin penggunaan |
| `accessed` | Tanggal akses (ISO) |
| `notes` | Catatan, termasuk batasan penggunaan |

## Yang tidak boleh disimpan di sini

- Data pribadi penutur (nomor telepon, alamat).
- File audio mentah berukuran besar.
- Kredensial atau tautan unduhan privat.
