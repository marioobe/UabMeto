# Spesifikasi Audio

Status: baseline F0. Angka final menunggu rekaman nyata dan pengukuran build.

## 1. Format

- Kontainer/codec: **Ogg Vorbis** (`.ogg`).
- Opus **tidak** dipakai sampai kompatibilitas pemutaran native Unity diuji
  pada perangkat target.
- Format lain ditolak validator.

## 2. Parameter awal

| Parameter | Target awal | Catatan |
|---|---|---|
| Channel | Mono | Dialog tetap mono; stereo tidak menambah nilai |
| Sample rate | 44.1 kHz atau 22.05 kHz | Dipilih setelah uji dengar |
| Bitrate | 32–48 kbps | Prioritas kejelasan konsonan |
| Loudness | sekitar -16 LUFS integrated | Konsisten antar klip |
| True peak | ≤ -1.5 dBTP | Hindari distorsi |
| Noise floor | serendah mungkin | Rekam di ruang tenang |

## 3. Penamaan

```
audio/<versi>/<lesson-id>/<item-id>.ogg
```

Contoh:

```
audio/v1/lesson-001/vocab-001.ogg
audio/v1/lesson-001/dlg-001-line-1.ogg
audio/v1/lesson-001/ex-003-prompt.ogg
```

- Huruf kecil, tanpa spasi.
- Path bersifat immutable; revisi membuat versi baru.
- Setiap klip memiliki `durationMs`, `license`, dan `consentRef` di JSON.

## 4. Estimasi ukuran

Perkiraan kasar pada 32 kbps (4 KB/detik):

| Komponen | Jumlah | Durasi rata-rata | Ukuran |
|---|---|---|---|
| Kosakata | 36 pelajaran × 10 | 2 detik | ±2,9 MB |
| Dialog | 36 × 6 baris | 3 detik | ±2,6 MB |
| Prompt latihan | 36 × 8 | 2 detik | ±2,3 MB |
| **Total audio** | | | **±8 MB** |

Angka ini harus diverifikasi dengan file nyata. Target unduhan ≤150 MB dan
instalasi ≤250 MB tetap menjadi acuan.

## 5. Kebutuhan rekaman

Daftar per item ada di `content/audio/recording-needs.csv`. Setiap baris
mencatat:

- item mana yang perlu direkam,
- varian bahasa (TTU/Kefamenanu),
- profil penutur,
- jumlah pengulangan (mis. lambat dan normal),
- status izin.

## 6. Kontrol kualitas

- [ ] Dapat dipahami tanpa teks oleh penutur lain varian TTU.
- [ ] Tidak ada clipping.
- [ ] Loudness konsisten antar klip dalam satu pelajaran.
- [ ] Tidak ada kebocoran data pribadi dalam rekaman.
- [ ] Izin penutur tercatat sebelum publikasi.
- [ ] Ukuran file sesuai target.

## 7. Pengujian ukuran aset

1. Bangun APK dengan audio nyata untuk satu topik.
2. Catat ukuran APK dan ukuran setelah instalasi.
3. Ekstrapolasi ke 12 topik; jika melebihi target, turunkan bitrate atau
   optimalkan durasi.
4. Catat hasil di laporan gate terkait.
