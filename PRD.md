# PRODUCT REQUIREMENTS DOCUMENT (PRD)

## Uab Meto — Aplikasi Pembelajaran Bahasa Dawan

**Versi:** 1.0  
**Platform:** Android  
**Engine:** Unity 6 (6000.6.0f1)  
**Nama aplikasi:** Uab Meto  
**Bahasa antarmuka:** Bahasa Indonesia  
**Target pengguna:** Masyarakat umum usia 16 tahun ke atas yang belum bisa atau masih pemula dalam Bahasa Dawan  
**Varian bahasa:** Bahasa Dawan (Uab Meto), wilayah Timor Tengah Utara (TTU), khususnya sekitar Kefamenanu.

---

## 1. MASALAH

### 1.1 Latar belakang

Bahasa Dawan merupakan bahasa daerah yang digunakan oleh masyarakat di Pulau Timor. Masyarakat yang ingin mempelajarinya membutuhkan materi yang mudah dipahami, konsisten, dan dapat digunakan untuk berlatih secara mandiri.

Pembelajaran bahasa tidak cukup hanya menghafal kosakata. Pengguna juga perlu berlatih membaca, menulis, mendengarkan, dan berbicara dalam konteks kehidupan sehari-hari.

Aplikasi pembelajaran yang menggunakan materi audio dan layanan AI juga perlu memastikan bahwa contoh bahasa serta pelafalan yang diajarkan sesuai dengan varian lokal yang menjadi acuan.

### 1.2 Permasalahan utama

1. Pemula membutuhkan jalur pembelajaran terstruktur dari materi dasar menuju percakapan sehari-hari.
2. Pengguna membutuhkan contoh pelafalan dan dialog yang dapat dipercaya, bukan sekadar hasil teks yang belum diverifikasi.
3. Koneksi internet yang terbatas dapat menghambat akses materi pembelajaran.
4. Pengguna membutuhkan latihan interaktif, umpan balik, serta motivasi agar konsisten belajar.
5. Materi Bahasa Dawan perlu dikelola dan divalidasi sebelum dipublikasikan.
6. Pemanfaatan AI untuk pembelajaran dan penilaian pelafalan harus memiliki batas penggunaan, mekanisme keamanan, serta penanganan ketidakpastian yang jelas.

### 1.3 Tujuan produk

Uab Meto bertujuan menyediakan aplikasi Android untuk mempelajari Bahasa Dawan varian TTU secara bertahap, interaktif, dan dapat digunakan secara offline untuk pembelajaran inti.

Produk memanfaatkan AI sebagai tutor dan pendamping latihan, dengan materi yang telah divalidasi sebagai sumber utama. Aplikasi juga menyediakan gamifikasi, ujian akhir, dan dashboard admin web untuk pengelolaan konten.

### 1.4 Indikator keberhasilan

Produk dinilai berhasil apabila:

- Pengguna dapat mengikuti jalur belajar dari tingkat pemula.
- Keempat keterampilan bahasa memiliki materi dan latihan yang sesuai.
- Seluruh materi inti dan audio yang telah dipaketkan dapat digunakan tanpa internet setelah login pertama.
- AI memberikan jawaban yang bersumber dari materi tervalidasi dan tidak mengarang informasi ketika referensi tidak tersedia.
- Progres belajar tersimpan secara lokal dan tersinkronisasi ke akun saat internet tersedia.
- Admin dapat mengelola serta menerbitkan materi tanpa harus membangun ulang APK untuk setiap perubahan konten.

---

## 2. PENGGUNA

### 2.1 Pengguna utama

Masyarakat umum berusia 16 tahun ke atas yang belum bisa atau masih pemula dalam Bahasa Dawan.

Karakteristik pengguna:

- Membutuhkan penjelasan sederhana dalam Bahasa Indonesia.
- Memerlukan latihan singkat dan bertahap.
- Membutuhkan contoh kata, kalimat, dialog, dan audio.
- Dapat menggunakan perangkat Android dengan spesifikasi terbatas.
- Membutuhkan akses ke materi dasar tanpa koneksi internet.

