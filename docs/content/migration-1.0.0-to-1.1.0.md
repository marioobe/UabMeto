# Migrasi Skema 1.0.0 → 1.1.0

Status: **berlaku.** Skema 1.1.0 disetujui 2026-10-26.

## 1. Yang berubah

| Perubahan | Sifat | Keterangan |
|---|---|---|
| `xp` | field baru, wajib | integer ≥ 0; nilai tampilan, bukan otoritas server |
| `estimatedMinutes` | field baru, wajib | integer 1–120; hanya tampilan |
| `examples[]` | field baru, wajib | contoh kalimat dengan sumber dan status |
| `schemaVersion` | naik | `1.0.0` → `1.1.0` |
| Gate penerbitan | lebih ketat | wajib 1.1.0, minimal satu contoh bersumber |

Tidak ada field lama yang dihapus atau diganti nama.

## 2. Prinsip

1. **Migrasi bukan validasi.** Membaca skema baru tidak berarti materi sudah
   benar atau sudah disetujui.
2. **Status tidak naik otomatis.** `draft` tetap `draft`; `validated` dan
   `published` harus diturunkan ke `in_review`.
3. **Tidak mengarang konten.** Migrasi tidak menambah kata, kalimat, atau
   terjemahan Dawan.
4. **Nilai netral wajib diganti.** `xp = 0` dan `estimatedMinutes = 1`
   adalah penanda, bukan nilai final.
5. **Review manusia tetap wajib.** Reviewer harus mengisi sumber, contoh,
   dan tanggal baru.

## 3. Langkah mekanis

```powershell
cd tools/content-validator
node src/migrate.mjs --in <berkas-1.0.0.json> --out <berkas-1.1.0.json>
```

Untuk konten `validated` atau `published`:

```powershell
node src/migrate.mjs --in <berkas.json> --out <berkas.json> --demote-to-in-review
```

Yang dilakukan tool:

1. Menyalin berkas; sumber tidak diubah.
2. Mengubah `schemaVersion` menjadi `1.1.0`.
3. Menambah `xp: 0` bila belum ada.
4. Menambah `estimatedMinutes: 1` bila belum ada.
5. Menambah `examples: []` bila belum ada.
6. Menurunkan status ke `in_review` bila diminta.
7. Menjalankan validator; keluar dengan kode 2 bila hasil belum valid.

## 4. Yang tidak dilakukan tool

- Tidak menambah contoh kalimat.
- Tidak mengisi `sourceRef`.
- Tidak mengubah `reviewedBy` atau `reviewedAt`.
- Tidak menaikkan status.
- Tidak menyalin `xp` dari klien atau dari UI.
- Tidak menghapus placeholder.

## 5. Langkah manusia setelah migrasi

1. Isi `xp` dengan nilai yang disepakati.
2. Isi `estimatedMinutes` dengan perkiraan nyata.
3. Tambahkan `examples[]`; setiap contoh wajib punya `sourceRef` dan
   `status`.
4. Periksa ulang seluruh teks Dawan terhadap sumber.
5. Isi `validation.reviewedBy` dan `reviewedAt` yang baru.
6. Jalankan `npm test` dan `node src/validate.mjs --root ../../content`.
7. Terbitkan hanya lewat gate.

## 6. Aturan status contoh

| Status pelajaran | Status contoh yang boleh |
|---|---|
| `UNVALIDATED` | hanya `UNVALIDATED` |
| `draft` / `in_review` | `UNVALIDATED`, `draft`, `in_review` |
| `validated` | `validated` atau `published` |
| `published` | `validated` atau `published` |

Pelajaran `validated`/`published` wajib punya minimal satu contoh.

## 7. XP dan server

`xp` di JSON adalah saran tampilan. Server menghitung hadiah dari versi
materi terbit. Klien tidak boleh mengirim nilai XP untuk mengubah hadiah.
Pengujian ada di `XpAwardPolicyTests`.

## 8. Rollback

Bila migrasi salah:

1. Kembalikan berkas dari Git.
2. Jangan menurunkan `schemaVersion` secara manual pada berkas yang sudah
   terbit.
3. Catat insiden di laporan gate.

## 9. Checklist per pelajaran

- [ ] `schemaVersion` = `1.1.0`.
- [ ] `xp` ≥ 0 dan disepakati.
- [ ] `estimatedMinutes` 1–120.
- [ ] `examples[]` minimal satu (untuk terbit).
- [ ] Setiap contoh punya `sourceRef` dan `status` yang konsisten.
- [ ] `reviewedBy` dan `reviewedAt` baru.
- [ ] Tidak ada placeholder.
- [ ] Validator lulus.
- [ ] Status tidak naik tanpa review.
