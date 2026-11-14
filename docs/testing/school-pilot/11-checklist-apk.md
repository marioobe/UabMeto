# Checklist APK dan Perangkat

## 1. Sebelum Senin (Minggu)

- [ ] APK utama tersedia: `Builds/Android/uabmeto-f1.apk` (0.5.0,
      `versionCode 6`, ARM64, 28.994.614 byte, SHA-256
      `4901E8D3…A166`).
- [x] APK universal ARMv7+ARM64 dibuat: `Builds/Android/uabmeto-sekolah-universal.apk`
      (44.293.568 byte, SHA-256
      `D94FF98ADBEB77D017122DEE075ED83209304DFFE100D07042EFD5C2F34322D3`).
      Sudah diuji di OPPO: ABI `arm64-v8a` + `armeabi-v7a` terbaca,
      `versionCode 6`, `versionName 0.5.0`, `minSdk 26`; aplikasi terbuka,
      Beranda dan Pelajaran 1 tampil normal.
- [ ] Checksum dicatat dengan PowerShell:
      `Get-FileHash <apk> -Algorithm SHA256`.
- [ ] Flashdisk/Drive berisi kedua APK; salinan di laptop.
- [ ] Kabel USB, charger, dan power bank dibawa.
- [ ] Lembar instrumen, surat izin, dan lembar kesediaan dicetak.
- [ ] Halaman kartu dicetak terpisah dari lembar skor.
- [ ] Kalibrasi dua penguji dilakukan (15 menit).
- [ ] Timer/stopwatch dan alat tulis siap.

### Catatan build universal

Build universal hanya untuk cadangan. Jangan mengubah berkas rilis
0.5.0 yang sudah tercatat. Jika build universal dipakai, catat di
laporan sebagai "APK universal uji sekolah" dan tulis checksum-nya.
Jalur 32-bit tidak dapat diuji di OPPO (ARM64), jadi jika HP siswa
32-bit, tulis batasan ini.

Build universal dibuat dengan
`UabMeto.Editor.BuildScript.BuildAndroidSchoolUniversal`; versi dan
konten sama dengan 0.5.0, hanya ABI diperluas. Build ini tidak menggantikan
`uabmeto-f1.apk`.

## 2. Senin 06:45 — instalasi

### OPPO `P01`

- [ ] Bersihkan data lama (ColorOS menolak `pm clear`):
      `adb uninstall id.uabmeto.app` lalu `adb install -r <apk>`.
- [ ] Pasang APK: `adb install -r <apk>`.
- [ ] Buka aplikasi; isi kode `P01`; tutup.
- [ ] Set font sistem 1.0×, timeout 5–10 menit, portrait.
- [ ] Pastikan baterai ≥80%.

### HP siswa `P02`

- [ ] Aktifkan "Instal dari sumber tidak dikenal" untuk aplikasi
      pengelola berkas/Chrome.
- [ ] Buka APK; jika Play Protect muncul, pilih "Instal saja"
      (hanya untuk APK tim).
- [ ] Jika muncul "Aplikasi tidak terpasang":
      - cek ruang penyimpanan;
      - coba APK universal;
      - jika tetap gagal, catat dan gunakan OPPO secara bergantian
        (kegiatan jadi dua sesi; catat sebagai batasan).
- [ ] Buka aplikasi; isi kode `P02`; tutup.
- [ ] Set font 1.0×, timeout 5–10 menit, portrait.
- [ ] Catat model, versi Android, dan ABI:

```powershell
adb shell getprop ro.product.model
adb shell getprop ro.build.version.release
adb shell getprop ro.product.cpu.abilist
adb shell dumpsys package id.uabmeto.app | Select-String versionName
```

Jika HP siswa tidak mengizinkan USB debugging, catat model/versi dari
Pengaturan → Tentang ponsel; ABI tidak perlu dipaksa.

## 3. Sebelum pretes (09:50)

- [ ] Dua ruang tenang, kursi berhadapan, meja untuk penguji.
- [ ] Satu set kartu dan lembar skor per ruang.
- [ ] Timer siap; naskah sudah dibaca.
- [ ] Wi-Fi/data dimatikan di kedua HP.
- [ ] Aplikasi dibuka di Beranda; kode peserta benar.
- [ ] Tutor AI dan Peringkat dijelaskan tidak dipakai.
- [ ] Jangan menyelesaikan pelajaran saat uji coba; jika terlanjur,
      hapus data dan ulangi pengisian kode.

## 4. Selama sesi APK (10:10–10:35)

- [ ] Siswa menekan "Mulai/Lanjutkan" dan menyelesaikan Pelajaran 1–3.
- [ ] Penguji mendampingi tanpa memberi jawaban soal.
- [ ] Catat waktu mulai/selesai dan pelajaran yang selesai.
- [ ] Catat bantuan yang diberikan (mis. membuka pelajaran, menggulir).
- [ ] Catat jika siswa membuka tab lain atau keluar aplikasi.
- [ ] Jika siswa selesai lebih awal, boleh mengulang; catat.
- [ ] Jangan mengubah pengaturan atau menghapus data.

## 5. Setelah sesi (11:00)

- [ ] Kuesioner siswa dan guru terkumpul.
- [ ] Skor pretes dicek: total = A + B, maksimum 20.
- [ ] Data perangkat dicatat (model, versi, ABI).
- [ ] **Jangan hapus data APK** sampai postes Kamis.
- [ ] Simpan lembar di map tertutup.
- [ ] Konfirmasi kehadiran tim Kamis.

## 6. Kamis 19 November

- [ ] Dua ruang dan lembar postes siap.
- [ ] Postes 10 menit per siswa; urutan diacak.
- [ ] Tanyakan apakah siswa membuka APK selama 3 hari.
- [ ] Catat jawaban; jangan memberi hukuman/nada menilai.
- [ ] Setelah selesai, hapus data (uninstall + pasang ulang bila perlu):
      `adb uninstall id.uabmeto.app`
- [ ] Uninstall jika sekolah tidak memerlukan aplikasi lagi.
- [ ] Ucapkan terima kasih; serahkan salinan surat/laporan jika perlu.

## 7. Troubleshooting

| Gejala | Tindakan |
|---|---|
| "Aplikasi tidak terpasang" | Cek ABI/ruang; coba APK universal; uninstall versi lama |
| `pm clear` ditolak (`SecurityException`) | ColorOS membatasi shell; uninstall lalu pasang ulang |
| `adb install` gagal tanpa pesan | Push ke `/data/local/tmp` lalu `adb shell pm install -r <path>` |
| Play Protect memblokir | "Instal saja"; pastikan APK dari tim, bukan sumber lain |
| Layar terlalu besar/kecil | Set font 1.0×; jangan ubah resolusi; catat |
| Aplikasi keluar sendiri | Buka lagi; catat waktu; jika berulang, hentikan sesi siswa itu |
| Banner "Offline" | Normal; jelaskan hasil menunggu sinkron |
| Tab Tutor AI/Peringkat tidak aktif | Normal; jangan dipakai |
| HP siswa kehabisan baterai | Charger/power bank; catat jika mengganggu |
| Waktu habis | Selesaikan 3 pelajaran; kuesioner bisa menyusul di ruang yang sama |

## 8. Yang dicatat di laporan

- Model dan versi Android kedua HP.
- Versi APK dan checksum (ARM64/universal).
- Waktu mulai/selesai pretes, APK, postes.
- Pelajaran yang selesai dan bantuan yang diberikan.
- Kendala teknis dan tindakan.
- Apakah siswa membuka APK selama jeda.
- Batasan perangkat (mis. ABI berbeda, layar berbeda).
