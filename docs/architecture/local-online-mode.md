# Mode Online Lokal

Status: **alat pengembangan. Bukan backend produksi.**
Tanggal: 2026-11-06.

Dokumen ini menjelaskan Worker + D1 lokal untuk menguji alur sinkronisasi
tanpa Firebase, Firestore, Cloudflare deployment, atau kredensial nyata.
Arsitektur produksi tetap mengikuti `docs/architecture/overview.md`.

## 1. Tujuan

- Menguji outbox klien → HTTP → server → status terkonfirmasi.
- Menguji idempotensi `operationId` dan konflik payload.
- Menguji XP yang dihitung server, bukan klaim klien.
- Menguji kuota harian dan papan peringkat lokal.
- Menyediakan kontrak API yang sama bentuknya dengan rencana produksi.

Yang **tidak** dibuktikan: autentikasi Firebase, Firestore, deployment,
Supabase, dan AI.

## 2. Arsitektur lokal

```text
Unity Editor / dev build
        │ HTTP http://127.0.0.1:8787
        ▼
Cloudflare Worker (wrangler dev --local)
        │
        ▼
D1 lokal (.wrangler/state)
```

D1 lokal dipakai sebagai pengganti Firestore untuk efek progres. Ini
penyederhanaan pengembangan, bukan pemisahan sumber kebenaran produksi.

## 3. Tabel

| Tabel | Fungsi |
|---|---|
| `content_versions` | XP resmi per `lesson_id` + versi + status |
| `operations` | Catatan idempotensi per `operation_id` |
| `applied_effects` | Efek progres yang sudah diterapkan |
| `quota` | Kuota harian per user, reset 00.00 WITA |

`content_versions` diisi dari `content/lessons/*.json` lewat
`npm run seed`. Hanya ID, versi, XP, dan status yang disalin; tidak ada teks
Dawan di Worker.

Papan peringkat **dihitung saat dibaca** dari `applied_effects`, bukan
ditulis ke tabel proyeksi terpisah. Tujuannya agar tidak ada penulisan kedua
yang bisa menggandakan XP. Proyeksi D1 terpisah menyusul di tahap produksi.

## 4. Endpoint

Semua endpoint memerlukan header `X-Dev-User` (3–64 karakter
`A-Z a-z 0-9 . _ -`). Ini **bukan autentikasi**; hanya identitas pengembangan.

| Method | Path | Keterangan |
|---|---|---|
| `GET` | `/health` | Status Worker |
| `POST` | `/v1/sync` | Kirim satu operasi |
| `GET` | `/v1/sync/{operationId}` | Status operasi |
| `GET` | `/v1/quota` | Kuota hari ini (WITA) |
| `GET` | `/v1/leaderboard` | Papan peringkat mingguan |

### 4.1 `POST /v1/sync`

```json
{
  "operationId": "0123456789abcdef0123456789abcdef",
  "lessonId": "lesson-001-salam",
  "contentVersion": "1.1.0",
  "clientClaimedXp": 10,
  "nickname": "Penguji"
}
```

`clientClaimedXp` hanya dicatat untuk audit. XP yang diberikan berasal dari
`content_versions.xp`.

Respons sukses:

```json
{
  "state": "confirmed",
  "awardedXp": 10,
  "duplicate": false,
  "devMode": false
}
```

### 4.2 Aturan idempotensi

1. `operationId` baru dicatat sebagai `reserved`.
2. Efek ditulis ke `applied_effects` dengan `ON CONFLICT DO NOTHING`.
3. Operasi diubah menjadi `confirmed`.
4. Permintaan ulang dengan payload sama mengembalikan hasil tersimpan
   (`duplicate: true`) tanpa menambah XP.
5. Permintaan ulang dengan `operationId` sama tetapi payload berbeda
   menghasilkan `409 operation_conflict`.

### 4.3 Aturan XP

- Hanya konten berstatus `validated` yang boleh memberi XP.
- Konten `in_review` ditolak `422 content_not_validated`.
- Tiga pelajaran Topik 1 kini `validated` (keputusan pemilik, 2026-11-06), jadi
  XP dapat diuji tanpa flag dev.
- Untuk menguji pelajaran `in_review` lain, set
  `DEV_ALLOW_XP_FOR_IN_REVIEW="true"` di `backend/.dev.vars` (diabaikan Git).
  Respons akan menandai `devMode: true`.
