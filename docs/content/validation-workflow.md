# Alur Validasi Materi

Tujuan: memastikan tidak ada materi Bahasa Dawan yang tampil sebagai fakta
sebelum diperiksa penutur lokal yang memahami varian TTU.

## 1. Peran

| Peran | Tanggung jawab |
|---|---|
| Penyusun | Menyiapkan struktur pelajaran, sumber, dan draf |
| Peninjau bahasa | Memeriksa kata, makna, ejaan, dan kelayakan budaya |
| Peninjau audio | Memastikan rekaman sesuai dan izinnya jelas |
| Penerbit (admin) | Menerbitkan setelah bukti validasi lengkap |

Satu orang boleh merangkap, tetapi **tidak boleh** meninjau pekerjaannya
sendiri tanpa pemeriksaan kedua.

## 2. Status

```
UNVALIDATED → draft → in_review → validated → published
                  ↑         │
                  └─────────┘ (revisi)
```

- `UNVALIDATED` hanya untuk contoh teknis di `content/samples/`.
- `draft`: materi disusun, belum diperiksa.
- `in_review`: sudah dikirim ke peninjau.
- `validated`: peninjau menyetujui dan bukti dicatat.
- `published`: diterbitkan oleh admin.

## 3. Bukti yang wajib dicatat

Sebelum status `validated`:

- [ ] `reviewedBy` berisi identitas peninjau (bukan "admin" generik).
- [ ] `reviewedAt` berisi tanggal tinjauan.
- [ ] `sources` mencantumkan minimal satu rujukan yang dapat diperiksa.
- [ ] Setiap kosakata dan dialog telah dibaca ulang terhadap sumber.
- [ ] Tidak ada teks yang masih berupa placeholder.
- [ ] Untuk audio: `audioLicense` dan `consentRef` terisi.
- [ ] Untuk audio: penutur menyetujui penggunaan rekaman.

## 4. Alur kerja

1. **Susun** — buat file JSON di `content/lessons/<topik>/`.
2. **Validasi struktur** — jalankan validator; perbaiki error.
3. **Tinjau** — kirim ke peninjau bahasa; ubah status ke `in_review`.
4. **Catat bukti** — isi metadata validasi; ubah status ke `validated`.
5. **Terbitkan** — admin menerbitkan; status menjadi `published`.
6. **Arsipkan sumber** — simpan rujukan di `content/references/`.

## 5. Penolakan dan revisi

- Jika peninjau menemukan kesalahan, status kembali ke `draft`.
- Alasan penolakan dicatat di `notes`.
- Riwayat perubahan mengikuti riwayat Git (lokal pada F0).

## 6. Larangan

- Menerbitkan konten `UNVALIDATED`.
- Mengisi `reviewedBy` dengan nama fiktif.
- Menyalin terjemahan mesin tanpa pemeriksaan penutur.
- Membuat audio sintetis untuk Bahasa Dawan tanpa verifikasi.
- Menghapus catatan sumber agar gate lolos.

## 7. Perubahan setelah terbit

- Perubahan materi membuat versi baru; audio lama tidak ditimpa.
- Jika versi baru gagal diunduh, versi lama tetap dipakai.
- Koreksi darurat harus tetap melalui peninjauan; tidak ada jalur pintas.

## 8. Pemantauan

Validator dijalankan pada setiap perubahan konten:

```powershell
cd tools/content-validator
npm test
node src/validate.mjs --root ../../content
```

Kegagalan validator berarti konten tidak boleh masuk build.