### 2.2 Admin aplikasi

Satu akun admin utama bertugas mengelola konten dan operasional aplikasi.

Admin dapat:

- Membuat serta mengubah jalur dan pelajaran.
- Mengelola kosakata, dialog, dan audio.
- Mengelola soal latihan dan ujian akhir.
- Memvalidasi serta menerbitkan materi.
- Mengelola akun pengguna sesuai kebutuhan operasional.
- Melihat progres belajar dan hasil ujian.
- Melihat laporan statistik penggunaan.

Hanya admin yang berwenang menerbitkan materi ke aplikasi. Referensi dan validasi dari penutur lokal yang memahami varian TTU digunakan untuk menjaga ketepatan konten.

### 2.3 Pengguna yang tidak menjadi sasaran versi awal

Aplikasi belum ditujukan untuk anak di bawah 16 tahun, pembelajar tingkat lanjut dengan kebutuhan akademik khusus, atau pengguna yang membutuhkan sertifikasi kompetensi resmi.

---

## 3. FITUR

### 3.1 Registrasi dan autentikasi

Aplikasi menyediakan:

- Pendaftaran dan login menggunakan email serta kata sandi.
- Login menggunakan akun Google.
- Verifikasi email untuk pendaftaran menggunakan email dan kata sandi.
- Pemulihan kata sandi melalui email.
- Logout dan pengelolaan sesi login.
- Profil sederhana dengan nickname.
- Penghapusan akun dan data terkait melalui aplikasi.

Ketentuan:

- Login online wajib dilakukan saat pertama kali menggunakan aplikasi.
- Setelah login berhasil, sesi disimpan secara aman agar pengguna dapat mengakses materi offline.
- Login Google tidak memerlukan verifikasi tambahan jika email akun telah terverifikasi.
- Nama asli, tanggal lahir lengkap, dan alamat tempat tinggal tidak diwajibkan.
- Pengguna mengonfirmasi bahwa dirinya berusia minimal 16 tahun saat pendaftaran.

### 3.2 Jalur pembelajaran

Uab Meto menggunakan pola belajar bertahap seperti aplikasi pembelajaran bahasa berbasis gamifikasi.

Target awal adalah **36 pelajaran dalam 12 topik**, dengan rata-rata tiga pelajaran untuk setiap topik. Jumlah ini menjadi target perencanaan yang perlu disesuaikan dengan ketersediaan materi terverifikasi.

Dua belas topik awal:

1. Salam dan sapaan.
2. Perkenalan diri.
3. Keluarga dan hubungan.
4. Angka, waktu, dan hari.
5. Aktivitas sehari-hari.
6. Makanan dan minuman.
7. Rumah dan benda sekitar.
8. Tempat dan arah.
9. Belanja dan tawar-menawar.
10. Budaya dan adat Dawan.
11. Kesehatan dan keadaan darurat.
12. Sekolah, pekerjaan, dan kegiatan sosial.

Setiap topik disusun secara bertahap dari pengenalan materi, latihan, hingga penerapan dalam situasi sehari-hari.

Pelajaran berikutnya terbuka setelah pelajaran sebelumnya selesai. Nilai minimum tidak diperlukan untuk membuka pelajaran berikutnya.

### 3.3 Materi dan latihan interaktif

Setiap pelajaran dapat memuat:

- Kosakata Bahasa Dawan dan arti dalam Bahasa Indonesia.
- Contoh kalimat dan dialog.
- Penjelasan penggunaan ungkapan.
- Audio acuan penutur lokal.
- Latihan membaca.
- Latihan menulis.
- Latihan mendengarkan.
- Latihan berbicara.
- Kuis dan penjelasan jawaban.

Jenis latihan dapat mencakup pilihan ganda, mencocokkan kata, menyusun kalimat, menjawab berdasarkan audio, serta aktivitas menulis dan berbicara.

Materi harus mengacu pada referensi tepercaya dan divalidasi sebelum diterbitkan.

### 3.4 Tutor AI

Tutor AI menjadi pendamping belajar untuk Bahasa Dawan dan budaya Dawan.

