# Tinjauan Desain Stitch

Status: **F0.5 — arah visual disetujui, konten mockup tidak disetujui.**
Sumber: `stitch_uab_meto_design_system.zip` (10 layar + `DESIGN.md`),
diterima 2026-10-26. Tinjauan dilakukan pada HTML dan tangkapan layar,
bukan pada perangkat.

Dokumen ini mencatat temuan dan keputusan. Token final ada di
`design-tokens.md`; inventaris layar ada di `screens.md`; pemetaan teks ke
konten ada di `content-binding.md`.

## 1. Artefak yang ditinjau

| Layar | Berkas | Peran |
|---|---|---|
| Design system | `timor_heritage_modern_learning/DESIGN.md` | palet, tipografi, komponen |
| Masuk | `layar_masuk_uab_meto/` | autentikasi |
| Daftar | `layar_daftar_uab_meto/` | autentikasi |
| Beranda | `beranda_uab_meto/` | jalur belajar |
| Pembuka pelajaran | `pembuka_pelajaran_uab_meto/` | awal pelajaran |
| Kartu kosakata | `kartu_kosakata_uab_meto/` | materi |
| Latihan pilihan ganda | `latihan_pilihan_ganda_uab_meto/` | latihan |
| Ringkasan pelajaran | `ringkasan_pelajaran_uab_meto/` | penutup |
| Tutor AI | `tutor_ai_uab_meto/` | AI |
| Peringkat | `peringkat_uab_meto/` | gamifikasi |
| Profil | `profil_uab_meto/` | akun, kuota, pengingat |

Tidak diserahkan: ujian akhir, lupa kata sandi, splash, verifikasi email
mandiri, dialog pelajaran, penjelasan tata bahasa, dan lima dari enam jenis
latihan.

## 2. Keputusan yang dikunci

| Aspek | Keputusan | Konsekuensi |
|---|---|---|
| Arah visual | Disetujui sebagai dasar UI Unity | Palet, tipografi, bentuk, dan komponen dipakai |
| Daftar topik | 12 topik PRD §3.2 persis | "Alam, Ladang & Hewan" dan "Cerita Tradisi" dibuang; ujian akhir keluar dari jalur topik |
| Varian bahasa | TTU/Kefamenanu saja | Label Amarasi, Amanuban, Soe, Kupang, Sonbai, Mutis dihapus |
| Teknologi UI | uGUI Canvas + TextMeshPro | Sesuai PRD §3.14; bukan WebView, bukan UI Toolkit |

## 3. Temuan P0 — menghalangi implementasi

### P0-1. Daftar topik tidak sesuai PRD

Mockup memuat 12 topik, tetapi komposisinya berbeda dari PRD §3.2.
Rincian ada di §7. Dua topik wajib hilang, dua topik baru muncul, satu
topik digabung, satu dipecah. Ujian akhir juga dijadikan "Topik 12",
padahal PRD §3.9 menempatkannya setelah seluruh jalur selesai.

**Tindakan:** pakai daftar PRD apa adanya; ujian akhir menjadi alur
terpisah di luar `topicId`.

### P0-2. Cakupan dialek melenceng dari TTU

Mockup menyebut "Dialek Amarasi / Dawan Netral", "Amanuban & Amarasi",
"Dialek Miomafo", "Divisi Sonbai", "Tingkat 4 · Mutis", "Kupang & Soe".
Amarasi, Amanuban, Soe, dan Kupang berada di luar TTU. Ini mengubah janji
produk, bukan sekadar label.

**Tindakan:** satu varian acuan, TTU sekitar Kefamenanu. Nama dialek lain
hanya boleh muncul di catatan editorial, bukan di UI.

### P0-3. Teks Dawan belum tervalidasi tampil sebagai fakta

Mockup memuat materi Dawan yang tidak punya `sources` atau `reviewedBy`.
Lokasinya:

- empat kata bilangan pada layar kosakata, ikhtisar materi, dan ringkasan;
- transkripsi fonetis dan klaim tentang penekanan suku kata;
- frasa contoh pada kartu kosakata beserta terjemahannya;
- ungkapan persatuan pada kartu tip;
- kalimat sapaan tamu pada Tutor AI;
- kalimat pembeli pada layar Percakapan;
- peribahasa pada layar Masuk;
- label bahasa Dawan di bawah judul Tutor AI.

Kutipan kata tidak diulang di dokumen ini agar tidak tersalin menjadi
materi. Semua melanggar aturan konten F0 dan PRD §5.4.

