# Prompt Stitch

Status: **sudah dijalankan (2026-10-26).** Hasilnya ditinjau di
`design-review-stitch.md`. Arah visual disetujui; konten di dalam mockup
tidak disetujui. Prompt disimpan sebagai jejak, bukan spesifikasi aktif.

Gunakan bersama `visual-direction.md`, `design-tokens.md`, dan `screens.md`.
Semua teks contoh memakai Bahasa Indonesia. Jangan memakai kata Dawan
karangan; gunakan placeholder seperti `[PLACEHOLDER-DWN-001]`.

## Prompt 0 — Design system

```
Buat design system untuk aplikasi pembelajaran bahasa bernama "Uab Meto".
Platform Android, orientasi portrait dan landscape, perangkat kelas rendah.

Gaya: modern, hangat, tenang, tidak kekanak-kanakan. Terinspirasi tenun
Timor: indigo malam, krem, kuning tenun, terakota. Motif hanya aksen tipis.

Keluarkan:
- palet warna dengan nama peran dan kode hex,
- skala tipografi (judul, subjudul, isi, label),
- skala spasi 4/8/12/16/24/32,
- tombol primer/sekunder/teks, kartu, chip status, bilah progres,
- aturan kontras minimal 4.5:1 dan target sentuh 48dp.

Hindari gradien berat, bayangan berlapis, dan animasi kompleks.
```

## Prompt 1 — Masuk & daftar

```
Rancang layar masuk dan daftar untuk aplikasi "Uab Meto".
Bahasa Indonesia. Latar indigo malam, teks krem, aksen kuning tenun.

Layar masuk: logo sederhana, email, kata sandi, tombol "Masuk",
tombol "Masuk dengan Google", tautan "Lupa kata sandi", tautan "Buat akun".
Layar daftar: email, kata sandi, konfirmasi, checkbox "Saya berusia
minimal 16 tahun", tautan kebijakan privasi, tombol "Daftar".

Tampilkan juga state: memuat, salah kata sandi, offline, email belum
diverifikasi. Jangan memakai ilustrasi orang yang realistis.
```

## Prompt 2 — Beranda jalur belajar

```
Rancang layar Beranda aplikasi "Uab Meto".
Bagian atas: sapaan, nickname, XP, level, streak, dan lima hati.
Bagian utama: jalur belajar vertikal berisi 12 topik; setiap topik
menampilkan 3 pelajaran dengan status terkunci/aktif/selesai.
Bagian bawah: navigasi empat tab (Beranda, Tutor AI, Peringkat, Profil).

Tampilkan banner tipis untuk status offline dan "menunggu sinkron".
Gunakan kartu membulat, ikon garis sederhana, tanpa foto.
```

## Prompt 3 — Pelajaran & latihan

```
Rancang alur satu pelajaran "Uab Meto" dalam 4 layar:
1) pembuka dengan tujuan dan tombol mulai,
2) kartu kosakata: teks Dawan, arti Indonesia, tombol putar audio,
3) soal pilihan ganda dengan umpan balik benar/salah dan penjelasan,
4) ringkasan: XP diperoleh, streak, tombol lanjut.

Gunakan placeholder [PLACEHOLDER-DWN-001] untuk teks Dawan.
Tampilkan indikator hati di pojok atas. Sediakan state "audio tidak
tersedia" yang hanya menampilkan teks.
```

## Prompt 4 — Tutor AI & percakapan

```
Rancang layar Tutor AI untuk aplikasi "Uab Meto".
Tab dalam layar: "Tutor", "Percakapan", "Pelafalan".

Tutor: percakapan teks, kartu rujukan materi, tombol pertanyaan contoh,
indikator sisa kuota harian.
Percakapan: mode teks dan suara, tombol mikrofon, indikator sesi.
Pelafalan: tombol rekam, gelombang sederhana, hasil skor atau status
"belum dapat dinilai", tombol coba lagi.

Tampilkan dialog izin mikrofon dan penjelasan bahwa audio dikirim ke
layanan AI untuk diproses.
```

## Prompt 5 — Peringkat

```
Rancang layar Peringkat aplikasi "Uab Meto".
Dua tab: "Global" dan "Mingguan".
Daftar peringkat menampilkan posisi, nickname, dan XP. Posisi pengguna
disorot. Tiga teratas memakai aksen kuning tenun yang halus.
Tampilkan state kosong dan offline (data terakhir).
Jangan menampilkan email atau nama asli.
```

## Prompt 6 — Profil, kuota, pengingat

```
Rancang layar Profil aplikasi "Uab Meto".
Isi: kartu identitas (nickname), ringkasan progres, lencana,
kuota harian (Tutor AI 10, Percakapan 3, Pelafalan 10),
pengaturan pengingat harian dengan pemilih waktu,
bagian privasi dan persetujuan suara,
tombol keluar dan "Hapus akun" dengan peringatan.

Gunakan daftar bertingkat yang mudah dipindai. Tanpa grafik rumit.
```

## Prompt 7 — Ujian akhir

```
Rancang alur ujian akhir "Uab Meto": pembuka aturan, layar soal membaca,
menulis, mendengarkan, berbicara, lalu hasil.
Hasil menampilkan nilai gabungan (dari 100), empat nilai keterampilan,
status lulus/tidak, dan rekomendasi materi perbaikan.
Tampilkan aturan: gabungan minimal 70 dan setiap keterampilan minimal 50.
Sediakan state "bagian berbicara belum dapat dinilai".
```

## Cara memakai

1. Jalankan prompt satu per satu di Stitch.
2. Simpan hasil sebagai gambar/tautan, bukan sebagai kode produksi.
3. Tinjau terhadap `screens.md` dan `visual-direction.md`.
4. Setujui atau revisi.
5. Baru setelah disetujui, terjemahkan ke Canvas Unity.