Kemampuan yang dibutuhkan:

- Menjelaskan kosakata dan maknanya.
- Menjelaskan penggunaan kalimat berdasarkan referensi.
- Menjawab pertanyaan tentang materi dan budaya Dawan yang telah divalidasi.
- Memberikan koreksi serta alasan atas jawaban yang salah.
- Memberikan rekomendasi latihan tambahan.
- Mengakui keterbatasan ketika informasi tidak ditemukan dalam materi tervalidasi.

Bahasa penjelasan utama adalah Bahasa Indonesia.

Tutor AI tidak boleh menggunakan pengetahuan model yang belum diverifikasi sebagai fakta Bahasa Dawan. Jika referensi tidak mencukupi, AI harus menyatakan bahwa jawaban belum dapat dipastikan.

### 3.5 Percakapan AI

Pengguna dapat berlatih percakapan melalui teks dan suara.

Ketentuan:

- Pengguna dapat mengetik atau berbicara melalui mikrofon.
- AI memberikan balasan dalam teks.
- Balasan audio Bahasa Dawan hanya diputar jika tersedia rekaman terverifikasi yang sesuai.
- Jika audio acuan tidak tersedia, aplikasi menampilkan teks dan penjelasan tanpa menghasilkan suara Bahasa Dawan yang belum tervalidasi.
- Satu sesi percakapan dihitung dalam kuota harian.
- Pengguna tetap dapat menggunakan percakapan berbasis teks tanpa izin mikrofon.

### 3.6 Latihan pelafalan berbasis suara

Pengguna merekam ucapan untuk dianalisis oleh layanan AI.

Fungsi yang dibutuhkan:

- Mengirim suara untuk analisis.
- Memberikan skor dan saran ketika hasil analisis cukup meyakinkan.
- Memungkinkan pengguna mencoba kembali.
- Menandai hasil sebagai **belum dapat dinilai** jika analisis tetap tidak meyakinkan.
- Tidak menyatakan pengguna lulus atau gagal berdasarkan nilai pelafalan yang tidak dapat dipercaya.

Kompatibilitas model pengenal suara dengan Bahasa Dawan varian TTU wajib diuji sebelum fitur ini dinyatakan layak digunakan.

### 3.7 Pembelajaran adaptif

AI menyesuaikan latihan berdasarkan hasil dan tingkat penguasaan pengguna.

Sistem harus mampu:

- Mengidentifikasi kosakata atau keterampilan yang sering dijawab salah.
- Memberikan latihan penguatan.
- Menyesuaikan kesulitan secara bertahap.
- Menggunakan materi yang telah divalidasi.
- Menyimpan hasil pembelajaran untuk mendukung rekomendasi berikutnya.

Semua pengguna memulai dari materi dasar.

### 3.8 Gamifikasi

Uab Meto menyediakan enam fitur gamifikasi.

**Poin pengalaman (XP)**

XP diberikan setelah pengguna menyelesaikan aktivitas belajar yang memenuhi ketentuan.

**Level dan jalur belajar**

Level menunjukkan perkembangan pengguna. Jalur belajar membuka pelajaran berikutnya setelah pelajaran sebelumnya selesai.

**Streak belajar**

Streak bertambah jika pengguna menyelesaikan satu pelajaran atau satu paket latihan inti sampai selesai dan memperoleh XP. Membuka aplikasi saja tidak dihitung.

**Lencana pencapaian**

Lencana diberikan atas target pembelajaran atau pencapaian tertentu.

**Papan peringkat**

Terdapat dua jenis peringkat:

- Global sepanjang waktu.
- Mingguan, dengan perhitungan ulang setiap minggu.

Papan peringkat menampilkan nickname, bukan nama asli atau email.

**Sistem hati atau nyawa**

- Pengguna memulai dengan lima hati.
- Hati berkurang ketika pengguna salah pada latihan inti tertentu, seperti pilihan ganda dan penyusunan kalimat.
- Hati tidak berkurang saat membaca materi, menggunakan Tutor AI, atau berlatih pelafalan.
- Hati dapat pulih otomatis atau melalui penyelesaian latihan tertentu.