**Tindakan:** ganti dengan `[PLACEHOLDER-DWN-xxx]` di semua dokumen dan
build sampai ada sumber tervalidasi. Teks Dawan hanya boleh berasal dari
`content/lessons/` berstatus `validated`.

### P0-4. Klaim dan metrik karangan

| Klaim | Lokasi | Masalah |
|---|---|---|
| "Model: Uab Meto v2.4 (Aktif)" | Tutor AI | Tidak ada model khusus; Workers AI generik |
| "Latensi 42ms" | Pelafalan | Angka karangan |
| "Terverifikasi Komunitas" | Profil | Fitur tidak ada di PRD |
| "Tersinkron" | Profil | Status harus dari keadaan nyata |
| "14.2 MB modul tersimpan" | Profil | Harus dihitung dari berkas |
| "v1.2.0", "Sejak Jan 2025" | Profil | Versi dan tanggal palsu |
| "12 / 15 Tanya" | Tutor AI | Kuota PRD 10, bukan 15 |
| "+25 XP" vs "+15 XP" | Beranda vs Kartu | XP pelajaran sama berbeda |
| "Divisi Sonbai" | Peringkat | Struktur sosial karangan |
| "Gelar Adat: Penutur Fasih Meto" | Beranda | Pemberian gelar adat, tidak ada di PRD |

**Tindakan:** semua angka berasal dari konten, Firestore, Worker, atau
build metadata. Yang tidak punya sumber dihapus.

### P0-5. Konfigurasi saling bertentangan

- Kuota Tutor: 15 (mockup) vs 10 (PRD §5.2).
- XP pelajaran: +15 dan +25 untuk pelajaran yang sama.
- "3 Kata Baru" padahal ikhtisar berisi empat kata.
- Progres: "Langkah 1 dari 3" dengan bilah 40%; "Pelajaran 2 dari 3" tetapi
  ringkasan menyebut "Topik 3: Pelajaran 2".
- Hati: header 5/5, panel salah menulis "Sisa 4" tanpa memperbarui header.

**Tindakan:** satu model progres `topic > lesson > step`; XP, durasi, dan
jumlah kata dari JSON; hati dari state tunggal.

### P0-6. Atribusi orang nyata tanpa bukti

"Oleh Bpk. Y. Kase (Penutur Asli Amarasi)" menampilkan nama dan klaim
status penutur asli. Skema konten mewajibkan `consentRef` untuk audio.

**Tindakan:** ganti dengan `[PLACEHOLDER-SPEAKER]` sampai ada perjanjian
tertulis. Jangan tampilkan nama tanpa izin.

### P0-7. Panel pratinjau desainer bocor ke layar produk

"SIMULASI JARINGAN", "Pratinjau Status Sistem", "Mode Desainer", toggle
"Audio Tersedia / Belum Tersedia", dan pengalih state latihan muncul di
dalam layar yang terlihat seperti produk.

**Tindakan:** pindahkan ke debug overlay yang hanya dikompilasi pada
`DEVELOPMENT_BUILD`. Tidak boleh ada di APK rilis.

### P0-8. `DESIGN.md` bertentangan dengan mockup

| Butir | `DESIGN.md` | Mockup |
|---|---|---|
| Bayangan | "zero heavy shadows" | `shadow-md`, `shadow-lg`, `shadow-2xl` |
| Blur | "no blurred glass layers" | `backdrop-blur-xl` di header dan nav |
| Kanvas | `#0E171C` | `#0b1519` |
| Sekunder | `#C96F4A` | token `secondary` = `#FFB599` |
| Primer | `#F3D08F` | token `primary` = `#FFEFD7` |

**Tindakan:** `design-tokens.md` menjadi satu-satunya sumber. Mockup
diperlakukan sebagai referensi komposisi, bukan spesifikasi teknis.

## 4. Temuan P1 — belum ada desainnya

1. **Landscape tidak ada.** AC-6.9.3 mewajibkan portrait dan landscape.
2. **Layar hilang:** splash, lupa sandi, verifikasi email mandiri, dialog,
   penjelasan, ujian akhir (pembuka, empat bagian, hasil, rekomendasi).
3. **Jenis latihan:** hanya pilihan ganda. Mencocokkan, menyusun kalimat,
   mendengarkan, menulis, dan berbicara belum ada.
4. **Nickname** tidak diminta saat daftar, padahal papan peringkat
   memerlukannya.
5. **Hapus akun** hanya satu `confirm()`. PRD butuh konfirmasi ganda,
   autentikasi ulang, dan pembersihan cache.
6. **Login offline dengan sesi tersimpan** belum punya jalur; banner hanya
   memblokir.
