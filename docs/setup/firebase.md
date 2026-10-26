# Setup Firebase (belum dijalankan)

Status: **panduan saja.** Tidak ada proyek Firebase yang dibuat pada F0.
Semua langkah di bawah dilakukan oleh pemilik akun, bukan oleh agen.

## 1. Yang perlu disiapkan

- Akun Google.
- Proyek Firebase baru (paket **Spark**, tanpa billing).
- Aplikasi Android terdaftar dengan package name `id.uabmeto.app`.

> Jika package name berubah, ubah juga
> `ProjectConfigurator.ApplicationIdentifier` di proyek Unity.

## 2. Langkah di Firebase Console

1. Buat proyek Firebase.
2. Tambahkan aplikasi Android:
   - Package name: `id.uabmeto.app`
   - Nickname: `Uab Meto Android`
   - SHA-1 debug (lihat `google-oauth.md`).
3. Aktifkan **Authentication → Sign-in method**:
   - Email/Password
   - Google
4. Aktifkan **Firestore Database** (mode produksi, region terdekat,
   mis. `asia-southeast2`).
5. Jangan mengaktifkan Cloud Functions (tidak tersedia di Spark).
6. Jangan mengaktifkan Firebase Storage (memerlukan Blaze).

## 3. File konfigurasi lokal

- Unduh `google-services.json` dari console.
- Simpan di `app/Assets/google-services.json` (lokasi yang dibaca Firebase
  Unity SDK).
- File ini **diabaikan Git** (lihat `.gitignore`).
- Jangan menyalin isi file ke dokumentasi, chat, atau commit.

Struktur yang diharapkan (nilai contoh, bukan kredensial nyata) ada di
`docs/setup/templates/google-services.json.example`.

## 4. Kuota Spark yang perlu diperhatikan

| Layanan | Batas gratis (indikatif) |
|---|---|
| Firestore reads | 50.000/hari |
| Firestore writes | 20.000/hari |
| Firestore deletes | 20.000/hari |
| Auth | Kuota harian untuk email/verifikasi |

Angka dapat berubah. Rancangan klien harus mengagregasi progres per
pengguna/hari dan menghindari tulis per ketukan.

## 5. Verifikasi

- [ ] Login email berhasil di build Development.
- [ ] Verifikasi email terkirim.
- [ ] Login Google berhasil.
- [ ] Dokumen profil dibuat di Firestore.
- [ ] Klien tidak memiliki izin tulis ke koleksi progres.
- [ ] Tidak ada service account di repository/APK.

## 6. Larangan

- Jangan mengaktifkan billing hanya untuk "mempermudah".
- Jangan menaruh service account di `Assets/`.
- Jangan memakai API key publik sebagai pengganti otorisasi.