- Flag ini tidak boleh dipakai di deployment.

### 4.4 Kuota

`GET /v1/quota` mengembalikan hari WITA (`YYYY-MM-DD`) dan tiga counter:
tutor (10), percakapan (3), pelafalan (10). Endpoint konsumsi belum ada
karena fitur AI belum diimplementasikan.

### 4.5 Leaderboard

`GET /v1/leaderboard?week=YYYY-MM-DD` menghitung total XP dari
`applied_effects` pada rentang Senin 00.00 WITA sampai Senin berikutnya.
Tanpa parameter, dipakai minggu berjalan. Maksimum 50 baris.

## 5. Menjalankan

```powershell
cd backend
npm install
npm run seed
npm run db:init
npm run dev
```

Uji cepat:

```powershell
curl http://127.0.0.1:8787/health
```

Untuk mengizinkan XP `in_review` di mesin lokal:

```powershell
Copy-Item .dev.vars.example .dev.vars
# pastikan DEV_ALLOW_XP_FOR_IN_REVIEW="true"
```

`.dev.vars` dan `.wrangler/` diabaikan Git.

## 6. Tes

```powershell
cd backend
npm test
```

Tes memakai `node:test` + `node:sqlite` sebagai adapter D1 dalam memori.
Cakupan: health, unauthorized, sync pertama, duplikat, konflik, konten tidak
dikenal, konten `in_review`, flag dev, status operasi, kuota, dan leaderboard.

Hasil: **11/11 lulus** pada 2026-11-06.

### 6.1 Bukti verifikasi manual

Dijalankan terhadap `wrangler dev` lokal pada 2026-11-06, sebelum Topik 1
divalidasi, dengan `DEV_ALLOW_XP_FOR_IN_REVIEW="true"`.

| Uji | Hasil |
|---|---|
| `GET /health` | `200` `{"ok":true,"mode":"local"}` |
| Sync pertama `lesson-001-salam` | `200` `{"state":"confirmed","awardedXp":10,"duplicate":false,"devMode":true}` |
| Ulang dengan payload sama | `200` `{"duplicate":true,"awardedXp":10}` |
| Ulang dengan payload berbeda | `409 operation_conflict` |
| `GET /v1/sync/{operationId}` | `200` `state: confirmed`, `awardedXp: 10` |
| `GET /v1/quota` | `200` `dayWita: 2026-11-06`, batas 10/3/10 |
| `GET /v1/leaderboard` | `200` `weekKey: 2026-11-02`, urut `dev-user-2` 20 XP lalu `dev-user` 10 XP |

Setelah duplikat, `applied_effects` hanya berisi tiga baris (satu per operasi
unik); tidak ada baris kedua untuk `operationId` yang sama. Ini bukti XP tidak
dihitung ganda pada tingkat penyimpanan.

Setelah Topik 1 divalidasi, sync `lesson-001-salam` berhasil tanpa flag dev
(`devMode: false`). Bukti perangkat ada di 6.2.

### 6.2 Bukti perangkat (rilis 0.2.0)

OPPO `CPH1803`, Android 8.1 (API 27), APK `0.2.0`, koneksi
`adb reverse tcp:8787 tcp:8787` (bukan Wi-Fi/LAN), 2026-11-06.

| Uji | Hasil |
|---|---|
| `POST /v1/sync` dari aplikasi | `200`, `awardedXp: 10`, `devMode: false` |
| `applied_effects` | `operation=2287b76e...`, user `dev-4febf867...`, `lesson-001-salam`, 10 XP |
| Kartu Profil | "Sinkron selesai: 1 hasil terkirim." |
| `GET /v1/leaderboard` dari aplikasi | 3 peserta; rio 10 XP ditandai aksen |

Banner aplikasi masih menampilkan "Offline" karena `internetReachability`
melihat Wi-Fi seluler mati; ini tidak menghalangi sinkron lewat adb reverse.
Rincian dan tangkapan layar: `docs/testing/f1-device-evidence.md` §3.2.

## 7. Batasan yang diakui

- Tidak ada verifikasi token; `X-Dev-User` bisa dipalsukan.
- Tidak ada Firestore; D1 lokal adalah pengganti efek.
- Tidak ada deployment; `workers_dev = false`.
- Tidak ada rate limit, audit log produksi, atau enkripsi tambahan.
- Leaderboard lokal bukan papan peringkat global.
- XP untuk konten `in_review` hanya lewat flag lokal dan ditandai `devMode`.

