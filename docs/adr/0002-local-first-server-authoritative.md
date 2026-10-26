# ADR-0002: Local-first dengan penulisan berwenang server

- Status: Diterima
- Tanggal: 2026-10-26

## Konteks

PRD mewajibkan materi inti dan progres dapat dipakai offline, tetapi juga
menyatakan data progres tidak boleh dapat diubah bebas oleh klien. Kedua
kebutuhan ini bertabrakan jika klien menulis langsung ke Firestore.

## Keputusan

1. Klien menyimpan progres secara lokal dan mengantrekan operasi di outbox.
2. Klien tidak menulis langsung ke koleksi progres/XP/hasil ujian.
3. Worker memverifikasi Firebase ID token, memvalidasi payload, lalu menulis
   ke Firestore dengan service account.
4. Setiap operasi membawa `operationId` untuk idempotensi.
5. Saat offline, UI menampilkan progres lokal dan status "belum tersinkron".

## Konsekuensi

- Diperlukan backend Worker sejak fitur progres lintas perangkat diuji.
- Ada latensi tambahan saat online; UI harus optimistis namun jujur soal status.
- Klien tetap dapat menyelesaikan latihan inti tanpa Worker.
- Aturan Firestore harus menolak penulisan langsung dari klien.
