# Cakupan Topik

Status: **dikunci.** Daftar ini menggantikan daftar topik pada mockup Stitch.

Sumber: `PRD.md` §3.2. Registry mesin: `content/path/topics.json`.

## 1. Dua belas topik yang disetujui

| Urutan | ID | Judul | PRD §3.2 |
|---|---|---|---|
| 1 | `topic-01-salam` | Salam dan sapaan | 1 |
| 2 | `topic-02-perkenalan` | Perkenalan diri | 2 |
| 3 | `topic-03-keluarga` | Keluarga dan hubungan | 3 |
| 4 | `topic-04-angka-waktu-hari` | Angka, waktu, dan hari | 4 |
| 5 | `topic-05-aktivitas` | Aktivitas sehari-hari | 5 |
| 6 | `topic-06-makanan` | Makanan dan minuman | 6 |
| 7 | `topic-07-rumah` | Rumah dan benda sekitar | 7 |
| 8 | `topic-08-tempat-arah` | Tempat dan arah | 8 |
| 9 | `topic-09-belanja` | Belanja dan tawar-menawar | 9 |
| 10 | `topic-10-budaya` | Budaya dan adat Dawan | 10 |
| 11 | `topic-11-kesehatan-darurat` | Kesehatan dan keadaan darurat | 11 |
| 12 | `topic-12-sekolah-pekerjaan` | Sekolah, pekerjaan, dan kegiatan sosial | 12 |

Target perencanaan: 36 pelajaran, tiga per topik. Jumlah final mengikuti
ketersediaan materi tervalidasi. Target ini bukan janji rilis.

## 2. Topik yang dikeluarkan

| Topik di mockup | Alasan | Tindakan |
|---|---|---|
| Alam, Ladang & Hewan | Tidak ada di PRD §3.2 | tidak diimplementasikan |
| Cerita Tradisi & Ujian Akhir | Tidak ada di PRD; ujian bukan topik | ujian menjadi alur terpisah |
| Angka & Berhitung | Pecahan dari topik 4 | digabung kembali |
| Waktu, Hari & Cuaca | Pecahan dari topik 4; "cuaca" tidak ada di PRD | digabung kembali |
| Perkenalan & Keluarga | Penggabungan topik 2 dan 3 | dipisah kembali |

## 3. Aturan

1. Menambah, menghapus, atau mengganti urutan topik memerlukan perubahan
   PRD dan persetujuan pengguna. Tidak boleh lewat konten atau UI.
2. Ujian akhir tidak punya `topicId` dan tidak tampil di jalur 12 topik.
3. Topik tanpa pelajaran terbit tetap tampil di Beranda dengan status
   "belum tersedia", bukan disembunyikan.
4. Nama topik hanya dalam Bahasa Indonesia. Terjemahan Dawan untuk judul
   topik tidak dipakai sampai tervalidasi.
5. Varian bahasa tetap `TTU-Kefamenanu`.

## 4. Verifikasi

| Butir | Cara |
|---|---|
| 12 topik, urutan benar | tes membandingkan `topics.json` dengan daftar di §1 |
| Tidak ada topik terlarang | `grep` nama di aset dan konten |
| Ujian di luar jalur | tes skema: ujian tidak punya `topicId` |
| Topik kosong tampil | tes state Beranda |

Status verifikasi: **belum dijalankan.** Tidak ada kode UI pada tahap ini.
