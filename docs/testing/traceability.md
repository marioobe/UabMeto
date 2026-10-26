# Matriks Traceability PRD → Verifikasi

Status: **F0.** Sebagian besar butir belum dapat diuji karena fitur belum
diimplementasikan. Tidak ada status `PASS` kecuali yang benar-benar dijalankan.

## Legenda status

| Status | Arti |
|---|---|
| `PASS` | Sudah dijalankan dan lulus |
| `FAIL` | Sudah dijalankan dan gagal |
| `NOT RUN` | Belum dijalankan |
| `BLOCKED` | Terhalang prasyarat (akun, perangkat, konten) |
| `N/A` | Tidak berlaku untuk tahap ini |

ID: `AC-<bagian PRD>.<nomor>`.

## 6.1 Autentikasi

| ID | Persyaratan | Verifikasi | Status |
|---|---|---|---|
| AC-6.1.1 | Daftar & login email + kata sandi | PlayMode + device | NOT RUN |
| AC-6.1.2 | Login Google | PlayMode + device | NOT RUN |
| AC-6.1.3 | Pendaftaran email wajib verifikasi | PlayMode | NOT RUN |
| AC-6.1.4 | Pemulihan kata sandi via email | Manual | NOT RUN |
| AC-6.1.5 | Konfirmasi usia minimal 16 | PlayMode | NOT RUN |
| AC-6.1.6 | Login online pertama sebelum offline | PlayMode | NOT RUN |
| AC-6.1.7 | Sesi dapat dipakai ulang tanpa internet | PlayMode | NOT RUN |
| AC-6.1.8 | Hapus akun + data terkait | PlayMode + Worker test | NOT RUN |

## 6.2 Materi & jalur belajar

| ID | Persyaratan | Verifikasi | Status |
|---|---|---|---|
| AC-6.2.1 | 12 topik tersedia | Validator + review konten | BLOCKED (belum ada korpus) |
| AC-6.2.2 | Target 36 pelajaran | Review konten | BLOCKED |
| AC-6.2.3 | Mencakup 4 keterampilan | Validator + review konten | NOT RUN (aturan skema ada) |
| AC-6.2.4 | Setiap pelajaran memuat materi, contoh, latihan | Validator + review konten | NOT RUN (aturan skema ada) |
| AC-6.2.5 | Audio berasal dari rekaman terverifikasi | Review + gate validasi | NOT RUN |
| AC-6.2.6 | Materi belum tervalidasi tidak dapat diterbitkan | Unit test validator + EditMode | PASS (tool level) |
| AC-6.2.7 | Pelajaran berikutnya terbuka setelah selesai | EditMode + PlayMode | NOT RUN |

## 6.3 Fitur AI

| ID | Persyaratan | Verifikasi | Status |
|---|---|---|---|
| AC-6.3.1 | Tutor menjawab dari materi tervalidasi | Evaluasi terarah | BLOCKED (F4) |
| AC-6.3.2 | Penjelasan Bahasa Indonesia | Evaluasi | BLOCKED |
| AC-6.3.3 | Koreksi + alasan berbasis materi | Evaluasi | BLOCKED |
| AC-6.3.4 | Percakapan teks dan suara | PlayMode + device | BLOCKED |
| AC-6.3.5 | Balasan audio hanya rekaman terverifikasi | Code review + test | BLOCKED |
| AC-6.3.6 | Latihan adaptif berdasarkan kesalahan | EditMode + evaluasi | BLOCKED |
| AC-6.3.7 | AI mengakui referensi tidak tersedia | Evaluasi | BLOCKED |
| AC-6.3.8 | Pelafalan dapat dicoba ulang | PlayMode | BLOCKED |
| AC-6.3.9 | Analisis tidak meyakinkan → belum dapat dinilai | Unit test kontrak | BLOCKED |
| AC-6.3.10 | Seluruh fitur AI lolos uji kualitas sebelum rilis | Gate rilis | BLOCKED |

## 6.4 Kuota dan biaya

| ID | Persyaratan | Verifikasi | Status |
|---|---|---|---|
| AC-6.4.1 | Tutor 10 pertanyaan/hari | Worker test | NOT RUN |
| AC-6.4.2 | Percakapan 3 sesi/hari | Worker test | NOT RUN |
| AC-6.4.3 | Pelafalan 10 kali/hari | Worker test | NOT RUN |
| AC-6.4.4 | Batas dihitung & ditegakkan server | Worker test | NOT RUN |
| AC-6.4.5 | Reset 00.00 WITA | Worker test (fake clock) | NOT RUN |
| AC-6.4.6 | Pengguna melihat sisa kuota | PlayMode | NOT RUN |
| AC-6.4.7 | Kuota habis → fitur berhenti sementara | Worker + PlayMode | NOT RUN |
| AC-6.4.8 | Kegagalan AI tidak mengurangi kuota | Worker test | NOT RUN |
| AC-6.4.9 | Kuota global habis → pesan jelas, progres aman | Worker test | NOT RUN |

## 6.5 Gamifikasi & evaluasi

