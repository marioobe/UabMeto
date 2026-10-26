# ADR-0004: Otorisasi admin lewat allowlist di Worker

- Status: Diterima
- Tanggal: 2026-10-26

## Konteks

PRD hanya mengizinkan satu akun admin utama. Firebase custom claim `admin`
sering diusulkan, tetapi claim tersebut tidak muncul otomatis dan harus
ditetapkan dari lingkungan tepercaya. Mengandalkan flag dari klien berbahaya.

## Keputusan

1. Worker menyimpan allowlist UID admin pada secret/konfigurasi server
   (mis. `ADMIN_UIDS`).
2. Setiap permintaan admin memverifikasi Firebase ID token, lalu memeriksa
   apakah `uid` ada di allowlist.
3. Klien tidak pernah mengirim peran admin; jika dikirim, diabaikan.
4. Custom claim tetap didokumentasikan sebagai alternatif bila jumlah admin
   bertambah, tetapi penetapannya hanya lewat Admin SDK di lingkungan
   tepercaya.

## Konsekuensi

- Menambah admin berarti mengubah konfigurasi Worker, bukan kode klien.
- Tidak ada endpoint publik untuk mendaftarkan admin.
- Dashboard tetap memerlukan login Firebase; allowlist adalah lapisan kedua.
- Perubahan allowlist harus dicatat (audit) dan diuji sebelum rilis.