## 8. Klien Unity

Klien menyiapkan transport, API, sinkronisasi, dan UI status. Tidak ada
permintaan jaringan otomatis saat aplikasi dibuka.

| Berkas | Fungsi |
|---|---|
| `Core/Sync/IHttpTransport.cs` | Kontrak transport; dapat diganti transport palsu |
| `Core/Sync/HttpResult.cs` | Status, body, dan error transport |
| `App/Net/UnityHttpTransport.cs` | Implementasi `UnityWebRequest`, batas waktu 10 detik |
| `App/Net/ApiClient.cs` | `/v1/sync`, `/v1/sync/{id}`, `/v1/quota`, `/v1/leaderboard` |
| `App/Net/SyncService.cs` | Kirim outbox, simpan `awardedXp` server, atur status |
| `App/Net/OnlineIdentity.cs` | `X-Dev-User` lokal, bukan autentikasi |
| `App/Net/OnlineSettings.cs` | `online.json`, nonaktif secara bawaan |
| `App/UI/LeaderboardView.cs` | Tab Peringkat; data hanya dimuat saat diminta |
| `App/UI/ProfileView.cs` | Kartu "Mode online lokal" dan tombol sinkron |
| `App/UI/ShellView.cs` | Banner status dan aksi sinkron manual |

Aturan klien:

- `clientClaimedXp` hanya dikirim untuk audit; XP yang disimpan berasal dari
  `awardedXp` respons server.
- `content_not_validated` dan `unknown_content` menandai operasi `Rejected`
  dan tidak dikirim ulang.
- `operation_conflict` menandai `Failed` (butuh rekonsiliasi).
- Kegagalan transport menghentikan putaran dan menambah `attemptCount`;
  setelah `maxAttempts` operasi menjadi `Failed`.
- `unauthorized` menghentikan putaran tanpa mengubah status.

Mengaktifkan di perangkat/Editor (opsional, untuk pengujian):

```json
// <persistentDataPath>/uabmeto/online.json
{ "enabled": true, "baseUrl": "http://127.0.0.1:8787" }
```

Di perangkat Android, `127.0.0.1` menunjuk ke perangkat itu sendiri. Untuk
pengujian lintas perangkat, isi `baseUrl` dengan alamat LAN komputer yang
menjalankan `wrangler dev`. Ini tetap bukan mode produksi.

`AppRoot` hanya membuat `ApiClient`/`SyncService` bila `enabled` bernilai
`true`; tidak ada sinkronisasi otomatis saat aplikasi dibuka.

Perilaku UI:

- Kartu Profil menampilkan "Nonaktif" bila mode online belum diaktifkan.
  Tidak ada tombol sinkron dan tidak ada klaim akun server.
- Bila aktif, kartu menampilkan alamat server, identitas pengembangan,
  jumlah hasil menunggu, dan tombol "Sinkronkan sekarang".
- Banner menampilkan hasil menunggu, keadaan offline, dan kegagalan sinkron.
  Banner tidak menampilkan pesan sukses permanen; hasil sukses tampil di
  kartu Profil.
- Tab Peringkat tidak menampilkan data rekaan. Tanpa mode online, tab hanya
  menjelaskan bahwa data tidak tersedia. Dengan mode online, data dimuat
  hanya setelah pengguna menekan "Muat peringkat".
- Baris peringkat pengguna saat ini ditandai warna aksen, bukan warna teks
  bahaya.

Tes EditMode: **81/81 lulus** (19 tes untuk `ApiClient` dan `SyncService`),
PlayMode **27/27 lulus** (8 tes untuk kartu online, banner, dan Peringkat),
2026-11-06.

APK rilis dan bukti gate F1 **belum** dibangun ulang setelah UI online
ditambahkan. Gate F1 tetap **TIDAK LULUS**; jangan memakai dokumen ini sebagai
bukti perangkat.

## 9. Langkah berikutnya

1. Setelah ada pelajaran `validated`, uji XP tanpa flag dev.
2. Bangun ulang APK dan ulangi bukti perangkat bila UI ini akan dirilis.
3. Untuk produksi: Firebase Auth + Firestore + Worker terverifikasi.
