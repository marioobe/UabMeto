# Inventaris Layar & Alur Navigasi

Status: **F0.5 — dikunci untuk implementasi.** Menggantikan versi usulan F0.
Arah visual disetujui dari Stitch; rincian konten mengikuti
`design-review-stitch.md`.

Setiap layar wajib punya state `memuat`, `kosong`, `galat`, `offline`, dan
`sesi kedaluwarsa` bila relevan. Layar tanpa state tersebut dianggap belum
selesai.

## 1. Navigasi utama

Empat tab bawah, tinggi 64 dp + safe area:

| Tab | Ikon | Isi |
|---|---|---|
| Beranda | `cottage` | jalur belajar, pelajaran berikutnya, XP, streak, hati |
| Tutor AI | `record_voice_over` | tutor, percakapan, pelafalan |
| Peringkat | `military_tech` | mingguan dan sepanjang waktu |
| Profil | `person` | progres, lencana, kuota, pengingat, akun |

Ikon dan label harus sama di semua layar. Tab aktif memakai `accent` dan
label tebal; tab pasif memakai `text-muted`.

Di luar tab: autentikasi, pelajaran, ujian akhir, modal, dan halaman web.

## 2. Alur

```
Splash
  ├─ sesi valid ──────────────► Beranda
  ├─ sesi kedaluwarsa ────────► Masuk
  └─ tanpa sesi ──────────────► Masuk
Masuk ──► Daftar ──► Verifikasi email ──► Masuk
Masuk ──► Lupa kata sandi ──► Verifikasi email terkirim
Beranda ──► Pembuka pelajaran ──► Materi ──► Latihan ──► Umpan balik
        └──► Ringkasan ──► Beranda
Beranda ──► Ujian akhir ──► 4 bagian ──► Hasil ──► Rekomendasi
Tutor AI ──► Percakapan / Pelafalan ──► Persetujuan suara (sekali)
Profil ──► Pengingat / Hapus akun / Kebijakan privasi
```

## 3. Model progres

Tiga tingkat, satu-satunya model yang dipakai UI:

| Tingkat | Isi | Selesai bila |
|---|---|---|
| Topik | 12 topik PRD §3.2 | semua pelajaran terbit selesai |
| Pelajaran | 3 per topik (target) | semua langkah wajib selesai |
| Langkah | materi, latihan, ringkasan | langkah ditandai selesai |

Aturan:

- Pelajaran berikutnya terbuka setelah pelajaran sebelumnya selesai.
- Tidak ada nilai minimum untuk membuka pelajaran berikutnya.
- Progres topik = pelajaran selesai / pelajaran terbit.
- Bilah progres pelajaran = langkah selesai / langkah wajib.
- XP, durasi, dan jumlah kosakata berasal dari JSON, bukan dari UI.

Label di layar harus menyebut tingkatnya secara eksplisit:
"Topik 3 · Pelajaran 2 · Langkah 1 dari 4". Jangan mencampur "bagian",
"pelajaran", dan "langkah" untuk hal yang sama.

## 4. Autentikasi

| ID | Layar | Isi | State wajib |
|---|---|---|---|
| AUTH-01 | Splash | logo, cek sesi | memuat, sesi valid, sesi kedaluwarsa |
| AUTH-02 | Masuk | email, kata sandi, Google, lupa sandi, daftar | kosong, salah sandi, offline tanpa sesi, offline dengan sesi, email belum verifikasi, terkunci sementara |
| AUTH-03 | Daftar | email, sandi, konfirmasi, nickname, usia 16+, setuju ketentuan | validasi, email terdaftar, sandi lemah |
| AUTH-04 | Verifikasi email | instruksi, kirim ulang, ganti email | menunggu, terverifikasi, gagal kirim, batas kirim |
| AUTH-05 | Lupa kata sandi | email | terkirim, email tidak dikenal (pesan netral) |
| AUTH-06 | Persetujuan suara (modal) | penjelasan pengiriman audio, pihak ketiga, tombol setuju/tolak | belum setuju, setuju, ditolak |

Aturan:

- Login online pertama wajib. Setelah sesi tersimpan, aplikasi boleh dibuka
  offline tanpa melewati Masuk.
- Bila perangkat offline dan sesi valid: masuk langsung ke Beranda dengan
  banner offline.
- Bila perangkat offline dan tidak ada sesi: tampilkan state terkunci,
  jangan berpura-pura bisa masuk.
- "Ingat perangkat ini" tidak dipakai. Sesi selalu disimpan aman; tidak ada
  pilihan yang membuat pengguna kehilangan akses offline.
