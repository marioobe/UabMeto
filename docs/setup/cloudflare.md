# Setup Cloudflare (Worker, D1, Workers AI)

Status: **panduan saja.** Belum ada akun, database, atau deployment.

## 1. Prasyarat

- Akun Cloudflare.
- Node.js 20+ dan npm (tersedia di mesin pengembangan).
- Wrangler dipasang lokal di proyek (`npm i -D wrangler`), bukan global
  secara wajib.

## 2. Login

```powershell
npx wrangler login
```

Login membuka browser. Token disimpan oleh Wrangler di profil pengguna,
bukan di repository.

## 3. D1

```powershell
npx wrangler d1 create uabmeto
```

Salin `database_id` ke `backend/wrangler.toml` (file konfigurasi, bukan
rahasia). Skema awal (dirancang di F3):

- `quota_counters(uid, feature, quota_date_wita, used, PRIMARY KEY(uid, feature, quota_date_wita))`
- `sync_operations(operation_id PRIMARY KEY, uid, status, payload_hash, created_at, updated_at)`
- `leaderboard_global(uid, nickname, xp, updated_at)`
- `leaderboard_weekly(week_key, uid, nickname, xp, updated_at)`

## 4. Secrets Worker

Disimpan lewat secret manager, bukan file:

```powershell
npx wrangler secret put FIREBASE_SERVICE_ACCOUNT_JSON
npx wrangler secret put ADMIN_UIDS
npx wrangler secret put SUPABASE_SERVICE_ROLE_KEY
```

Untuk pengembangan lokal, gunakan `backend/.dev.vars` (diabaikan Git).
Contoh struktur ada di `backend/.dev.vars.example` — berisi placeholder,
bukan nilai nyata.

## 5. Workers AI

- Aktifkan binding `AI` di `wrangler.toml`.
- Model dipilih saat F4 setelah uji kelayakan.
- Kuota gratis bersama 10.000 Neurons/hari (dapat berubah). Jika habis,
  Worker mengembalikan pesan yang jelas dan fitur offline tetap berjalan.

## 6. Deployment

**Belum diizinkan pada F0.** Deployment hanya dilakukan setelah:

1. Ada persetujuan eksplisit.
2. Secrets sudah dipasang.
3. Ada uji lokal dengan `wrangler dev`.

## 7. Verifikasi

- [ ] `wrangler dev` berjalan lokal.
- [ ] Request tanpa token → 401.
- [ ] Kuota bertambah dan reset pada hari WITA.
- [ ] Tidak ada secret di `wrangler.toml` atau source.
