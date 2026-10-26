# ADR-0001: Konten sebagai sumber kebenaran

- Status: Diterima
- Tanggal: 2026-10-26

## Konteks

Materi Bahasa Dawan harus dapat diperbarui tanpa membangun ulang APK, dan
materi yang belum divalidasi tidak boleh tampil sebagai fakta. Jika teks
pelajaran ditulis langsung di kode C#, perubahan konten selalu berarti rilis
baru dan risiko materi tidak tervalidasi ikut terkirim.

## Keputusan

1. Seluruh materi direpresentasikan sebagai JSON dengan skema
   `content/schema/lesson.schema.json`.
2. Klien tidak memuat teks pelajaran dari kode; klien membaca bundel konten.
3. Validator (`tools/content-validator`) menjadi gerbang: hanya konten
   `validated` yang dapat diterbitkan.
4. Konten `UNVALIDATED` hanya boleh berada di `content/samples/` dan memakai
   placeholder eksplisit, bukan kata Dawan karangan.

## Konsekuensi

- Pipeline konten harus dijalankan sebelum build.
- Skema perlu versi (`schemaVersion`) agar perubahan tidak memecahkan klien lama.
- Tim konten membutuhkan alur validasi manusia (penutur lokal).
- Ukuran APK dipengaruhi audio, bukan teks.