- Verifikasi email memblokir fitur akun, bukan materi offline.
- Checkbox 16+ dan ketentuan tidak boleh tercentang otomatis.
- Nickname wajib saat daftar: 3–20 karakter, tanpa email, tanpa karakter
  kendali. Keunikan tidak diwajibkan.

## 5. Beranda

| ID | Layar | Isi | State wajib |
|---|---|---|---|
| HOME-01 | Beranda | sapaan + nickname, XP, level, streak, hati, pelajaran berikutnya, jalur 12 topik | memuat, kosong (belum ada materi terbit), offline, menunggu sinkron, gagal sinkron |

Aturan:

- Jalur menampilkan 12 topik PRD, termasuk topik yang belum punya pelajaran
  terbit (status "belum tersedia", bukan "terkunci").
- Kartu topik: terkunci, aktif, selesai, belum tersedia.
- Banner offline dan antrean sinkron memakai data nyata dari outbox.
- Jangan menampilkan "tersinkron" tanpa hasil sinkron terakhir.
- "Lanjutkan belajar" hanya muncul bila ada pelajaran aktif.

## 6. Pelajaran

| ID | Layar | Isi | State wajib |
|---|---|---|---|
| LEARN-01 | Pembuka | judul, tujuan, estimasi, XP, syarat hati, mulai | audio tidak tersedia, offline, terkunci |
| LEARN-02 | Kosakata | kartu kata, arti, catatan, audio | audio tersedia, audio tidak tersedia |
| LEARN-03 | Dialog | baris per penutur, terjemahan, audio | audio tidak tersedia |
| LEARN-04 | Penjelasan | catatan penggunaan, rujukan | rujukan tidak tersedia |
| LEARN-05 | Latihan | satu soal per layar | 6 jenis latihan |
| LEARN-06 | Umpan balik | benar/salah, alasan, hati | benar, salah, belum dapat dinilai |
| LEARN-07 | Ringkasan | XP, streak, akurasi, kosakata, outbox | tersimpan lokal, tersinkron |

Jenis latihan dan state:

| Jenis | State khusus |
|---|---|
| `multiple_choice` | terpilih, benar, salah |
| `match` | sebagian cocok, semua cocok, salah |
| `sentence_order` | token terpakai, urutan salah |
| `listening` | audio diputar, audio gagal |
| `writing` | kosong, dinilai, tidak dapat dinilai otomatis |
| `speaking` | izin ditolak, merekam, menganalisis, skor, belum dapat dinilai |

Aturan:

- Hati berkurang hanya pada `multiple_choice` dan `sentence_order`.
- Membaca materi, Tutor AI, dan pelafalan tidak mengurangi hati.
- `speaking` tidak pernah menghasilkan lulus/gagal.
- Bila audio tidak ada: tampilkan teks dan penjelasan, tanpa audio sintetis.
- Bila analisis tidak yakin: tampilkan "Belum dapat dinilai", tanpa skor.
- Semua hasil latihan masuk outbox lokal lebih dulu.

## 7. Ujian akhir

| ID | Layar | Isi | State wajib |
|---|---|---|---|
| EXAM-01 | Pembuka | aturan, 4 bagian, syarat, mulai | terkunci, siap |
| EXAM-02 | Membaca | soal | — |
| EXAM-03 | Menulis | soal | — |
| EXAM-04 | Mendengarkan | audio + soal | audio gagal |
| EXAM-05 | Berbicara | rekam | izin ditolak, belum dapat dinilai |
| EXAM-06 | Hasil | gabungan, 4 nilai, lulus/tidak | lulus, tidak lulus, sebagian belum dapat dinilai |
| EXAM-07 | Rekomendasi | materi perbaikan, ulangi | — |

Aturan:

- Terbuka setelah seluruh pelajaran yang diterbitkan selesai. Bila jalur
  belum lengkap, ujian terkunci dan menampilkan alasannya.
- Lulus: gabungan ≥ 70 dan setiap keterampilan ≥ 50.
- Bagian berbicara yang belum dapat dinilai tidak dihitung sebagai lulus
  atau gagal; hasil menampilkan catatan terpisah.
- Percobaan ulang tanpa batas.
- Ujian bukan topik dan tidak muncul di jalur 12 topik.

## 8. Tutor AI

| ID | Layar | Isi | State wajib |
|---|---|---|---|
| AI-01 | Tutor | percakapan teks, rujukan materi, pertanyaan contoh, sisa kuota | memuat, kuota habis, layanan tidak tersedia, referensi tidak ditemukan |
| AI-02 | Percakapan | mode teks/suara, indikator sesi, kuota, giliran | izin mikrofon ditolak, sesi habis, offline |
| AI-03 | Pelafalan | target kata, rekam, analisis, hasil | izin ditolak, menganalisis, skor, belum dapat dinilai, kuota habis |