7. **State AI** belum ada: kuota habis, layanan tidak tersedia, referensi
   tidak ditemukan, izin mikrofon ditolak.
8. **Target sentuh** di bawah 48 dp pada banyak tombol ikon dan chip.
9. **Navigasi tidak konsisten** antar layar: ikon dan tinggi bottom bar
   berbeda, header Peringkat dan Profil masih berjudul "Beranda".
10. **Kamus Saku** ("Simpan Kata") adalah fitur baru di luar PRD.

## 5. Temuan P2 — perbaikan sebelum build

- "Tutor Ai" seharusnya "Tutor AI".
- Chip "Fase Pengenalan" dan label "LANGKAH 1 DARI 3 · KOSAKATA BARU"
  terpotong/terbungkus di 360 dp.
- Nama penutur terpotong ("Oleh Bpk. Y. Kase (Penut...").
- Tombol "Buka Materi [PLACEHOLDER-DWN-003]" memotong token di tengah.
- Chip pertanyaan cepat meluber keluar layar di Tutor AI.
- "Visualisasi Spektrogram" sebenarnya waveform, bukan spektrogram.
- Logo memakai URL `lh3.googleusercontent.com`; harus aset lokal.
- `min-height: max(884px, 100dvh)` adalah hack pratinjau, bukan aturan.

## 6. Yang dipertahankan

- Palet indigo malam, krem, kuning tenun, terakota.
- Motif belah ketupat sebagai hairline, bukan pola penuh.
- Kartu topik tiga status: terkunci, aktif, selesai.
- Bilah progres dan kelompok stat XP/streak/hati.
- Panel umpan balik benar/salah dengan ikon dan warna.
- Banner offline dan outbox.
- State "Audio Belum Tersedia" dan "Belum Dapat Dinilai".
- Dialog persetujuan suara sebelum fitur suara pertama.
- Bottom navigation empat tab sesuai PRD §3.14.
- Dua tab peringkat: mingguan dan sepanjang waktu.

## 7. Matriks topik

| # | PRD §3.2 | Mockup | Tindakan |
|---|---|---|---|
| 1 | Salam dan sapaan | Salam & Sapaan Dasar | pakai |
| 2 | Perkenalan diri | digabung dengan Keluarga | pisahkan |
| 3 | Keluarga dan hubungan | digabung dengan Perkenalan | pisahkan |
| 4 | Angka, waktu, dan hari | dipecah jadi Angka & Berhitung + Waktu, Hari & Cuaca | satukan |
| 5 | Aktivitas sehari-hari | Kegiatan Sehari-hari | pakai |
| 6 | Makanan dan minuman | Makanan & Minuman Khas Timor | pakai |
| 7 | Rumah dan benda sekitar | Benda di Sekitar Rumah | pakai |
| 8 | Tempat dan arah | Tempat & Arah Perjalanan | pakai |
| 9 | Belanja dan tawar-menawar | Percakapan Pasar & Transaksi | pakai |
| 10 | Budaya dan adat Dawan | Adat Istiadat & Tenun Meto | pakai |
| 11 | Kesehatan dan keadaan darurat | tidak ada | tambah |
| 12 | Sekolah, pekerjaan, kegiatan sosial | tidak ada | tambah |
| — | — | Alam, Ladang & Hewan | buang |
| — | — | Cerita Tradisi & Ujian Akhir | buang; ujian jadi alur terpisah |

Target 36 pelajaran (3 per topik) tetap dipakai sebagai sasaran
perencanaan, bukan janji rilis.

## 8. Klaim yang dilarang tampil

Sampai ada bukti, teks berikut tidak boleh muncul di UI:

- nama model atau versi model AI;
- latensi, akurasi, atau skor yang tidak diukur;
- status "tersinkron" tanpa hasil sinkron nyata;
- nama penutur tanpa `consentRef`;
- gelar adat atau status komunitas;
- jumlah modul, ukuran cache, atau versi aplikasi yang di-hardcode;
- teks Dawan tanpa `sources` dan `reviewedBy`.

## 9. Kriteria keluar F0.5

1. `design-tokens.md` menjadi sumber tunggal token.
2. `screens.md` memuat seluruh layar dan state, termasuk landscape.
3. `content-binding.md` memetakan setiap teks ke sumbernya.
4. `accessibility.md` memuat kontras terhitung dan audit target sentuh.
5. Tidak ada teks Dawan yang disajikan sebagai materi contoh; kutipan
   pelanggaran diredaksi.
6. Daftar topik sama persis dengan PRD §3.2.
7. Traceability AC-6.9.1 diperbarui dengan bukti.
