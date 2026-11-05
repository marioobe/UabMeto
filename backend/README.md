# Backend — Cloudflare Worker (mode lokal)

Status: **mode online lokal untuk pengembangan.** Bukan backend produksi.
Tidak ada Firebase, Firestore, deployment, atau kredensial nyata.

Dokumen kontrak dan batasan: `docs/architecture/local-online-mode.md`.

## Isi

- `src/index.js` — Worker: `/health`, `/v1/sync`, `/v1/quota`, `/v1/leaderboard`.
- `schema.sql` — skema D1 lokal.
- `scripts/generate-content-seed.mjs` — menyalin ID/versi/XP/status dari
  `content/lessons/*.json` ke `src/content-seed.js` (tanpa teks Dawan).
- `test/` — tes `node:test` dengan adapter `node:sqlite`.

## Menjalankan

```powershell
cd backend
npm install
npm run seed
npm run db:init
npm run dev
```

Uji:

```powershell
curl http://127.0.0.1:8787/health
npm test
```

## Batasan

- Identitas memakai header `X-Dev-User`, bukan Firebase ID token.
- D1 lokal menggantikan efek Firestore.
- XP hanya untuk konten `validated`; flag `DEV_ALLOW_XP_FOR_IN_REVIEW`
  hanya untuk pengujian lokal.
- `workers_dev = false`; jangan deploy.
- Papan peringkat dihitung dari `applied_effects`, bukan proyeksi global.

## Belum ada

- `wrangler.toml` produksi.
- Firebase Auth / Firestore.
- Endpoint AI.
- Deployment.
