# Inventaris Layar & Alur Navigasi

Status: **usulan F0.** Belum ada mockup final. Nama layar dapat berubah
setelah desain disetujui.

## 1. Navigasi utama

Empat tab bawah:

1. **Beranda** — jalur belajar, pelajaran berikutnya, XP, streak, hati.
2. **Tutor AI** — tutor, percakapan, latihan pelafalan.
3. **Peringkat** — global dan mingguan.
4. **Profil** — progres, lencana, kuota, pengingat, akun.

Alur di luar tab: autentikasi, pelajaran, ujian akhir, pengaturan.

## 2. Autentikasi

| Layar | Isi utama | Status yang harus ada |
|---|---|---|
| Splash | Logo, cek sesi | memuat, sesi valid, sesi kedaluwarsa |
| Masuk | Email, kata sandi, tombol Google, lupa sandi | kosong, salah, offline, terkunci sementara |
| Daftar | Email, sandi, konfirmasi 16+, setuju privasi | validasi, email sudah dipakai |
| Verifikasi email | Instruksi, kirim ulang | menunggu, terverifikasi, gagal kirim |
| Lupa sandi | Email | terkirim, email tidak dikenal (pesan netral) |
| Persetujuan suara | Penjelasan pengiriman audio ke AI | belum setuju, setuju |

Catatan: login online pertama wajib; sesi tersimpan agar materi offline
dapat dibuka.

## 3. Beranda

- Sapaan + nickname.
- XP, level, streak, hati.
- Jalur belajar per topik (12 topik).
- Kartu pelajaran: terkunci, aktif, selesai.
- Banner offline / menunggu sinkron.

Status: memuat, kosong (belum ada materi), offline, gagal sinkron.

## 4. Pelajaran

| Layar | Isi |
|---|---|
| Pembuka pelajaran | Tujuan, estimasi waktu, tombol mulai |
| Kosakata | Daftar kartu, audio, contoh |
| Dialog | Percakapan per baris, audio, terjemahan |
| Penjelasan | Catatan penggunaan |
| Latihan | Satu soal per layar |
| Umpan balik | Benar/salah, alasan, hati berkurang |
| Ringkasan | XP diperoleh, streak, lanjut |

Jenis latihan: pilihan ganda, mencocokkan, menyusun kalimat, mendengarkan,
menulis, berbicara.

Status khusus:

- Audio tidak tersedia → tampilkan teks, tanpa audio palsu.
- Pelafalan tidak meyakinkan → "belum dapat dinilai".
- Offline → latihan tetap berjalan, hasil masuk outbox.

## 5. Tutor AI

| Layar | Isi |
|---|---|
| Tutor | Percakapan teks, rujukan materi, tombol contoh |
| Percakapan | Mode teks/voice, indikator sesi, kuota |
| Pelafalan | Rekam, kirim, hasil/saran, coba lagi |

Status: kuota habis, layanan AI tidak tersedia, referensi tidak ditemukan,
izin mikrofon ditolak.

## 6. Peringkat

- Tab Global (sepanjang waktu) dan Mingguan.
- Baris: peringkat, nickname, XP.
- Posisi pengguna disorot.
- Tidak menampilkan email/nama asli.

Status: memuat, kosong, offline (tampilkan data terakhir bila ada).

## 7. Profil

- Nickname, status akun.
- Progres per keterampilan.
- Lencana.
- Kuota harian per fitur.
- Pengingat: aktif/nonaktif, waktu.
- Privasi: kebijakan, persetujuan suara.
- Akun: logout, hapus akun (konfirmasi ganda).

## 8. Ujian akhir

| Layar | Isi |
|---|---|
| Pembuka | Syarat, aturan, empat bagian |
| Membaca | Soal |
| Menulis | Soal |
| Mendengarkan | Audio + soal |
| Berbicara | Rekam |
| Hasil | Nilai gabungan, nilai per keterampilan, lulus/tidak |
| Rekomendasi | Materi perbaikan, tombol ulang |

Aturan: gabungan ≥70, tiap keterampilan ≥50, percobaan tak terbatas,
bagian berbicara yang tidak dapat dinilai tidak menentukan kelulusan.

## 9. Status lintas layar

Setiap layar wajib mendefinisikan:

- memuat,
- kosong,
- error,
- offline,
- kuota habis (bila relevan),
- sesi kedaluwarsa.

## 10. Di luar aplikasi Android

- Dashboard admin adalah aplikasi web terpisah.
- Kebijakan privasi dapat berupa halaman web.
