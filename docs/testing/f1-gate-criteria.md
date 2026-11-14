# Kriteria Gate F1

Status: **F1 selesai 2026-11-12 dengan catatan.** Hasil aktual ada di
`docs/testing/f1-gate-result.md`: seluruh kriteria F1 dan gate perangkat
dijalankan, D1-05 **WAIVED** (portrait-only), dan D1-06 terbukti sampai batas
slider perangkat 1.6× (2.0× hanya di PlayMode).

## 1. Prinsip

1. Gate build dan gate perangkat dipisah.
2. Tidak ada klaim lulus tanpa bukti yang dijalankan.
3. Konten `UNVALIDATED` tidak boleh muncul di build rilis.
4. Mockup bukan bukti produk.
5. Emulator bukan perangkat fisik.

## 2. Prasyarat sebelum F1 dimulai

| # | Syarat | Bukti |
|---|---|---|
| 1 | F0.5 dan daftar remediasi disetujui | `remediation.md` |
| 2 | Keputusan skema 1.1.0 (`xp`, `estimatedMinutes`, `examples[]`) | ADR atau catatan keputusan |
| 3 | Token dan font siap | `design-tokens.md` + aset font berlisensi |
| 4 | Kerangka pelajaran lolos validator | `npm test` + CLI |
| 5 | Tidak ada perubahan Unity selama persiapan | `git status` bersih |

## 3. Kriteria lulus F1

### 3.1 Cakupan fungsional

| # | Kriteria | Bukti |
|---|---|---|
| F1-01 | Tema dan token diterapkan lewat satu aset | review prefab + tes |
| F1-02 | Font TMP memuat glif uji `ā ē ī ō ū ʼ ’ ʔ` | tangkapan layar uji |
| F1-03 | Shell empat tab berjalan | PlayMode + screenshot |
| F1-04 | Beranda membaca 12 topik dari `topics.json` | tes + screenshot |
| F1-05 | State kosong tampil bila belum ada pelajaran terbit | tes + screenshot |
| F1-06 | Alur pelajaran: pembuka → materi → latihan → umpan balik → ringkasan | PlayMode |
| F1-07 | Hasil pelajaran masuk outbox lokal sebelum sinkron | tes `SyncReconciler` |
| F1-08 | Banner offline dan antrean sinkron memakai data nyata | tes + screenshot |
| F1-09 | Tidak ada panel debug di build rilis | pemeriksaan build |
| F1-10 | Portrait dan landscape diuji pada dua ukuran layar | screenshot 360×640 dan 412×915 |

### 3.2 Konten

| # | Kriteria | Bukti |
|---|---|---|
| F1-11 | **Minimal satu pelajaran berstatus `validated`** | berkas di `content/lessons/` |
| F1-12 | Pelajaran itu punya minimal satu `sources[]` yang dapat diperiksa | `sources.csv` + JSON |
| F1-13 | Pelajaran itu punya `reviewedBy` dan `reviewedAt` | `review-signoff.md` |
| F1-14 | Setiap audio yang dipakai punya `consentRef` dan izin penggunaan | `consent-log.csv` |
| F1-15 | Tidak ada audio sintetis pengganti rekaman penutur | review konten |
| F1-16 | Bila audio belum ada, UI menampilkan "Audio Belum Tersedia" | tes + screenshot |
| F1-17 | Tidak ada placeholder di build rilis | `PublishGate` + pemeriksaan APK |
| F1-18 | Tidak ada nama orang nyata tanpa izin | review konten |

**F1 tidak lulus bila F1-11 sampai F1-14 belum terpenuhi**, walaupun UI
sudah berjalan. Sampai saat itu, pelajaran tetap `UNVALIDATED` dan tidak
boleh dipublikasikan.

### 3.3 Aksesibilitas

| # | Kriteria | Bukti |
|---|---|---|
| F1-19 | REM-P1-08.C1–C5 `CLOSED` | skrip kontras |
| F1-20 | REM-P1-08.T1–T12 `CLOSED` | pengukuran rect |
| F1-21 | Uji font 1.3× dan 2.0× tanpa teks terpotong | screenshot |
| F1-22 | Setiap status punya ikon + teks | review |

### 3.4 Kualitas build

| # | Kriteria | Bukti |
|---|---|---|
| F1-23 | Build IL2CPP Release ARM64 berhasil | log build |
| F1-24 | APK terpasang dan dibuka di perangkat fisik | lihat §4 |
| F1-25 | Tidak ada error atau warning kompilasi | log build |
| F1-26 | Ukuran APK dicatat | path + byte |
| F1-27 | Tidak ada kredensial di repositori atau APK | pemindaian |

## 4. Gate perangkat (terpisah)

Gate ini **tidak boleh diklaim lulus** tanpa perangkat Android fisik yang
terhubung.

| # | Kriteria | Bukti |
|---|---|---|
| D1-01 | APK terpasang | `adb install` |
| D1-02 | Aplikasi dibuka tanpa crash | logcat |
| D1-03 | Alur pelajaran dijalankan | rekaman/screenshot |
| D1-04 | Mode offline diuji (mode pesawat) | screenshot |
| D1-05 | Rotasi portrait ↔ landscape | screenshot |
| D1-06 | Ukuran font sistem 1.3× dan 2.0× | screenshot |
| D1-07 | Perilaku tombol back | catatan |

Status gate perangkat: **PASS dengan catatan** (2026-11-12, APK 0.5.0).
D1-01, D1-02, D1-03, D1-04, D1-06 (sampai 1.6×), dan D1-07 lulus. D1-05
**WAIVED** atas keputusan pemilik 2026-11-12: F1 portrait-only dan rotasi
landscape tidak diklaim. Slider font OPPO berhenti di 1.6×, jadi skala 2.0×
hanya dibuktikan lewat PlayMode. Bukti:
`docs/testing/f1-device-evidence.md`.

## 5. Bukti yang tidak diterima

- Tangkapan layar mockup Stitch.
- Konten `UNVALIDATED` sebagai bukti materi.
- "Sudah dicoba di emulator" sebagai pengganti gate perangkat.
- Klaim aksesibilitas tanpa pengukuran.
- Klaim sinkron tanpa log atau hasil.

## 6. Kondisi berhenti

F1 dihentikan dan dilaporkan gagal bila:

1. Ada konten Dawan tanpa sumber di build.
2. Ada placeholder di build rilis.
3. Ada kredensial di repositori.
4. APK tidak dapat dibangun.
5. Gate perangkat diklaim tanpa perangkat.

## 7. Format laporan gate F1

Wajib memuat:

1. Berkas yang berubah.
2. Hasil build: path, ukuran byte, status.
3. Hasil tes: lulus/gagal/dilewati, dengan nama tes.
4. Keputusan yang dikunci dan yang tertunda.
5. Risiko dan tindakan berikutnya.
6. Status gate perangkat secara terpisah.
