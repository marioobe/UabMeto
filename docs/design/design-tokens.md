# Token Desain

Status: **F0.5 — dikunci untuk implementasi.** Dokumen ini menggantikan
nilai warna dan tipografi di `DESIGN.md` Stitch bila terjadi perbedaan.

Satu sumber kebenaran: `app/Assets/_Project/Scripts/App/UI/ThemeTokens.asset`
(ScriptableObject) yang dibuat pada F1. Prefab dan skrip tidak boleh memuat
kode hex mentah.

## 1. Prinsip

1. **Datar, bukan berlapis.** Kedalaman dibuat dengan perbedaan luminansi
   permukaan, bukan bayangan.
2. **Tanpa blur.** Tidak ada `backdrop-blur`, glassmorphism, atau shader
   realtime. Header dan bottom bar memakai warna solid.
3. **Tanpa bayangan.** `shadow-*` di mockup tidak diterjemahkan.
4. **Motif sebagai garis.** Belah ketupat 4 dp pada garis 1 dp, bukan pola
   penuh atau ilustrasi.
5. **Warna tidak sendiri.** Status selalu punya ikon atau teks pendamping.

## 2. Warna

### 2.1 Token semantik

| Token | Hex | Pemakaian | Larangan |
|---|---|---|---|
| `canvas` | `#0B1519` | latar akar | — |
| `surface-card` | `#16242B` | kartu, panel, sheet | — |
| `surface-low` | `#141D22` | panel sekunder, track | — |
| `surface` | `#182126` | kontainer netral | — |
| `surface-high` | `#222B31` | state terpilih, kontrol | — |
| `surface-highest` | `#2D363C` | kontrol aktif, chip kuat | — |
| `border` | `#223640` | garis 1 dp, pemisah | jangan untuk teks |
| `outline` | `#998F81` | ikon pasif, label sekunder | jangan untuk teks < 14 sp pada `surface-high` |
| `text-primary` | `#F4EBDD` | judul, isi utama | — |
| `text-muted` | `#B8AEA0` | keterangan, terjemahan | — |
| `accent` | `#F3D08F` | aksi utama, progres, status aktif | jangan untuk teks di atas `accent` |
| `on-accent` | `#0B1519` | teks/ikon di atas `accent` | — |
| `accent-deep` | `#C96F4A` | aksen budaya, state pressed | jangan untuk teks kecil |
| `accent-deep-bg` | `#7B3313` | latar pressed, badge budaya | — |
| `success` | `#5FA97A` | benar, selesai | — |
| `warning` | `#E39B5C` | offline, kuota, perhatian | — |
| `danger-fill` | `#C6544C` | isian, garis, ikon bahaya | **jangan untuk teks** |
| `danger-text` | `#FFB4AB` | teks bahaya | — |
| `success-panel` | `#132B20` | latar panel benar | — |
| `danger-panel` | `#2C1415` | latar panel salah | — |
| `scrim` | `#000000` @ 80% | latar modal | — |

Catatan: `primary`, `secondary`, `tertiary`, dan `on-*` dari `DESIGN.md`
adalah nama peran Material, bukan token produk. Jangan dipakai di kode.

### 2.2 Kontras terukur

Nilai dihitung dengan rumus WCAG 2.1 (luminans relatif), bukan dikutip
dari mockup. Ambang: 4.5:1 untuk teks normal, 3:1 untuk teks ≥ 18.66 px
tebal atau ≥ 24 px.

| Teks | Latar | Rasio | Status |
|---|---|---|---|
| `text-primary` | `canvas` | 15.65 | lulus |
| `text-primary` | `surface-card` | 13.45 | lulus |
| `text-muted` | `canvas` | 8.45 | lulus |
| `text-muted` | `surface-card` | 7.27 | lulus |
| `accent` | `canvas` | 12.53 | lulus |
| `on-accent` | `accent` | 12.53 | lulus |
| `success` | `surface-card` | 5.63 | lulus |
| `warning` | `surface-card` | 6.89 | lulus |
| `danger-text` | `surface-card` | 9.36 | lulus |
| `danger-text` | `danger-panel` | 10.15 | lulus |
| `text-primary` | `danger-panel` | 14.59 | lulus |
| `text-primary` | `success-panel` | 12.75 | lulus |
| `outline` | `canvas` | 5.81 | lulus |
| `outline` | `surface-high` | 4.53 | lulus tipis; hindari teks kecil |
| `accent-deep` | `accent` | 4.56 | lulus tipis; hanya teks besar |
| `danger-fill` | `surface-card` | **3.61** | **gagal** |
| `danger-fill` | `canvas` | **4.20** | **gagal** |
| `primary` `#FFEFD7` | `accent` | **1.31** | **gagal total** |

Aturan turunan:

- Teks bahaya wajib `danger-text`, bukan `danger-fill`.
- `danger-fill` hanya untuk isian, garis, dan ikon.
- Jangan pernah memakai `#FFEFD7` atau `text-primary` di atas `accent`.
- `accent-deep` `#C96F4A` tidak dipakai untuk teks di bawah 18.66 px tebal.

## 3. Tipografi

| Gaya | Font | Ukuran / baris | Berat | Pemakaian |
|---|---|---|---|---|
| `headline-1` | Plus Jakarta Sans | 24 / 32 | 700 | judul layar |
| `headline-2` | Plus Jakarta Sans | 20 / 28 | 600 | judul bagian |
| `headline-3` | Plus Jakarta Sans | 16 / 24 | 600 | judul kartu |
| `body` | Inter | 16 / 24 | 400 | isi, penjelasan |
| `body-small` | Inter | 14 / 20 | 400 | keterangan |
| `label-caps` | Plus Jakarta Sans | 12 / 16 | 600, +0.06em | label bagian |
| `label-chip` | Inter | 12 / 16 | 500 | chip, metadata |
| `button` | Plus Jakarta Sans | 16 / 24 | 700 | tombol |