Aturan:

- Kuota dari Worker: Tutor 10, Percakapan 3, Pelafalan 10 per hari WITA.
- Kuota gagal tidak mengurangi jatah.
- Balasan audio hanya dari rekaman terverifikasi.
- Nama model AI tidak ditampilkan.
- Percakapan teks tetap tersedia tanpa izin mikrofon.
- Persetujuan suara (AUTH-06) muncul sebelum mikrofon dipakai pertama kali.

## 9. Peringkat

| ID | Layar | Isi | State wajib |
|---|---|---|---|
| RANK-01 | Peringkat | tab Mingguan dan Sepanjang Waktu, posisi pengguna | memuat, kosong, offline (data terakhir), gagal |

Aturan:

- Hanya nickname dan XP. Tidak ada email, nama asli, atau divisi.
- Tidak ada struktur sosial karangan ("Divisi Sonbai").
- Reset mingguan: Senin 00.00 WITA, dapat dikonfigurasi.
- Bila offline, tampilkan data terakhir dengan label waktu.

## 10. Profil

| ID | Layar | Isi | State wajib |
|---|---|---|---|
| PROF-01 | Profil | nickname, progres 4 keterampilan, lencana, kuota, pengingat, privasi, akun | memuat, offline, gagal muat |
| PROF-02 | Pengingat (modal) | aktif/nonaktif, waktu, hari | izin notifikasi ditolak |
| PROF-03 | Hapus akun (modal) | peringatan, konfirmasi ganda, autentikasi ulang | salah sandi, gagal |
| PROF-04 | Kebijakan privasi | halaman web | offline |

Aturan:

- Lencana memakai nama netral, bukan gelar adat.
- Kuota menampilkan angka dari Worker, bukan hardcode.
- Ukuran cache dihitung dari berkas nyata.
- Versi aplikasi dari build metadata.
- Hapus akun: dua langkah konfirmasi, autentikasi ulang, lalu bersihkan
  sesi dan cache.
- Persetujuan suara dapat dicabut; pencabutan menghentikan fitur suara.

## 11. State lintas layar

| State | Perilaku |
|---|---|
| Memuat | skeleton atau spinner, tanpa menggeser layout |
| Kosong | pesan + tindakan lanjutan |
| Galat | pesan penyebab + coba lagi |
| Offline | banner tipis, fitur jaringan dinonaktifkan dengan alasan |
| Kuota habis | panel khusus, waktu reset, materi tetap bisa dipelajari |
| Sesi kedaluwarsa | modal, arahkan ke Masuk, data lokal tetap aman |
| Konten belum terbit | status "belum tersedia", bukan "terkunci" |

Matriks offline:

| Fitur | Offline |
|---|---|
| Beranda | ya, data lokal |
| Pelajaran dan latihan | ya |
| Audio bawaan | ya |
| Ringkasan dan outbox | ya |
| Tutor, percakapan, pelafalan | tidak |
| Peringkat | data terakhir |
| Profil dan pengingat | ya, kecuali hapus akun |

## 12. Landscape

Tidak ada mockup landscape. Spesifikasi turunan berikut mengikat
implementasi:

| Layar | Portrait | Landscape |
|---|---|---|
| Beranda | satu kolom | jalur kiri 40%, ringkasan kanan 60% |
| Pelajaran materi | satu kolom | materi kiri 40%, contoh/audio kanan 60% |
| Latihan | satu kolom | soal kiri 40%, jawaban kanan 60% |
| Ujian | satu kolom | navigasi soal kiri, soal kanan |
| Tutor AI | satu kolom | daftar sesi kiri, percakapan kanan |
| Peringkat | satu kolom | papan kiri, posisi pengguna kanan |
| Profil | satu kolom | daftar bagian kiri, detail kanan |

Aturan: teks isi tidak di bawah 14 sp; metadata boleh 12 sp menurut gaya
`label-caps`/`label-chip` di `design-tokens.md` §3; tidak ada kontrol yang
terpotong; safe area tetap dihormati; bottom bar tetap 4 tab.

## 13. Overlay debug

Panel "pratinjau status", "simulasi jaringan", dan pengalih state hanya
boleh ada pada `DEVELOPMENT_BUILD`. Tidak ada di APK rilis.

## 14. Di luar aplikasi Android

- Dashboard admin: aplikasi web terpisah.
- Kebijakan privasi: halaman web, URL nyata, bukan `#`.
