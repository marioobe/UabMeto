# ADR-0007: Ogg Vorbis sebagai format audio baseline

- Status: Diterima
- Tanggal: 2026-10-26

## Konteks

Target ukuran unduhan ≤150 MB dan instalasi ≤250 MB. Audio perlu diputar
native di Unity pada perangkat Android low-end. Opus memiliki kompresi lebih
baik, tetapi kompatibilitas pemutaran native Unity belum diuji dalam proyek
ini dan tidak boleh diasumsikan.

## Keputusan

1. Format baseline: **Ogg Vorbis (`.ogg`)** untuk audio offline dan unduhan.
2. Validator menolak path audio selain `.ogg`.
3. Target awal: mono, 32–48 kbps, sample rate 44.1 kHz atau 22.05 kHz
   (dipilih setelah uji dengar).
4. Bitrate dan sample rate final ditetapkan setelah rekaman nyata tersedia
   dan ukuran build diukur.
5. Format lain hanya boleh dipakai setelah ada hasil uji pemutaran di
   perangkat target dan ADR baru.

## Konsekuensi

- Ukuran file sedikit lebih besar daripada Opus pada kualitas setara.
- Perlu pengujian ukuran APK setelah audio nyata masuk.
- Tidak ada transcode otomatis tanpa verifikasi kualitas pelafalan.
