# Autentikasi Worker ke Firestore

Status: **rancangan F0. Tidak ada kredensial dibuat atau disalin.**

## 1. Masalah

Aplikasi Android tidak boleh menulis langsung ke Firestore untuk data yang
berpengaruh (XP, hati, hasil ujian). Jika klien menulis langsung, pengguna
dapat memalsukan progres. Karena itu penulisan dilakukan Worker.

Worker adalah proses server yang tidak memiliki identitas pengguna Firebase
secara default. Ia harus membuktikan identitasnya sendiri ke Google API.

## 2. Model otorisasi

Dua identitas berbeda terlibat:

| Identitas | Dipakai untuk | Cara verifikasi |
|---|---|---|
| Pengguna | Memanggil Worker | Firebase ID token di header `Authorization: Bearer <idToken>` |
| Worker (service account) | Menulis Firestore | OAuth 2.0 access token dari service account |

Alur permintaan:

1. Klien mengirim ID token Firebase.
2. Worker memverifikasi tanda tangan, issuer, audience, dan masa berlaku token.
3. Worker mengambil `uid` dari klaim token.
4. Worker memeriksa otorisasi (pengguna biasa vs admin allowlist).
5. Worker memakai access token service account untuk menulis Firestore.

## 3. Kredensial service account

- Service account dibuat di Google Cloud / Firebase console oleh pemilik proyek.
- File JSON service account **tidak** disimpan di repository, APK, atau
  frontend.
- Nilainya dimasukkan sebagai secret Worker:

```powershell
# Dijalankan oleh pemilik akun, bukan bagian dari F0.
wrangler secret put FIREBASE_SERVICE_ACCOUNT_JSON
```

- Secret berisi JSON service account utuh (atau field terpisah:
  `client_email`, `private_key`, `project_id`).
- `private_key` hanya hidup di memori Worker saat runtime.

## 4. Alur OAuth 2.0 (JWT bearer)

1. Worker menyusun JWT dengan header `alg=RS256`, `kid` dari service account.
2. Klaim: `iss=client_email`, `scope=https://www.googleapis.com/auth/datastore`,
   `aud=https://oauth2.googleapis.com/token`, `iat`, `exp` (maks 1 jam).
3. JWT ditandatangani dengan `private_key`.
4. Worker menukarnya ke `https://oauth2.googleapis.com/token`.
5. Access token di-cache sampai mendekati kedaluwarsa (mis. 55 menit).
6. Setiap request Firestore memakai `Authorization: Bearer <accessToken>`.

Alternatif: memakai library resmi Google Auth di runtime Worker (mis.
`google-auth-library`) agar penandatanganan tidak ditulis manual. Pilihan
library ditetapkan saat F3.

## 5. Prinsip hak akses minimum

- Service account hanya diberi peran yang dibutuhkan (Firestore read/write
  pada project yang sama). Bukan Editor/Owner project.
- Tidak ada akses ke Cloud Storage Firebase, karena audio memakai Supabase.
- Kunci service account dirotasi bila ada indikasi kebocoran.

## 6. Firestore Security Rules

- Klien **tidak** diberi izin tulis ke koleksi progres, XP, atau hasil ujian.
- Klien boleh membaca dokumen miliknya sendiri (jika diperlukan untuk
  sinkronisasi tampilan).
- Semua penulisan dari Worker melewati aturan (Admin SDK melewati rules),
  sehingga validasi wajib dilakukan di kode Worker.
- Rules awal akan diuji di F3 sebelum ada data nyata.

## 7. Yang tidak boleh dilakukan

- Menaruh service account JSON di `Assets/`, `StreamingAssets/`, atau APK.
- Menaruh service account JSON di variabel build Unity.
- Memakai API key publik Firebase sebagai pengganti otorisasi server.
- Memberi klien izin tulis "sementara" untuk mempercepat pengembangan.

## 8. Verifikasi sebelum F3 dinyatakan siap

- [ ] Token pengguna diverifikasi (termasuk `aud` dan `exp`).
- [ ] Permintaan tanpa token ditolak 401.
- [ ] UID yang tidak berhak ditolak 403.
- [ ] Service account tidak muncul di log.
- [ ] Rotasi secret diuji di lingkungan non-produksi.
