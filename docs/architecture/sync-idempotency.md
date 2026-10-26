# Idempotency & Rekonsiliasi Sinkronisasi

Status: **desain F0 + logika keputusan yang diuji.**
Implementasi backend menyusul di F3.

## 1. Mengapa unique constraint saja tidak cukup

`operationId` unik di D1 mencegah dua baris tercatat. Namun efek sebenarnya
ditulis ke **Firestore**, sistem terpisah. Urutan yang mungkin terjadi:

```
Tulis D1 (reserved)  ──✅──▶  Tulis Firestore  ──✅──▶  Konfirmasi D1
                     │                         │
                     └── proses mati           └── proses mati
```

Jika proses mati setelah Firestore tertulis tetapi sebelum D1 dikonfirmasi,
mengulang operasi akan menggandakan XP kecuali server memeriksa penanda efek
di Firestore. Karena itu alurnya memakai **dua penanda**:

1. D1: `operationId` + status (`reserved`, `confirmed`).
2. Firestore: penanda `appliedOperations/{operationId}` di dokumen pengguna
   (atau subkoleksi), ditulis dalam transaksi yang sama dengan efeknya.

## 2. Status operasi

| Status | Arti | Boleh retry? |
|---|---|---|
| `Pending` | Klien masih menyimpan di outbox | Ya (kirim) |
| `Reserved` | D1 mencatat opId, efek Firestore belum tentu ada | Ya (cek efek) |
| `Confirmed` | Efek Firestore terverifikasi + D1 dikonfirmasi | Tidak perlu |
| `Rejected` | Payload tidak valid | Tidak otomatis |
| `Failed` | Gagal permanen / percobaan habis | Manual |

## 3. Matriks keputusan

Diimplementasikan di `UabMeto.Core.Sync.SyncReconciler` dan diuji di
`SyncReconcilerTests`.

| Status D1 | Efek Firestore | Keputusan |
|---|---|---|
| Pending | tidak ada | `ReserveThenApply` |
| Pending | ada | `MarkConfirmed` (D1 hilang, efek ada) |
| Reserved | tidak ada | `ApplyToFirestore` (jendela 1) |
| Reserved | ada | `MarkConfirmed` (jendela 2) |
| Confirmed | ada | `ReturnCachedResult` (duplikat) |
| Confirmed | tidak ada | `HaltForReconciliation` |
| apa pun | ada, payload beda | `HaltForReconciliation` |
| payload invalid | — | `Reject` |
| attempts ≥ max | — | `Exhausted` |

## 4. Jendela kegagalan yang diuji

1. **Setelah D1 reserved, sebelum Firestore.** Retry melihat `Reserved` +
   tanpa efek → menerapkan sekali. XP tidak dobel.
2. **Setelah Firestore, sebelum konfirmasi D1.** Retry melihat `Reserved` +
   efek ada → hanya konfirmasi. XP tidak dobel.
3. **Pengiriman ganda setelah konfirmasi.** Mengembalikan hasil tersimpan.

Pengujian ada di `app/Assets/_Project/Tests/EditMode/SyncReconcilerTests.cs`
dan dijalankan melalui Unity Test Framework.

## 5. Penulisan Firestore yang idempotent

Setiap operasi membawa `operationId`. Worker menjalankan transaksi:

1. Baca dokumen pengguna + penanda `appliedOperations/{operationId}`.
2. Jika penanda ada:
   - payload hash sama → kembalikan hasil tersimpan;
   - payload hash berbeda → tolak (409) dan tandai rekonsiliasi.
3. Jika belum ada: terapkan efek (XP, streak, hasil) **dan** tulis penanda
   dalam transaksi yang sama.
4. Setelah commit, konfirmasi D1.

Karena penanda dan efek berada di satu transaksi Firestore, tidak ada kondisi
"efek tertulis tanpa penanda" di dalam Firestore.

## 6. Retry

- Backoff eksponensial + jitter, maksimum 8 percobaan.
- `Rejected` tidak di-retry otomatis.
- `Exhausted` masuk daftar rekonsiliasi manual.
- Klien menyimpan outbox secara persisten; aplikasi ditutup tidak menghapus
  operasi.

## 7. Rekonsiliasi

Pekerjaan terjadwal (manual atau cron) memeriksa:

- D1 `reserved` yang lebih tua dari N menit → cek efek Firestore, lalu
  konfirmasi atau tandai gagal.
- Firestore memiliki penanda tetapi D1 tidak → buat catatan `confirmed`.
- Efek tanpa penanda (seharusnya tidak terjadi) → tandai anomali, jangan
  menulis ulang otomatis.

Semua tindakan rekonsiliasi dicatat dengan alasan agar dapat diaudit.

## 8. Batasan yang diakui

- Sistem ini memberikan **efek exactly-once**, bukan pengiriman exactly-once.
- Jika Firestore tidak dapat dibaca saat rekonsiliasi, operasi ditahan, bukan
  diterapkan buta.
- Kuota di D1 dan progres di Firestore tidak berada dalam satu transaksi.
  Kegagalan di antaranya tidak boleh mengurangi kuota untuk operasi yang gagal
  (lihat acceptance criteria PRD 6.4).
