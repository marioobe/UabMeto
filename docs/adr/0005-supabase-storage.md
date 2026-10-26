# ADR-0005: Supabase Storage Free sebagai penyimpanan audio awal

- Status: Diterima (dengan risiko yang diakui)
- Tanggal: 2026-10-26

## Konteks

Firebase Storage memerlukan paket Blaze. Supabase Storage Free menyediakan
1 GB penyimpanan dengan batas bandwidth, cukup untuk audio pelajaran awal.
Proyek gratis dapat dijeda setelah periode tidak aktif.

## Keputusan

1. Supabase Storage dipakai untuk audio terbit dan bundel konten.
2. Bucket audio bersifat publik-baca untuk file terbit; penulisan hanya lewat
   Worker dengan service role key.
3. Nama file bersifat immutable dan berversi (mis. `audio/v3/lesson-001/...`).
4. Audio yang sudah diunduh tetap tersedia lokal saat offline.

## Risiko auto-pause

- **Dampak:** audio baru tidak dapat diunduh; unggahan gagal; manifest baru
  tidak lengkap.
- **Tidak terdampak:** materi dan audio yang sudah ada di perangkat, progres
  lokal, fitur offline.
- **Mitigasi best-effort:** ping berkala dari Worker. Ini **bukan jaminan**
  proyek tetap aktif.
- **Pemulihan:** aktifkan proyek di dashboard Supabase, verifikasi file,
  uji unduhan, lalu lanjutkan publish. Tidak ada data pengguna yang hilang
  karena progres ada di Firestore/D1.

## Konsekuensi

- Perlu pemantauan kapasitas dan bandwidth.
- Mengganti penyedia memerlukan ADR baru dan persetujuan pemilik produk.
- Rahasia service role hanya di Worker, tidak di APK/dashboard.
