# Setup Supabase Storage

Status: **panduan saja.** Belum ada proyek Supabase.

## 1. Prasyarat

- Akun Supabase.
- Proyek baru pada paket **Free**.

## 2. Bucket

- Nama bucket: `audio`.
- Akses baca: publik untuk file yang sudah terbit.
- Akses tulis: hanya lewat Worker memakai service role key.
- Struktur path berversi:

```
audio/v1/lesson-001/vocab-001.ogg
audio/v1/lesson-001/dlg-001-line-1.ogg
```

Path lama tidak ditimpa; perubahan audio membuat path versi baru.

## 3. Batas paket gratis

| Item | Batas indikatif |
|---|---|
| Penyimpanan | 1 GB |
| Bandwidth | Kuota bulanan terbatas |
| Proyek idle | Dapat dijeda setelah periode tidak aktif |

Angka dapat berubah. Pemantauan kapasitas wajib setelah publikasi.

## 4. Risiko auto-pause

- Proyek gratis dapat dijeda bila tidak aktif.
- **Dampak:** audio baru tidak dapat diunduh; unggahan gagal.
- **Tidak terdampak:** audio yang sudah ada di perangkat, progres lokal.
- **Mitigasi:** ping berkala dari Worker — best-effort, **bukan jaminan**.
- **Pemulihan:** aktifkan proyek di dashboard, verifikasi file, uji unduhan,
  lanjutkan publish.

## 5. Kredensial

- `SUPABASE_URL` bukan rahasia.
- `SUPABASE_SERVICE_ROLE_KEY` adalah rahasia → hanya di Worker secret.
- Jangan memakai service role key di aplikasi Android atau dashboard frontend.
- Anon key tidak diperlukan untuk membaca bucket publik.

## 6. Verifikasi

- [ ] Unggah satu file uji lewat Worker (bukan manual dari klien).
- [ ] Unduh file dari URL publik.
- [ ] Hapus file uji setelah verifikasi.
- [ ] Service role key tidak muncul di log.

## 7. Perubahan penyedia

Mengganti penyimpanan mengubah arsitektur yang disepakati. Perlu ADR baru
dan persetujuan pemilik produk.
