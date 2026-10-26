# Keamanan & Pengelolaan Rahasia

Status: berlaku sejak F0.

## 1. Prinsip

1. Tidak ada rahasia di repository, APK, dashboard frontend, atau log.
2. Rahasia hanya berada di secret manager layanan.
3. Konfigurasi publik (mis. Firebase API key, Supabase URL) dipisahkan dari
   rahasia server.
4. Hak akses minimum untuk setiap kredensial.

## 2. Inventaris rahasia

| Nama | Pemilik | Disimpan di | Tidak boleh di |
|---|---|---|---|
| `FIREBASE_SERVICE_ACCOUNT_JSON` | Worker | Cloudflare secret | repo, APK, admin frontend |
| `SUPABASE_SERVICE_ROLE_KEY` | Worker | Cloudflare secret | repo, APK, admin frontend |
| `ADMIN_UIDS` | Worker | Cloudflare secret/env | repo publik, klien |
| Keystore rilis Android | Pemilik rilis | Penyimpanan terenkripsi di luar repo | repo, build log |
| Firebase Web API key | Publik | Konfigurasi klien | — (bukan rahasia, tapi rules tetap wajib) |
| `SUPABASE_URL` | Publik | Konfigurasi | — |

## 3. File yang diabaikan Git

`.gitignore` menolak:

- `.env`, `.env.*`, `*.local.*`
- `google-services.json`, `GoogleService-Info.plist`
- `service-account*.json`, `*serviceAccount*.json`
- `*.keystore`, `*.jks`, `keystore.properties`
- `local.properties`

## 4. Praktik pengembangan

- Gunakan `*.example` untuk dokumentasi struktur, dengan nilai placeholder.
- Jangan menempelkan nilai rahasia ke chat, issue, atau commit message.
- Jika sebuah rahasia tidak sengaja masuk commit:
  1. Rotasi rahasia segera.
  2. Hapus dari riwayat Git (butuh persetujuan; ini operasi destruktif).
  3. Catat insiden.
- Jangan memakai akun layanan bersama untuk pengembangan dan produksi.

## 5. Pemeriksaan sebelum commit

```powershell
git diff --cached --name-only
git grep -n -I -E "BEGIN PRIVATE KEY|service_role|AIza[0-9A-Za-z_-]{20,}"
```

Jika ada kecocokan, hentikan commit.

## 6. Build APK

- APK tidak memuat service account, service role key, atau token admin.
- Konfigurasi Firebase di APK bersifat publik; keamanan data bergantung pada
  Firestore rules dan Worker.
- Jangan menaruh `.dev.vars` di folder yang ikut dibangun Unity.