| ID | Persyaratan | Verifikasi | Status |
|---|---|---|---|
| AC-6.5.1 | XP hanya untuk aktivitas memenuhi ketentuan | EditMode | NOT RUN |
| AC-6.5.2 | Level & jalur konsisten | EditMode | NOT RUN |
| AC-6.5.3 | Streak dari pelajaran/paket selesai ber-XP | EditMode | NOT RUN |
| AC-6.5.4 | Lencana sesuai syarat | EditMode | NOT RUN |
| AC-6.5.5 | Peringkat global & mingguan | Worker + PlayMode | NOT RUN |
| AC-6.5.6 | Peringkat hanya nickname | Code review + UI test | NOT RUN |
| AC-6.5.7 | Mulai dengan 5 hati | EditMode | NOT RUN |
| AC-6.5.8 | Hati berkurang hanya pada latihan tertentu | EditMode | NOT RUN |
| AC-6.5.9 | Hati pulih otomatis/lewat latihan | EditMode | NOT RUN |
| AC-6.5.10 | Ujian menilai 4 keterampilan | EditMode + PlayMode | NOT RUN |
| AC-6.5.11 | Lulus: total ≥70, tiap skill ≥50 | EditMode | NOT RUN |
| AC-6.5.12 | Gagal → rekomendasi + ulang tanpa batas | PlayMode | NOT RUN |
| AC-6.5.13 | Berbicara tak ternilai tidak menentukan lulus/gagal | EditMode | NOT RUN |

## 6.6 Offline & sinkronisasi

| ID | Persyaratan | Verifikasi | Status |
|---|---|---|---|
| AC-6.6.1 | Materi inti offline setelah login pertama | PlayMode + device | NOT RUN |
| AC-6.6.2 | Audio bawaan diputar offline | PlayMode + device | NOT RUN |
| AC-6.6.3 | Latihan inti selesai offline | PlayMode | NOT RUN |
| AC-6.6.4 | Progres offline tersimpan di perangkat | PlayMode | NOT RUN |
| AC-6.6.5 | Sinkron otomatis saat koneksi tersedia | PlayMode + Worker | NOT RUN |
| AC-6.6.6 | Sinkron tidak menggandakan XP/hasil/lencana | EditMode (`SyncReconcilerTests`) + Worker | PASS (logika keputusan) |
| AC-6.6.7 | Materi/audio diperbarui tanpa rebuild APK | Integrasi | NOT RUN |
| AC-6.6.8 | Update gagal → versi lama tetap | Integrasi | NOT RUN |
| AC-6.6.9 | Fitur AI membutuhkan internet | PlayMode | NOT RUN |

## 6.7 Dashboard admin

| ID | Persyaratan | Verifikasi | Status |
|---|---|---|---|
| AC-6.7.1 | Hanya satu akun admin utama | Worker test | NOT RUN |
| AC-6.7.2 | Kelola jalur, pelajaran, kosakata, dialog, audio, soal | E2E admin | NOT RUN |
| AC-6.7.3 | Validasi & terbitkan materi | E2E admin | NOT RUN |
| AC-6.7.4 | Lihat nickname, email, status, progres, hasil ujian | E2E admin | NOT RUN |
| AC-6.7.5 | Laporan statistik | E2E admin | NOT RUN |
| AC-6.7.6 | Pengguna biasa tidak dapat akses admin | Worker test | NOT RUN |
| AC-6.7.7 | Audio diperbarui tanpa APK baru | Integrasi | NOT RUN |
| AC-6.7.8 | Rahasia tidak terekspos di frontend/APK | Code review + scan | NOT RUN |

## 6.8 Privasi

| ID | Persyaratan | Verifikasi | Status |
|---|---|---|---|
| AC-6.8.1 | Persetujuan sebelum fitur suara pertama | PlayMode | NOT RUN |
| AC-6.8.2 | Rekaman tidak disimpan sebagai riwayat | Code review + Worker test | NOT RUN |
| AC-6.8.3 | Hasil analisis disimpan tanpa audio | Worker test | NOT RUN |
| AC-6.8.4 | Pengguna dapat menghapus akun + data | PlayMode + Worker | NOT RUN |
| AC-6.8.5 | Sesi & cache lokal dibersihkan saat hapus akun | PlayMode | NOT RUN |
| AC-6.8.6 | Kebijakan privasi menjelaskan Firebase, Cloudflare, Supabase, AI | Review dokumen | NOT RUN |

## 6.9 UI & performa

| ID | Persyaratan | Verifikasi | Status |
|---|---|---|---|
| AC-6.9.1 | Desain Stitch diterjemahkan ke UI Unity | Review desain + PlayMode | BLOCKED (mockup belum ada) |
| AC-6.9.2 | Seluruh menu Bahasa Indonesia | Review UI | NOT RUN |
| AC-6.9.3 | Portrait & landscape | PlayMode + device | NOT RUN |
| AC-6.9.4 | Tombol/teks mudah dibaca | Review aksesibilitas | NOT RUN |
| AC-6.9.5 | Lancar di perangkat rendah | Profiling device | BLOCKED (belum ada device) |
| AC-6.9.6 | Ukuran unduhan/instalasi terukur | Build nyata | BLOCKED (audio belum ada) |
| AC-6.9.7 | APK dapat dipasang & diuji di perangkat | Device test | BLOCKED (belum ada device) |
| AC-6.9.8 | Target API 36 saat rilis | Inspeksi manifest APK | NOT RUN |

## Ringkasan F0

- `PASS`: 2 butir (gate penerbitan 6.2.6; logika idempotensi 6.6.6).
  Catatan: 6.6.6 baru pada tingkat logika keputusan, bukan integrasi
  D1/Firestore nyata.
- `NOT RUN`: mayoritas fitur yang belum dibangun.
- `BLOCKED`: butir yang memerlukan akun layanan, perangkat, atau korpus.

Matriks ini diperbarui pada setiap gate.