Aturan:

- Isi minimum 16 sp pada 360 dp. Informasi penting tidak boleh 12 sp.
- `label-caps` dan `label-chip` hanya untuk metadata, bukan instruksi.
- Jangan memakai huruf dekoratif atau italic untuk teks materi.
- Teks Dawan dan Indonesia dibedakan dengan label + warna, bukan hanya
  gaya huruf.

### 3.1 Aset font di Unity

| Berkas | Lisensi | Catatan |
|---|---|---|
| Inter | SIL OFL 1.1 | regular, medium, semibold, bold |
| Plus Jakarta Sans | SIL OFL 1.1 | semibold, bold |
| Material Symbols Outlined | Apache 2.0 | subset ikon |

Buat `TMP_FontAsset` dinamis: sampling 90, padding 9, atlas 1024,
multi-atlas aktif. Sertakan berkas lisensi di `app/Assets/ThirdParty/`.

### 3.2 Uji cakupan glif sebelum UI

Font harus lulus uji render untuk:

```
ā ē ī ō ū ʼ ’ ‘ ʔ é è ê ñ
```

Gagal render = ganti font sebelum prefab dibuat. Uji ini tidak memakai
kata Dawan, hanya cakupan Unicode.

## 4. Spasi dan bentuk

- Skala: 4, 8, 12, 16, 24, 32 dp.
- Margin luar: 16 dp (ponsel), 24 dp (layar ≥ 600 dp).
- Jarak antar kartu: 12 dp.
- Radius: 8 (chip kecil), 12 (kartu), 16 (kartu besar), 24 (pil),
  9999 (lingkaran).
- Garis: 1 dp `border`; state aktif 1.5–2 dp `accent`.
- Target sentuh minimum 48 × 48 dp, termasuk tombol ikon.
- Tidak ada posisi absolut; pakai anchor, layout group, dan safe area.

## 5. Elevasi

| Level | Latar | Garis | Dipakai untuk |
|---|---|---|---|
| 0 | `canvas` | — | akar layar |
| 1 | `surface-card` | 1 dp `border` | kartu, daftar |
| 2 | `surface-high` | 1 dp `accent` | item terpilih, sheet |
| 3 | `surface-card` + `scrim` | 1 dp `border` | modal |

Tidak ada bayangan, blur, atau gradien. Pemisahan dibuat oleh warna.

## 6. Komponen

| Komponen | Tinggi | Radius | Catatan |
|---|---|---|---|
| Tombol primer | 48 | 24 | isi `accent`, teks `on-accent` |
| Tombol sekunder | 48 | 24 | transparan, garis 1.5 dp `border` |
| Tombol hantu | 48 | 24 | teks `text-primary` atau `accent` |
| Tombol ikon | 48 | 24 | ikon 24 dp, label aksesibilitas wajib |
| Chip | 28 | 14 | padding horizontal 12 |
| Tile jawaban | 56 | 12 | padding 16 |
| Kartu pelajaran | dinamis | 12–16 | minimum 72 |
| Bilah progres | 8 | 4 | isi `accent` atau `success` |
| Bottom bar | 64 + safe area | 0 | 4 item, tiap item ≥ 48 |
| Top bar | 64 + safe area | 0 | judul + aksi |

State tile jawaban:

| State | Latar | Garis | Ikon |
|---|---|---|---|
| dasar | `surface-card` | 1 dp `border` | — |
| terpilih | `surface-high` | 2 dp `accent` | radio terisi |
| benar | `success-panel` | 2 dp `success` | centang |
| salah | `danger-panel` | 2 dp `danger-fill` | silang |

## 7. Ikon

- Material Symbols Outlined, ukuran 20/24/28 dp, ketebalan 400.
- Subset hanya ikon yang dipakai; simpan daftar di
  `docs/design/icon-inventory.md` pada F1.
- Ikon dekoratif diberi `aria-hidden`; ikon aksi wajib punya label teks
  atau label aksesibilitas.
- Bila ukuran APK membengkak, ganti ke sprite atlas 2D tanpa mengubah
  token.

## 8. Gerak

- Durasi 150–250 ms, ease-out.
- Hanya transformasi dan opasitas.
- Tidak ada animasi yang menahan input.
- Hormati "kurangi gerak": ganti transisi dengan perubahan langsung.
- Tanpa animasi loop kecuali indikator memuat.

## 9. Tema gelap saja

Versi pertama hanya tema gelap. Alasan: satu tema lebih murah diuji pada
perangkat rendah dan mockup sudah konsisten.

Klaim "menghemat daya" hanya berlaku pada layar OLED; jangan dipakai
sebagai klaim pemasaran.

## 10. Pemetaan ke Unity

| Token | Implementasi |
|---|---|
| Warna | field `Color` di `ThemeTokens` |
| Tipografi | `TMP_StyleSheet` + `TMP_FontAsset` |
| Spasi | konstanta `float` di `ThemeTokens` |
| Komponen | prefab di `Assets/_Project/Prefabs/UI` |
| Tema | satu aset, di-inject lewat `AppRoot` |

Aturan penegakan:

1. Tidak ada `new Color(...)` atau hex di skrip UI.
2. Validator editor menolak prefab yang memuat warna di luar token.
3. `Image` tanpa `ThemeBinder` dianggap gagal review.
