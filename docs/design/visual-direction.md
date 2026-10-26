# Arah Visual

Status: **F0.5 — arah disetujui, rincian dikunci di `design-tokens.md`.**
Sumber arah: paket Stitch `stitch_uab_meto_design_system.zip`. Nilai warna
dan tipografi di bawah adalah ringkasan; bila berbeda, `design-tokens.md`
yang berlaku.

## 1. Prinsip

1. **Mudah dipahami pemula.** Satu layar, satu tujuan. Tidak ada menu
   bertingkat yang dalam.
2. **Modern dan menyenangkan.** Progres terlihat, umpan balik cepat, animasi
   ringan.
3. **Menghormati budaya Dawan/Timor.** Motif dan warna sebagai aksen, bukan
   kostum. Tidak memakai citra sakral tanpa konteks.
4. **Realistis di perangkat rendah.** Sedikit draw call, tanpa efek mahal,
   teks tajam, aset 2D.
5. **Jujur.** Status offline, kuota, dan "belum dapat dinilai" ditampilkan
   jelas.

## 2. Palet

| Peran | Warna | Kode |
|---|---|---|
| Latar utama | Indigo malam | `#0B1519` |
| Permukaan | Indigo lembut | `#16242B` |
| Teks utama | Krem hangat | `#F4EBDD` |
| Teks sekunder | Pasir redup | `#B8AEA0` |
| Aksen utama | Kuning tenun | `#F3D08F` |
| Aksen sekunder | Terakota | `#C96F4A` |
| Sukses | Hijau daun | `#5FA97A` |
| Peringatan | Oranye | `#E39B5C` |
| Bahaya (isian) | Merah bata | `#C6544C` |
| Bahaya (teks) | Merah muda | `#FFB4AB` |

Kontras terukur ada di `accessibility.md`. `#C6544C` tidak boleh dipakai
sebagai warna teks.

## 3. Tipografi

- Judul: Plus Jakarta Sans, tebal.
- Isi: Inter, ukuran minimal 16 sp pada layar 360 dp.
- Metadata: 12 sp, bukan untuk instruksi.
- Teks Dawan dan Indonesia dibedakan dengan label + warna, bukan hanya gaya.
- Hindari font dekoratif untuk teks materi.
- Cakupan glif `ā`, `’`, `ʔ` wajib diuji sebelum prefab dibuat.

## 4. Spasi & bentuk

- Skala 4/8/12/16/24/32.
- Sudut membulat 12–16 px untuk kartu, 24 px untuk pil.
- Tombol utama minimal 48×48 dp.
- Satu aksen motif tenun per layar, bukan lapisan pola penuh.
- Tanpa bayangan dan tanpa blur.

## 5. Komponen dasar

- Kartu pelajaran (terkunci/aktif/selesai).
- Bilah progres topik.
- Tombol primer/sekunder/teks.
- Chip status (offline, tersinkron, kuota habis).
- Kartu kosakata (Dawan + Indonesia + tombol audio).
- Panel umpan balik benar/salah.
- Dialog izin mikrofon dan pemrosesan suara.
- Indikator hati, XP, streak.

## 6. Gerak

- Durasi 150–250 ms.
- Hanya transformasi dan opacity.
- Tidak ada animasi yang menahan input.
- Hormati preferensi "kurangi gerak" bila tersedia.

## 7. Orientasi

- Portrait dan landscape.
- Layout memakai anchor + CanvasScaler, bukan posisi absolut.
- Layar kunci (login, latihan) diuji pada kedua orientasi.

## 8. Aksesibilitas

- Kontras minimal 4.5:1 untuk teks.
- Label tombol ikon.
- Ukuran target sentuh 48 dp.
- Tidak mengandalkan warna saja untuk status.
- Teks dapat diperbesar tanpa terpotong.

## 9. Aset

- Ilustrasi 2D sederhana (SVG/PNG terkompresi).
- Audio hanya dari rekaman terverifikasi.
- Tanpa video latar atau shader berat.
