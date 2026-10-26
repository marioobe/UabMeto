# Paket Validasi — Pelajaran 1: Salam dan sapaan

Status: **belum divalidasi.** Tidak ada materi Bahasa Dawan di paket ini.
Semua isi Dawan masih placeholder.

Tujuan: menyiapkan satu pelajaran yang bisa diperiksa, disetujui, dan
diterbitkan tanpa mengarang kosakata, terjemahan, atau dialog.

## 1. Isi paket

| Berkas | Fungsi |
|---|---|
| `../../samples/lesson-001-salam.UNVALIDATED.json` | kerangka mesin, dicek validator |
| `sources.csv` | daftar sumber yang dipakai |
| `strings.csv` | setiap string yang perlu ditinjau |
| `examples.csv` | contoh kalimat (calon field skema 1.1.0) |
| `audio-needs.csv` | kebutuhan rekaman per item |
| `consent-log.csv` | bukti izin penutur (referensi saja) |
| `review-signoff.md` | keputusan reviewer dan tanggal |

Kerangka JSON sengaja berada di `content/samples/` karena aturan repo:
file `UNVALIDATED` hanya boleh di `samples/`.

## 2. Alur kerja

1. **Kumpulkan sumber.** Isi `sources.csv` dengan minimal satu sumber yang
   dapat diperiksa ulang. Jangan mengarang sitasi.
2. **Isi string.** Ganti placeholder di `strings.csv` dengan teks Dawan dan
   Indonesia, lalu tautkan `source_id`.
3. **Isi contoh kalimat.** Lengkapi `examples.csv` dengan konteks
   penggunaan. Contoh tidak boleh berdiri tanpa sumber.
4. **Rencanakan audio.** Lengkapi `audio-needs.csv`: item, profil penutur,
   jumlah take, format Ogg Vorbis.
5. **Minta izin.** Catat izin penutur di `consent-log.csv`. Simpan bukti
   asli di luar repositori; repositori hanya menyimpan `consent_id`.
6. **Review.** Reviewer manusia memeriksa ejaan, makna, nada, dan
   kesesuaian budaya. Isi `review-signoff.md`.
7. **Validasi.** Setelah disetujui:
   - salin kerangka ke `content/lessons/lesson-001-salam.json`;
   - ganti semua placeholder dengan teks nyata;
   - isi `validation.reviewedBy`, `reviewedAt`, `sources`,
     `audioLicense`, `consentRef`;
   - ubah status menjadi `validated`;
   - jalankan validator.
8. **Terbitkan.** Hanya lewat `PublishGate` setelah status `validated` dan
   semua metadata lengkap.

## 3. Aturan keras

1. Tidak boleh mengarang kata, arti, ejaan, fonetis, atau dialog Dawan.
2. Tidak boleh memakai terjemahan mesin sebagai hasil akhir.
3. Setiap string Dawan wajib punya `source_id`.
4. Setiap audio wajib punya `consent_id`.
5. Nama penutur tidak ditampilkan tanpa izin tertulis.
6. Placeholder tidak boleh masuk build rilis.
7. Satu reviewer tidak boleh menyetujui materi yang ia susun sendiri tanpa
   reviewer kedua.

## 4. Definisi selesai

- [ ] Minimal satu sumber tercatat dan dapat diperiksa.
- [ ] Semua baris `strings.csv` berstatus `APPROVED`.
- [ ] Semua baris `examples.csv` berstatus `APPROVED`.
- [ ] Semua audio yang direncanakan punya `consent_id`.
- [ ] `review-signoff.md` ditandatangani dan bertanggal.
- [ ] JSON valid, status `validated`, tanpa placeholder.
- [ ] Validator lulus tanpa error.
- [ ] Tidak ada nama orang nyata di UI tanpa izin.

## 5. Catatan skema

Kerangka memakai `schemaVersion` `1.0.0` agar lolos validator saat ini.
Dua kebutuhan belum ada di skema:

| Kebutuhan | Status |
|---|---|
| `xp` dan `estimatedMinutes` per pelajaran | diusulkan untuk 1.1.0 |
| `examples[]` (contoh kalimat) | diusulkan untuk 1.1.0 |

Sampai skema 1.1.0 disetujui, contoh kalimat hanya hidup di `examples.csv`
dan belum bisa masuk JSON. Ini harus diputuskan sebelum F1.