Rincian waktu regenerasi serta jumlah hati yang dipulihkan oleh suatu latihan ditetapkan saat desain teknis.

### 3.9 Ujian akhir

Setelah menyelesaikan jalur pembelajaran, pengguna dapat mengikuti ujian akhir yang mengukur membaca, menulis, mendengarkan, dan berbicara.

Aturan kelulusan:

- Nilai gabungan minimal 70 dari 100.
- Setiap keterampilan harus memperoleh nilai minimal 50.
- Jika tidak lulus, pengguna mendapatkan rekomendasi materi yang perlu diperbaiki.
- Pengguna dapat mengulang ujian tanpa batas percobaan.
- Hasil berbicara yang belum dapat dinilai tidak boleh dianggap sebagai nilai lulus atau gagal.

### 3.10 Akses offline dan sinkronisasi

Seluruh materi inti yang direncanakan, termasuk audio acuan dan latihan, tersedia sejak aplikasi dipasang.

Fitur offline:

- Membuka seluruh pelajaran yang telah disertakan.
- Memutar audio yang tersedia secara lokal.
- Mengerjakan latihan inti.
- Menyimpan hasil belajar di perangkat.

Saat koneksi kembali tersedia, aplikasi menyinkronkan progres ke Firebase secara otomatis, termasuk hasil latihan, XP, level, streak, serta pencapaian.

Jika pembaruan materi gagal diunduh, versi lokal sebelumnya tetap tersedia.

Fitur AI dan sinkronisasi server memerlukan internet.

### 3.11 Pengingat belajar

Pengguna dapat:

- Mengaktifkan atau menonaktifkan pengingat.
- Memilih waktu pengingat harian.
- Melihat streak dan progres belajar.

### 3.12 Dashboard admin web

Dashboard admin merupakan aplikasi web terpisah dari aplikasi Android.

Fungsi dashboard:

1. Mengelola jalur dan pelajaran.
2. Mengelola kosakata serta dialog.
3. Mengunggah dan mengganti audio.
4. Mengelola latihan dan ujian.
5. Memvalidasi serta menerbitkan materi.
6. Melihat dan mengelola data operasional pengguna.
7. Melihat laporan penggunaan, progres, dan hasil ujian.

Akses dashboard hanya untuk satu akun admin utama yang telah dikonfigurasi secara khusus.

Penerbitan materi harus dibedakan dari penyuntingan agar materi yang belum diperiksa tidak langsung tersedia kepada pengguna.

### 3.13 Pengelolaan data dan privasi

Data yang disimpan mencakup:

- Identitas autentikasi dan nickname.
- Status akun.
- Progres pembelajaran.
- XP, level, streak, dan pencapaian.
- Hasil latihan dan ujian.
- Hasil analisis serta saran pelafalan.

Rekaman suara diproses untuk analisis dan tidak disimpan sebagai riwayat audio oleh Uab Meto.

Sebelum pertama kali menggunakan fitur suara, pengguna harus menyetujui pemberitahuan bahwa suara dikirim ke layanan AI untuk diproses.

Pengguna dapat menghapus akun sendiri. Penghapusan harus mencakup data akun dan progres terkait, pembersihan sesi, serta penghapusan cache lokal yang sesuai.

### 3.14 Navigasi dan desain antarmuka

Empat menu navigasi utama:

1. **Beranda:** jalur belajar, pelajaran berikutnya, XP, dan streak.
2. **Tutor AI:** tutor, percakapan, dan latihan pelafalan.
3. **Peringkat:** papan peringkat global dan mingguan.
4. **Profil:** progres, pencapaian, kuota, pengingat, dan pengelolaan akun.

Antarmuka menggunakan Bahasa Indonesia.

Arah visual ditentukan melalui rekomendasi AI dengan prioritas:

- Mudah dipahami oleh pemula.
- Modern dan menyenangkan untuk belajar.
- Menghormati identitas budaya Dawan dan Timor.
- Menggunakan aset 2D serta animasi ringan.
- Realistis untuk diimplementasikan pada perangkat Android berspesifikasi rendah.

