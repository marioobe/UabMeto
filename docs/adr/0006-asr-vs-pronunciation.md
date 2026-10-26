# ADR-0006: Pemisahan pengenalan ucapan dan penilaian pelafalan

- Status: Diterima
- Tanggal: 2026-10-26

## Konteks

Model pengenal ucapan umum belum tentu memahami Bahasa Dawan varian TTU.
Menggunakan skor model sebagai "nilai pelafalan" berisiko memberi umpan balik
yang salah dan menyesatkan pengguna.

## Keputusan

1. Pengenalan ucapan (transkripsi/kecocokan) dan penilaian kualitas pelafalan
   adalah dua kemampuan berbeda dengan kontrak API berbeda.
2. Uji 20–30 kata hanya **uji kelayakan awal**, bukan bukti akurasi.
3. Selama belum ada dataset penutur lokal tervalidasi, hasil yang tidak
   meyakinkan ditandai `not_assessable` dan ditampilkan sebagai
   **"belum dapat dinilai"**.
4. Tidak ada status lulus/gagal, dan tidak ada pengurangan hati, dari fitur
   pelafalan.
5. Ambang keyakinan adalah konfigurasi yang dapat diubah, bukan klaim
   kebenaran ilmiah.

## Konsekuensi

- Fitur pelafalan bisa diluncurkan sebagai latihan tanpa skor.
- Perlu jalur UI untuk "belum dapat dinilai" dan "coba lagi".
- Evaluasi kualitas memerlukan kerja sama penutur lokal.
- Ujian akhir tidak boleh memakai skor pelafalan yang tidak dapat dipercaya.
