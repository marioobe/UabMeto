# Arah Visual Awal

Status: **usulan F0, belum final.** UI final menunggu mockup Stitch yang
disetujui. Layar Unity saat ini hanya wireframe netral.

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

## 2. Palet awal (usulan)

| Peran | Warna | Kode |
|---|---|---|
| Latar utama | Indigo malam | `#0E171C` |
| Permukaan | Indigo lembut | `#16242B` |
| Teks utama | Krem hangat | `#F4EBDD` |
| Aksen utama | Kuning tenun | `#F3D08F` |
| Aksen sekunder | Terakota | `#C96F4A` |
| Sukses | Hijau daun | `#5FA97A` |
| Peringatan | Oranye | `#E39B5C` |
| Bahaya | Merah bata | `#C6544C` |

Palet ini belum diuji kontras. Target kontras teks minimal 4.5:1.

## 3. Tipografi

- Judul: sans-serif geometris, tebal.
- Isi: sans-serif humanis, ukuran minimal 16 sp pada layar 360 dp.
- Teks Dawan dan Indonesia dibedakan dengan warna/label, bukan hanya gaya.
- Hindari font dekoratif untuk teks materi.

## 4. Spasi & bentuk

- Skala 4/8/12/16/24/32.
- Sudut membulat 12–20 px untuk kartu.
- Tombol utama minimal 48×48 dp.
- Satu aksen motif tenun per layar, bukan lapisan pola penuh.

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