Desain dibuat melalui Stitch, lalu diwujudkan sebagai **UI asli Unity menggunakan Canvas, komponen UI, dan C#**. Mockup Stitch tidak dianggap otomatis menjadi UI Unity; komponen dan interaksinya harus diimplementasikan serta diuji di Unity.

---

## 4. DI LUAR CAKUPAN

Fitur dan platform berikut tidak termasuk dalam versi awal:

1. Aplikasi iOS, desktop, dan web untuk pengguna belajar.
2. Pembelajaran bahasa selain varian Dawan TTU yang menjadi acuan.
3. Sertifikat kelulusan resmi atau sertifikasi kompetensi terakreditasi.
4. Sistem pembayaran, langganan berbayar, atau pembelian kuota AI.
5. Pendaftaran admin publik, banyak level peran admin, atau panel editor terpisah.
6. Pengiriman materi oleh komunitas secara langsung ke aplikasi.
7. Chatbot umum untuk pertanyaan di luar pembelajaran Bahasa Dawan dan budaya Dawan yang tervalidasi.
8. Pembuatan audio Bahasa Dawan secara otomatis tanpa verifikasi.
9. Penyimpanan riwayat rekaman suara pengguna.
10. Sistem pengajaran langsung oleh guru manusia.
11. Ujian penempatan sebelum mulai belajar.
12. Fitur yang mengharuskan pengguna selalu online untuk membuka materi dasar.
13. Fitur sosial seperti pesan pribadi antarpengguna.

---

## 5. BATASAN

### 5.1 Batasan teknologi

Aplikasi dikembangkan menggunakan:

- Unity 6 versi 6000.6.0f1.
- Lokasi editor: `D:\Unity2022\6000.6.0f1\Editor\Unity.exe`.
- Bahasa pemrograman C#.
- UI mobile asli Unity, bukan WebView sebagai tampilan utama.
- Firebase Authentication dan Cloud Firestore untuk autentikasi serta data pembelajaran.
- Cloudflare Workers sebagai perantara layanan AI dan pemeriksaan kuota.
- Cloudflare Workers AI sebagai kandidat layanan AI awal.
- Supabase Storage Free untuk audio pembelajaran.
- Dashboard admin berbasis web.

Kunci API AI, kredensial administrator, dan rahasia server tidak boleh ditanam langsung dalam kode aplikasi atau APK.

### 5.2 Batasan biaya layanan

Pengembangan harus mengutamakan layanan gratis dan tidak bergantung pada pengaktifan billing Firebase.

