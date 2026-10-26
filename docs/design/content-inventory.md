# Inventaris Konten Mockup yang Tidak Boleh Dipakai

Status: **semua butir OPEN.** Dokumen ini mencatat teks Dawan tanpa sumber
dan klaim/angka yang harus dihapus atau diganti sebelum implementasi.

Aturan: jangan menyalin teks dari sini ke kode atau konten. Placeholder
hanya untuk penandaan, bukan materi.

## 1. Delapan lokasi teks Dawan tanpa sumber

| ID | Layar | Isi yang ditemukan | Mengapa ditolak | Pengganti |
|---|---|---|---|---|
| DWN-01 | Masuk | peribahasa Dawan + terjemahan Indonesia | tanpa `sources`, `reviewedBy` | `[PLACEHOLDER-DWN-001]` |
| DWN-02 | Beranda | sapaan pembuka kepada pengguna | tanpa sumber; bentuk sapaan belum disepakati | `[PLACEHOLDER-DWN-010]` |
| DWN-03 | Pembuka pelajaran | cuplikan audio bilangan 1–3 dan label "kosakata baru" | angka dan pelafalan belum divalidasi | `[PLACEHOLDER-DWN-020]` |
| DWN-04 | Kartu kosakata | kata utama, transkripsi fonetis, klaim penekanan, contoh frasa + terjemahan, tip ungkapan | lima elemen tanpa sumber; fonetis dikarang | `[PLACEHOLDER-DWN-030]` … `[PLACEHOLDER-DWN-034]` |
| DWN-05 | Latihan pilihan ganda | kata uji, catatan penutur, nama penutur | tanpa sumber; atribusi orang nyata | `[PLACEHOLDER-DWN-040]`, `[PLACEHOLDER-SPEAKER]` |
| DWN-06 | Ringkasan pelajaran | daftar tiga kata yang "dipelajari" | daftar berasal dari materi tak tervalidasi | `[PLACEHOLDER-DWN-050]` … `[PLACEHOLDER-DWN-052]` |
| DWN-07 | Tutor AI | sapaan tamu, istilah rumah adat, istilah sirih-pinang, label bahasa | istilah budaya tanpa rujukan; berisiko salah makna | `[PLACEHOLDER-DWN-060]` … `[PLACEHOLDER-DWN-063]` |
| DWN-08 | Percakapan (Tutor AI) | dua baris dialog | tanpa sumber; nada dan konteks tidak diketahui | `[PLACEHOLDER-DWN-070]`, `[PLACEHOLDER-DWN-071]` |

Keputusan: **tidak ada teks Dawan yang dipertahankan dari mockup.** Semua
harus datang dari `content/lessons/*.json` berstatus `validated`.

## 2. Sepuluh klaim atau angka tanpa dasar

| ID | Layar | Klaim | Masalah | Pengganti |
|---|---|---|---|---|
| CLM-01 | Tutor AI | "Model: Uab Meto v2.4 (Aktif)" | tidak ada model khusus; versi karangan | hapus baris model |
| CLM-02 | Pelafalan | "Latensi 42ms" | tidak diukur | hapus; bila perlu, ukur nyata |
| CLM-03 | Profil | "Terverifikasi Komunitas" | fitur tidak ada di PRD | hapus |
| CLM-04 | Profil | "Tersinkron" | status tanpa hasil sinkron | ambil dari outbox |
| CLM-05 | Profil, Kosakata | "14.2 MB modul tersimpan" | angka karangan | hitung dari berkas |
| CLM-06 | Profil | "v1.2.0", "Sejak Jan 2025" | versi dan tanggal palsu | `Application.version`, tanggal build |
| CLM-07 | Tutor AI | "12 / 15 Tanya" | kuota PRD 10 | ambil dari Worker |
| CLM-08 | Beranda, Pembuka, Latihan | "+25 XP" dan "+15 XP" untuk pelajaran sama | dua nilai berbeda | `lesson.xp` |
| CLM-09 | Peringkat | "Divisi Sonbai" | struktur sosial karangan | hapus; hanya nickname + XP |
| CLM-10 | Beranda | "Gelar Adat: Penutur Fasih Meto" | gelar adat tidak ada di PRD | hapus |

## 3. Data contoh yang harus menjadi nilai runtime

Angka berikut bukan klaim produk, tetapi tetap tidak boleh di-hardcode:

| Contoh | Sumber seharusnya |
|---|---|
| 1.420 XP | Firestore |
| 7 hari streak | Firestore |
| 5/5 hati | Firestore |
| 85% progres topik | turunan progres |
| 88% skor | hasil analisis nyata |
| 1.435 / 2.890 peringkat | D1 projection |
| "4/5 sisa" | state hati |
| "3 kata baru" | `lesson.vocabulary.length` |
| "Langkah 1 dari 3" | `2 + exercises.length` |

## 4. Tindakan yang sudah diambil

| Tindakan | Bukti |
|---|---|
| Folder `approved` diganti `reference/` | `docs/design/reference/stitch-2026-10-26/` |
| README referensi menyatakan tidak final | `reference/stitch-2026-10-26/README.md` |
| Dokumen produk memakai placeholder | `screens.md`, `content-binding.md` |
| Registry topik tanpa teks Dawan | `content/path/topics.json` |
| Skeleton pelajaran placeholder-only | `content/samples/lesson-001-salam.UNVALIDATED.json` |

## 5. Yang belum bisa ditutup

- Penggantian teks Dawan nyata menunggu reviewer manusia dan sumber.
- Penghapusan klaim di mockup tidak mengubah gambar; gambar tetap
  referensi dan tidak boleh dipakai sebagai bukti produk.
- Verifikasi otomatis belum ada karena belum ada kode UI.
