# ADR-0003: D1 untuk kuota dan proyeksi papan peringkat

- Status: Diterima
- Tanggal: 2026-10-26

## Konteks

Kuota harian membutuhkan increment atomik dan reset berbasis hari WITA.
Papan peringkat membutuhkan pembacaan terurut yang murah. Firestore Spark
memiliki batas operasi harian dan struktur biaya per operasi; memakai Firestore
untuk counter kuota berisiko menghabiskan kuota gratis.

## Keputusan

1. Counter kuota disimpan di Cloudflare D1 dengan kunci
   `(uid, feature, quota_date_wita)`.
2. Increment dilakukan dengan satu statement atomik.
3. Log idempotensi operasi disimpan di D1 (`sync_operations`).
4. Papan peringkat global dan mingguan adalah **proyeksi** di D1.
5. Firestore tetap menjadi sumber kebenaran XP dan progres.

## Konsistensi

- D1 berada di satu region; baca-setelah-tulis konsisten untuk kebutuhan kuota.
- Proyeksi leaderboard dapat tertinggal; waktu tunda yang dapat diterima
  ditetapkan saat F3.
- Rebuild proyeksi harus idempotent dan tidak mengubah XP kanonik.
- Jika D1 hilang, kuota dapat direset dan leaderboard dibangun ulang dari
  Firestore. Tidak ada data akademik yang hilang.

## Konsekuensi

- Dua sistem data perlu dipantau.
- Rekonsiliasi D1↔Firestore wajib (lihat `sync-idempotency.md`).
- Tidak ada jaminan transaksi lintas D1 dan Firestore.