Firebase Spark memiliki kuota gratis, tetapi tidak menyediakan akses Cloud Functions. Firebase Storage juga memerlukan paket Blaze. Oleh karena itu, Cloudflare Workers dan Supabase Storage digunakan untuk melengkapi arsitektur tanpa menjadikan Firebase berbayar sebagai prasyarat. Referensi: [Firebase pricing plans](https://firebase.google.com/docs/projects/billing/firebase-pricing-plans) dan [Firebase Storage billing requirements](https://firebase.google.com/docs/storage/faqs-storage-changes-announced-sept-2024).

Kuota awal per akun per hari:

| Fitur             |         Batas |
| ----------------- | ------------: |
| Tutor AI          | 10 pertanyaan |
| Percakapan AI     |        3 sesi |
| Latihan pelafalan |       10 kali |

Kuota aplikasi direset pada pukul 00.00 WITA berdasarkan waktu server.

Cloudflare Workers AI memiliki batas penggunaan gratis bersama sebesar 10.000 Neurons per hari pada dokumentasi yang diperiksa, dengan reset pukul 00.00 UTC atau 08.00 WITA. Kuota layanan global ini berbeda dari kuota per akun Uab Meto. Jika batas layanan tercapai, aplikasi harus menampilkan pesan yang sesuai dan tetap menyediakan pembelajaran offline. Referensi: [Cloudflare Workers AI pricing](https://developers.cloudflare.com/workers-ai/platform/pricing/).

Kuota gratis dapat berubah dan tidak menjamin seluruh pengguna selalu mendapat kapasitas AI tanpa gangguan.

### 5.3 Batasan penyimpanan audio

Supabase Storage Free menyediakan penyimpanan file sebesar 1 GB dan kuota bandwidth terbatas. Proyek gratis dapat dijeda setelah satu minggu tidak aktif. Batas ini harus dipantau, terutama setelah aplikasi dipublikasikan. Referensi: [Supabase pricing](https://supabase.com/pricing).

Audio harus dikompresi tanpa menghilangkan kejelasan pelafalan. Audio yang sudah terunduh tetap tersedia secara lokal ketika offline.

### 5.4 Batasan bahasa dan AI

- Ketepatan Bahasa Dawan bergantung pada materi terverifikasi.
- Kemampuan model dalam mengenali ucapan Bahasa Dawan varian TTU belum boleh diasumsikan akurat sebelum pengujian.
- Skor pelafalan hanya diberikan apabila hasil analisis cukup meyakinkan.
- AI tidak boleh menghasilkan balasan audio Bahasa Dawan yang belum divalidasi.
- Jika referensi tidak tersedia, AI harus mengakui keterbatasannya.
- Seluruh materi dan rekaman yang diterbitkan harus memiliki sumber serta izin penggunaan yang sesuai.

### 5.5 Batasan perangkat

Aplikasi harus dioptimalkan bagi ponsel Android dengan spesifikasi rendah.

- Minimum Android: 8.0, API 26, sesuai persyaratan minimum Unity 6.6.
- Target API untuk distribusi Google Play: Android 16, API 36 atau lebih tinggi, sesuai persyaratan yang berlaku sejak 31 Agustus 2026.
- Orientasi: portrait dan landscape.
- Target ukuran unduhan awal: diupayakan maksimal 150 MB.
- Target ukuran setelah instalasi: diupayakan maksimal 250 MB.

Ukuran tersebut adalah sasaran desain yang harus diuji terhadap konten audio nyata. Target Android didasarkan pada [persyaratan sistem Unity 6.6](https://docs.unity.com/ja-jp/engine/6000.6/manual/get-started/install-and-upgrade/getting-started-installing-unity/system-requirements) dan [kebijakan target API Google Play](https://support.google.com/googleplay/android-developer/answer/11926878?hl=en-en).

### 5.6 Batasan distribusi

Distribusi dilakukan secara bertahap:

1. Pembuatan dan pengujian APK Android.
2. Pengujian pada perangkat Android nyata.
3. Pengujian layanan backend, audio, sinkronisasi, dan fitur AI.
4. Persiapan publikasi di Google Play Store setelah seluruh persyaratan kelayakan terpenuhi.

### 5.7 Batasan privasi dan keamanan

- Pengguna harus diberi penjelasan sebelum audio pertama kali dikirim ke layanan AI.
- Rekaman tidak disimpan sebagai riwayat audio oleh Uab Meto.
- Dashboard admin harus dilindungi autentikasi serta kontrol akses.
- Data progres dan hasil ujian tidak boleh dapat diubah secara bebas oleh klien.
- Penghapusan akun harus membersihkan data terkait sesuai kebijakan retensi yang berlaku.
- Kebijakan privasi harus menjelaskan data yang dikumpulkan, tujuan pemrosesan, dan penggunaan layanan pihak ketiga.

Rancangan privasi perlu mengacu pada [UU Nomor 27 Tahun 2022 tentang Pelindungan Data Pribadi](https://jdih.komdigi.go.id/produk_hukum/view/id/832/t/crc32/) dan persyaratan [penghapusan akun Google Play](https://support.google.com/googleplay/android-developer/answer/13327111?hl=en).

---

## 6. KRITERIA PERMINTAAN (ACCEPTANCE CRITERIA)

### 6.1 Autentikasi

- [ ] Pengguna dapat mendaftar dan login dengan email serta kata sandi.
- [ ] Pengguna dapat login melalui Google.
- [ ] Pendaftaran dengan email mewajibkan verifikasi.
- [ ] Pengguna dapat memulihkan kata sandi melalui email.
- [ ] Aplikasi meminta konfirmasi bahwa pengguna berusia minimal 16 tahun.
- [ ] Login online pertama diperlukan sebelum penggunaan offline.
- [ ] Sesi login dapat digunakan kembali secara aman ketika aplikasi dibuka tanpa internet.
- [ ] Pengguna dapat menghapus akun beserta data terkait melalui aplikasi.

### 6.2 Materi dan jalur belajar

- [ ] Dua belas topik pembelajaran tersedia sesuai cakupan awal.
- [ ] Target awal materi adalah 36 pelajaran, dengan jumlah final menyesuaikan validasi konten.
- [ ] Materi mencakup membaca, menulis, mendengarkan, dan berbicara.
- [ ] Setiap pelajaran memuat materi, contoh penggunaan, serta latihan yang relevan.
- [ ] Audio berasal dari rekaman yang sesuai dan telah diverifikasi.
- [ ] Materi yang belum divalidasi tidak dapat diterbitkan.
- [ ] Pelajaran berikutnya terbuka setelah pelajaran sebelumnya selesai.

### 6.3 Fitur AI

- [ ] Tutor AI dapat menjawab pertanyaan terkait materi Bahasa Dawan dan budaya Dawan yang tervalidasi.
- [ ] Penjelasan menggunakan Bahasa Indonesia.
- [ ] Tutor dapat mengoreksi jawaban dan menjelaskan alasan koreksi berdasarkan materi.
- [ ] Percakapan AI mendukung teks dan masukan suara.
- [ ] Balasan audio Bahasa Dawan hanya menggunakan rekaman terverifikasi yang sesuai.
- [ ] Pembelajaran adaptif memberikan latihan penguatan berdasarkan kesalahan dan penguasaan pengguna.
- [ ] AI mengakui ketika referensi tidak tersedia.
- [ ] Latihan pelafalan memungkinkan percobaan ulang.
- [ ] Analisis yang tidak meyakinkan ditandai belum dapat dinilai, bukan diberi skor palsu.
- [ ] Seluruh fitur AI melewati pengujian kualitas dan ketepatan Bahasa Dawan sebelum dirilis.

### 6.4 Kuota dan biaya

- [ ] Tutor AI dibatasi 10 pertanyaan per akun per hari.
- [ ] Percakapan AI dibatasi 3 sesi per akun per hari.
- [ ] Latihan pelafalan dibatasi 10 kali per akun per hari.
- [ ] Batas dihitung dan ditegakkan di server.
- [ ] Kuota aplikasi direset berdasarkan hari WITA.
- [ ] Pengguna dapat melihat kuota yang tersisa.
- [ ] Ketika kuota habis, fitur terkait berhenti sementara sampai reset berikutnya.
- [ ] Kegagalan layanan AI tidak mengurangi kuota pengguna sebagai penggunaan yang berhasil.
- [ ] Ketika kuota global layanan gratis habis, aplikasi menampilkan pesan yang sesuai dan tidak kehilangan progres pengguna.

### 6.5 Gamifikasi dan evaluasi

- [ ] XP hanya diberikan untuk aktivitas belajar yang memenuhi ketentuan.
- [ ] Level dan jalur belajar menunjukkan progres yang konsisten.
- [ ] Streak dihitung dari pelajaran atau paket latihan inti yang selesai dan menghasilkan XP.
- [ ] Lencana pencapaian diberikan sesuai persyaratan yang ditetapkan.
- [ ] Papan peringkat global dan mingguan tersedia.
- [ ] Papan peringkat hanya menampilkan nickname.
- [ ] Pengguna memulai dengan lima hati.
- [ ] Hati berkurang hanya pada jenis latihan inti yang ditentukan.
- [ ] Hati dapat pulih otomatis atau melalui latihan sesuai aturan yang ditampilkan.
- [ ] Ujian akhir menilai empat keterampilan.
- [ ] Kelulusan memerlukan nilai gabungan minimal 70 dan nilai minimal 50 pada setiap keterampilan.
- [ ] Pengguna yang belum lulus memperoleh rekomendasi materi perbaikan serta dapat mengulang ujian tanpa batas.
- [ ] Bagian berbicara yang belum dapat dinilai tidak digunakan sebagai dasar kelulusan yang tidak dapat dipercaya.

### 6.6 Offline dan sinkronisasi

- [ ] Setelah login online pertama, seluruh materi inti yang telah dipaketkan dapat digunakan tanpa internet.
- [ ] Audio yang telah disertakan di aplikasi dapat diputar secara offline.
- [ ] Latihan inti tetap dapat diselesaikan secara offline.
- [ ] Progres offline disimpan pada perangkat.
- [ ] Progres tersinkronisasi otomatis setelah koneksi tersedia.
- [ ] Sinkronisasi tidak menggandakan XP, hasil latihan, atau pencapaian.
- [ ] Materi dan audio dapat diperbarui tanpa membangun ulang APK.
- [ ] Pembaruan yang gagal tidak menghapus materi versi sebelumnya.
- [ ] Fitur AI tetap dinyatakan membutuhkan internet.

### 6.7 Dashboard admin

- [ ] Hanya satu akun admin utama yang dapat mengakses dashboard.
- [ ] Admin dapat mengelola jalur belajar, pelajaran, kosakata, dialog, audio, dan soal.
- [ ] Admin dapat memvalidasi dan menerbitkan materi.
- [ ] Admin dapat melihat nickname, email, status akun, progres, serta hasil ujian.
- [ ] Admin dapat melihat laporan statistik.
- [ ] Pengguna biasa tidak dapat mengakses fungsi administratif.
- [ ] Admin dapat memperbarui audio tanpa merilis APK baru.
- [ ] Rahasia layanan penyimpanan dan AI tidak terekspos dalam frontend dashboard atau APK.

### 6.8 Privasi

- [ ] Persetujuan pemrosesan suara ditampilkan sebelum penggunaan pertama fitur suara.
- [ ] Rekaman tidak disimpan sebagai riwayat audio oleh Uab Meto.
- [ ] Hasil analisis serta saran pelafalan dapat disimpan tanpa menyimpan rekaman audio.
- [ ] Pengguna dapat menghapus akun dan data terkait.
- [ ] Sesi serta cache lokal yang relevan dibersihkan ketika akun dihapus.
- [ ] Kebijakan privasi menjelaskan penggunaan Firebase, Cloudflare, Supabase, dan layanan AI yang dipilih.

### 6.9 UI dan performa

- [ ] Desain awal dibuat melalui Stitch dan diterjemahkan menjadi UI asli Unity.
- [ ] Seluruh menu dan penjelasan aplikasi menggunakan Bahasa Indonesia.
- [ ] Antarmuka mendukung portrait serta landscape.
- [ ] Tombol dan teks mudah dibaca pada layar ponsel.
- [ ] Aplikasi dapat dinavigasi dengan lancar pada perangkat uji berspesifikasi rendah.
- [ ] Target ukuran unduhan dan ukuran instalasi dievaluasi menggunakan build nyata.
- [ ] APK dapat dipasang dan diuji pada perangkat Android.
- [ ] Target API 36 atau lebih tinggi digunakan saat menyiapkan rilis Google Play.

---

## Penutup dan status PRD

PRD ini menjadi acuan kebutuhan produk Uab Meto versi pertama.

Implementasi harus menjaga tiga prioritas utama: **ketepatan materi Bahasa Dawan, akses belajar offline, dan keamanan layanan AI**.

Seluruh fitur yang telah ditetapkan ditargetkan tersedia pada peluncuran pertama. Namun, fitur yang bergantung pada layanan gratis atau kemampuan AI untuk memahami Bahasa Dawan harus melewati pengujian teknis sebelum dinyatakan siap dirilis.

Keputusan detail seperti lama regenerasi hati, batas giliran per sesi percakapan, pilihan model AI akhir, dan volume audio final harus ditetapkan melalui pengujian prototipe sebelum implementasi penuh.
