# Rencana F0 — Fondasi Lokal

Status: **dieksekusi.** Dokumen ini adalah acuan lingkup F0, bukan jadwal rilis.

## 1. Tujuan

Menyiapkan fondasi teknis yang dapat diuji tanpa akun layanan produksi:

- Git lokal, struktur direktori, proyek Unity.
- Skema konten, template, contoh `UNVALIDATED`, validator.
- Dokumentasi arsitektur, ADR, setup, keamanan.
- Arah visual awal + prompt Stitch.
- Matriks traceability PRD → verifikasi.
- Build APK lokal.

## 2. Batas

- Root kerja hanya `D:\01_Uab_Meto\`.
- Git lokal saja; tanpa remote, tanpa push.
- Tidak membuat akun layanan, deployment, atau unggahan eksternal.
- Tidak menyimpan kredensial produksi.
- Tidak mengarang materi Bahasa Dawan.
- Tidak mengklaim pengujian yang belum dijalankan.
- F1 tidak dimulai sebelum laporan F0 ditinjau.

## 3. Koreksi yang diterapkan

| Koreksi | Penerapan |
|---|---|
| Audio baseline Ogg Vorbis | Validator menolak selain `.ogg`; ADR-0007; `audio-spec.md` |
| Autentikasi Worker→Firestore | `worker-firestore-auth.md` (OAuth service account, secret, least privilege) |
| Idempotency lintas D1/Firestore | `sync-idempotency.md` + `SyncReconciler` + 12 unit test |
| Admin tanpa asumsi custom claim | ADR-0004 (allowlist UID di Worker) |
| Supabase auto-pause | ADR-0005 (risiko diterima + prosedur pemulihan) |
| ASR ≠ penilaian pelafalan | ADR-0006 + kontrak `not_assessable` |

## 4. Paket kerja

| Paket | Isi | Artefak |
|---|---|---|
| W1 | Git lokal, ignore, atribut, README | `.gitignore`, `.gitattributes`, `README.md` |
| W2 | Proyek Unity + konfigurasi Android + build script | `app/` |
| W3 | Skema, template, contoh, validator | `content/`, `tools/content-validator/` |
| W4 | Alur validasi manusia | `docs/content/validation-workflow.md` |
| W5 | Arsitektur & ADR | `docs/architecture/`, `docs/adr/` |
| W6 | Setup tanpa kredensial | `docs/setup/`, `*.example` |
| W7 | Traceability | `docs/testing/traceability.md` |
| W8 | Arah visual & prompt Stitch | `docs/design/` |
| W9 | Logika idempotensi | `Core/Sync/` + test |

## 5. Gate F0

| Gate | Kriteria | Bukti |
|---|---|---|
| G0.1 | Direktori diverifikasi sebelum menulis | Laporan |
| G0.2 | Git lokal aktif, tanpa remote, rahasia ter-ignore | `git remote -v` kosong, `git status` |
| G0.3 | Proyek Unity terkompilasi, EditMode test berjalan | Hasil test XML |
| G0.4 | Validator lulus pada contoh & menolak publish `UNVALIDATED` | `npm test` |
| G0.5 | APK terbangun lokal | Path + ukuran |
| G0.6 | Dokumentasi lengkap | Daftar file |
| G0.7 | Traceability dibuat | Matriks |
| D0 | Pengujian perangkat fisik | **Terpisah, belum dijalankan** |

## 6. Format laporan akhir

1. File/folder dibuat atau diubah.
2. Hasil build + lokasi + ukuran.
3. Hasil tes nyata, termasuk gagal/tidak dapat dijalankan.
4. Keputusan terkunci vs tertunda.
5. Risiko + tindakan berikutnya.

## 7. Estimasi

5–8 hari kerja. Ini perkiraan perencanaan, bukan tenggat yang dijanjikan.
