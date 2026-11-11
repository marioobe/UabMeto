# Arsitektur Uab Meto

Status: **F0 — desain, belum ada integrasi layanan produksi.**
Dokumen ini menjelaskan pembagian tanggung jawab dan alasan di baliknya.
Keputusan rinci ada di `docs/adr/`.

## 1. Gambaran komponen

```
┌─────────────────────────────┐
│  Aplikasi Android (Unity 6) │
│  - UI Canvas + C#           │
│  - konten offline (JSON+ogg)│
│  - outbox progres lokal     │
└──────────────┬──────────────┘
               │ HTTPS (UnityWebRequest)
               ▼
┌─────────────────────────────┐        ┌──────────────────────┐
│  Cloudflare Worker          │───────▶│  Firebase Auth        │
│  - verifikasi ID token      │        │  (email, Google)      │
│  - kuota harian (D1)        │        └──────────────────────┘
│  - validasi & sinkronisasi  │
│  - proxy AI                 │        ┌──────────────────────┐
│  - manifest konten          │───────▶│  Firestore (Spark)    │
└───────┬─────────────┬───────┘        │  profil, progres,     │
        │             │                │  XP, hasil ujian      │
        ▼             ▼                └──────────────────────┘
┌──────────────┐ ┌──────────────┐
│  D1 (SQLite) │ │ Workers AI   │
│  kuota,      │ │ tutor, ASR   │
│  idempotency,│ └──────────────┘
│  leaderboard │
└──────────────┘
        │
        ▼
┌──────────────────────────────┐
│ Supabase Storage (audio)      │
└──────────────────────────────┘

┌──────────────────────────────┐
│ Dashboard admin (web)         │──▶ Worker (allowlist admin)
└──────────────────────────────┘
```

## 2. Prinsip

1. **Konten adalah sumber kebenaran.** Semua layar dan latihan digerakkan
   skema JSON tervalidasi. Tidak ada materi yang di-hardcode di C#.
2. **Local-first.** Progres ditulis ke perangkat lebih dulu; server menerima
   operasi, bukan nilai akhir yang dipercaya.
3. **Server berwenang.** XP, hati, kuota, dan hasil ujian hanya berubah lewat
   Worker setelah verifikasi identitas dan validasi payload.
4. **AI dibatasi rujukan.** Jawaban AI harus berasal dari korpus tervalidasi.
5. **Kegagalan tidak menghapus data.** Update konten gagal → versi lokal lama
   tetap dipakai. Sinkronisasi gagal → outbox dipertahankan.

## 3. Tanggung jawab klien (Unity)

- Menampilkan materi dari bundel konten lokal.
- Menjalankan latihan inti secara offline.
- Menyimpan outbox operasi (`operationId`, payload, status, attempt).
- Memutar audio lokal (Ogg Vorbis).
- Menangani sesi login (Firebase Auth SDK) dan cache sesi aman.
- Tidak menyimpan rahasia server dan tidak menulis langsung ke Firestore.

## 4. Tanggung jawab Worker

- Memverifikasi Firebase ID token pada setiap permintaan.
- Menegakkan kuota harian per akun (D1, reset 00.00 WITA).
- Memvalidasi payload operasi (skema, batas nilai, kepemilikan).
- Menulis ke Firestore dengan service account (lihat
  `worker-firestore-auth.md`).
- Mencatat `operationId` di D1 dan melakukan rekonsiliasi (lihat
  `sync-idempotency.md`).
- Menyajikan manifest konten terbit dan URL audio.
- Meneruskan permintaan AI dan menandai hasil yang tidak dapat dinilai.

## 5. Sumber kebenaran data

| Data | Sumber kebenaran | Penulis | Catatan |
|---|---|---|---|
| Identitas login | Firebase Auth | Klien (SDK resmi) | Worker hanya memverifikasi token |
| Profil: nickname, status akun, consent suara | Firestore | Worker | Klien tidak menulis langsung |
| Progres pelajaran, XP, level, streak, lencana | Firestore | Worker | Ditulis sebagai operasi terverifikasi |
| Hasil latihan & ujian | Firestore | Worker | Rekap per pengguna/hari untuk hemat kuota |
| Counter kuota harian | **D1** | Worker | Increment atomik |
| Log idempotency operasi | **D1** | Worker | `operationId` unik + status |
| Papan peringkat global/mingguan | **D1** (proyeksi) | Worker | Eventual, dapat dibangun ulang |
| Metadata konten & versi terbit | Firestore | Worker (publish) | Manifest dibaca klien |
| File audio | Supabase Storage | Worker (upload) | Publik-baca untuk audio terbit |
| Bundel konten | Supabase Storage / manifest | Worker | Cache lokal di perangkat |

**Aturan konsistensi:**

- D1 tidak pernah menjadi sumber kebenaran progres. Jika D1 hilang, progres
  tetap dapat direkonstruksi dari Firestore.
- Papan peringkat boleh tertinggal beberapa menit; sumber XP tetap Firestore.
- Rebuild proyeksi leaderboard bersifat idempotent.
- Kuota memakai D1 karena butuh increment atomik; Firestore tidak dipakai
  untuk counter ini.

## 6. Alur konten

```
content/ (JSON + audio)
   │  validator (tools/content-validator)
   ▼
draft → in_review → validated → published
   │
   ▼
manifest versi + unggah audio (Worker → Supabase)
   │
   ▼
klien: unduh manifest, verifikasi hash, simpan versi baru
       (jika gagal, versi lama tetap dipakai)
```

Konten berstatus `UNVALIDATED` hanya boleh berada di `content/samples/` dan
tidak pernah masuk manifest terbit.

## 7. Format audio

Baseline: **Ogg Vorbis (`.ogg`)**. Opus tidak dianggap kompatibel dengan
pemutaran native Unity sebelum diuji. Detail ada di `docs/content/audio-spec.md`
dan ADR-0007.

## 8. AI dan suara

- Tutor AI teks (0.3.0, UI diperbarui di 0.4.0) memakai retrieval BM25 di atas
  korpus tervalidasi;
  jawaban hanya dari konteks, dengan penolakan eksplisit bila referensi tidak
  ada. Vector search ditunda sampai korpus cukup besar.
- Provider lokal bawaan `mock`; provider Workers AI sudah disiapkan tetapi
  belum diuji tanpa kredensial. Lihat `docs/testing/tutor-ai-quality.md`.
- Pengenalan ucapan dan penilaian kualitas pelafalan adalah dua hal berbeda.
- Hasil yang tidak meyakinkan → `not_assessable` → UI "belum dapat dinilai".
- Tidak ada status lulus/gagal dari fitur pelafalan.

## 9. Admin

- Dashboard web terpisah.
- Otorisasi: allowlist UID admin yang diverifikasi di Worker. Custom claim
  didokumentasikan sebagai alternatif, bukan asumsi.
- Klien tidak pernah mengirim flag admin.
- Penerbitan terpisah dari penyuntingan.

## 10. Yang belum diputuskan

| Topik | Status | Diperlukan sebelum |
|---|---|---|
| Model AI akhir | Tertunda | F4 |
| Ambang keyakinan ASR | Tertunda | F4 (setelah uji kelayakan) |
| Regenerasi hati | Tertunda | F2 |
| Batas giliran sesi percakapan | Tertunda | F4 |
| Target bitrate audio final | Tertunda | setelah rekaman nyata |
| IL2CPP vs Mono untuk rilis | Tertunda (F0 pakai Mono) | sebelum rilis |
| Strategi signing keystore rilis | Tertunda | sebelum publikasi |
