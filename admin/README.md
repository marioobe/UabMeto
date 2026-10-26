# Admin Dashboard

Status: **belum diimplementasikan.** Folder ini baru berisi konfigurasi contoh.

Rencana fungsi (F6):

- Login admin (Firebase Auth + allowlist UID di Worker).
- Kelola jalur, pelajaran, kosakata, dialog, audio, soal.
- Alur draft → review → validated → published.
- Unggah audio ke Supabase lewat Worker.
- Laporan progres dan hasil ujian.

Berkas contoh:

- `.env.example` — konfigurasi publik, bukan kredensial.

Larangan:

- Tidak menyimpan service role key atau service account di frontend.
- Tidak menampilkan data pribadi melebihi kebutuhan operasional.

Lihat ADR-0004 dan `docs/architecture/overview.md`.
