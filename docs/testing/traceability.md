# Matriks Traceability PRD → Verifikasi

Status: **F1.** UI inti dan build F1 sudah dijalankan di Editor dan diuji
dasar pada perangkat fisik portrait; konten tervalidasi, alur pelajaran
rilis, landscape perangkat, dan fitur lanjutan belum. Tidak ada status
`PASS` kecuali yang benar-benar dijalankan. Hasil lengkap:
`docs/testing/f1-gate-result.md`; bukti perangkat:
`docs/testing/f1-device-evidence.md`.

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
| AC-6.2.1 | 12 topik tersedia | Validator + review konten | PASS (registry + UI F1); korpus pelajaran belum |
| AC-6.2.2 | Target 36 pelajaran | Review konten | BLOCKED (3/36 pelajaran Topik 1 tervalidasi) |
| AC-6.2.3 | Mencakup 4 keterampilan | Validator + review konten | NOT RUN (aturan skema ada; F1 teks saja) |
| AC-6.2.4 | Setiap pelajaran memuat materi, contoh, latihan | Validator + review konten | PASS (3 pelajaran Topik 1, validator 0 error) |
| AC-6.2.5 | Audio berasal dari rekaman terverifikasi | Review + gate validasi | NOT RUN (F4; belum ada audio) |
| AC-6.2.6 | Materi belum tervalidasi tidak dapat diterbitkan | Unit test validator + EditMode | PASS (tool level) |
| AC-6.2.7 | Pelajaran berikutnya terbuka setelah selesai | EditMode + PlayMode | PASS (alur 1→2→3 di perangkat) |

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
| AC-6.5.7 | Mulai dengan 5 hati | EditMode | NOT RUN (field data ada, UI/state belum) |
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
| AC-6.6.3 | Latihan inti selesai offline | PlayMode | PASS (F1: pelajaran selesai dengan Wi-Fi/data mati, hasil masuk antrean) |
| AC-6.6.4 | Progres offline tersimpan di perangkat | PlayMode | PASS (local store F1); sinkron server belum |
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
| AC-6.9.1 | Desain Stitch diterjemahkan ke UI Unity | Review desain + PlayMode | PASS (subset F1; bukan semua layar) |
| AC-6.9.2 | Seluruh menu Bahasa Indonesia | Review UI | PASS (subset F1) |
| AC-6.9.3 | Portrait & landscape | PlayMode + device | PASS (portrait terkunci); landscape perangkat BLOCKED (OEM portrait lock); D1-05 WAIVED keputusan pemilik |
| AC-6.9.4 | Tombol/teks mudah dibaca | Review aksesibilitas | PASS (12/12 kontras, target F1 ≥48, font sistem 1.0×–1.6× perangkat / 2.0× PlayMode); TalkBack NOT RUN |
| AC-6.9.5 | Lancar di perangkat rendah | Profiling device | NOT RUN (perangkat tersedia; profiling belum) |
| AC-6.9.6 | Ukuran unduhan/instalasi terukur | Build nyata | PASS (28.994.614 byte; audio belum ada) |
| AC-6.9.7 | APK dapat dipasang & diuji di perangkat | Device test | PASS (D1-01..D1-04, D1-06, D1-07; D1-05 WAIVED) |
| AC-6.9.8 | Target API 36 saat rilis | Inspeksi manifest APK | PASS (`aapt`: targetSdk 36) |

## Ringkasan F1 (diperbarui 2026-11-12, rilis 0.5.0)

- `PASS`: registry 12 topik, gate penerbitan, logika idempotensi, UI inti
  F1, alur tiga pelajaran Topik 1, outbox dan banner offline, build APK,
  kontras F1, target sentuh F1, font sistem 1.0×–1.6× di perangkat,
  validasi penutur, targetSdk 36, dan instalasi/peluncuran/back pada
  perangkat fisik portrait.
- `PASS DENGAN CATATAN`: skala font 2.0× (PlayMode saja), build tanpa
  warning kompilasi (3 warning IL2CPP informasional).
- `PARTIAL`: aksesibilitas penuh (TalkBack belum), sinkron pemulihan
  setelah offline, beberapa butir UI yang baru mencakup subset F1.
- `NOT RUN`: auth, hati, kuota, ujian, model Workers AI nyata, lima jenis
  latihan lain, sinkron backend produksi, profiling perangkat.
- `WAIVED`: D1-05 landscape perangkat, karena F1 dikunci portrait-only
  (keputusan pemilik 2026-11-12).
- `BLOCKED`: Topik 2–12 (31 pelajaran tersisa), audio/consent F4.

Perubahan F1:

- Shell empat tab, Beranda, Profil, alur pelajaran lima langkah, empty state,
  nickname lokal, progress/outbox lokal, banner antrean/offline, dan
  screenshot.
- Build `uabmeto-f1.apk` 28.994.614 byte (0.5.0, code 6); 0 error,
  3 warning IL2CPP informasional.
- EditMode 99/99; PlayMode 43/43; kontras 12/12; validator 36/36
  (0 error, 4 warning audio contoh).
- Uji perangkat menemukan dan memperbaiki scroll mati, kartu Profil
  bertumpuk, kartu modal bertumpuk, dan label statistik pada font besar.
- Gate F1 **LULUS DENGAN CATATAN**; rincian di
  `docs/testing/f1-gate-result.md`.

Matriks ini diperbarui pada setiap gate.
