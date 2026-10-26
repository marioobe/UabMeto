# Backend — Cloudflare Worker

Status: **belum diimplementasikan.** Folder ini baru berisi konfigurasi contoh.

Rencana tanggung jawab (F3–F4):

- Verifikasi Firebase ID token.
- Kuota harian per akun (D1, reset 00.00 WITA).
- Validasi & sinkronisasi operasi progres ke Firestore.
- Idempotency dan rekonsiliasi (`docs/architecture/sync-idempotency.md`).
- Manifest konten dan URL audio.
- Proxy AI (Workers AI) dengan pembatasan rujukan.

Berkas contoh:

- `.dev.vars.example` — struktur konfigurasi lokal, bukan kredensial.

Belum ada:

- `wrangler.toml` produksi.
- Skema D1.
- Endpoint apa pun.
- Deployment.

Lihat `docs/setup/cloudflare.md` dan `docs/architecture/worker-firestore-auth.md`.
