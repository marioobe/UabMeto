# Bukti Uji Perangkat F1

Tanggal: 2026-11-03 (WITA)
Status: **PARTIAL** — instalasi, peluncuran, UI dasar, dan tombol back lulus;
alur pelajaran rilis, offline, landscape, dan skala font belum.

## 1. Perangkat dan APK

| Item | Nilai |
|---|---|
| Perangkat | Xiaomi `24117RN76O` |
| Android | 16 (API 36) |
| Layar | 1080×2400, 450 dpi |
| ADB | `device` (terotorisasi) |
| Paket | `id.uabmeto.app` |
| Versi | `0.1.0` (`versionCode 1`) |
| minSdk / targetSdk | 26 / 36 |
| ABI | `arm64-v8a` |
| APK | `Builds/Android/uabmeto-f1.apk` |
| Ukuran | 28.278.020 byte |
| SHA-256 | `D4416E0250AD0F188A45B142BE6A1764850D4C4BD0D6BF18A0FE3AF97BA3C465` |
| firstInstallTime | 2026-11-03 13:56:17 |
| lastUpdateTime | 2026-11-03 14:47:55 |

`adb install -r` berhasil. `dumpsys package` mengonfirmasi versi, minSdk,
targetSdk, dan waktu instalasi di atas.

## 2. Hasil per butir gate perangkat

| ID | Status | Bukti |
|---|---|---|
| D1-01 APK terpasang | **PASS** | `adb install -r` → `Success`; `dumpsys package` |
| D1-02 Dibuka tanpa crash | **PASS** | `UnityPlayerGameActivity` foreground; logcat tanpa fatal/exception aplikasi |
| D1-03 Alur pelajaran | **BLOCKED** | APK rilis tidak memuat pelajaran `validated`; fixture Editor tidak boleh masuk rilis |
| D1-04 Mode offline | **NOT RUN** | Belum diuji dengan mode pesawat/mock reachability |
| D1-05 Rotasi portrait ↔ landscape | **BLOCKED (perangkat)** | Perangkat/launcher memaksa portrait; rotasi paksa menghasilkan letterbox ±885×1080 |
| D1-06 Font sistem 1.3×/2.0× | **NOT RUN** | Belum diuji |
| D1-07 Tombol back | **PASS** | Profil → Beranda; dialog nama → Profil |

## 3. UI yang diperiksa

Semua tangkapan layar berasal dari perangkat, bukan mockup:

| Berkas | Isi |
|---|---|
| `docs/testing/evidence/f1-device/f1-final-coldstart.png` | Beranda setelah cold start, empty state |
| `docs/testing/evidence/f1-device/f1-fixed-beranda-scroll.png` | Daftar topik setelah scroll |
| `docs/testing/evidence/f1-device/f1-fixed-beranda-bottom.png` | Bagian bawah daftar topik |
| `docs/testing/evidence/f1-device/f1-final-profile2.png` | Profil, kartu identitas |
| `docs/testing/evidence/f1-device/f1-fixed-profil-scroll.png` | Profil setelah scroll ke Tentang |
| `docs/testing/evidence/f1-device/f1-final-dialog2.png` | Dialog nama panggilan |
| `docs/testing/evidence/f1-device/f1-final-topic-modal4.png` | Modal topik belum tersedia |
| `docs/testing/evidence/f1-device/f1-final-tutor.png` | Tab Tutor AI |
| `docs/testing/evidence/f1-device/f1-final-peringkat.png` | Tab Peringkat |
| `docs/testing/evidence/f1-device/f1-back-home.png` | Back dari Profil ke Beranda |
| `docs/testing/evidence/f1-device/f1-back-dialog.png` | Back menutup dialog |

Catatan: salinan ini di-commit sebagai bukti; berkas asli dari perangkat ada
di `Builds/device/` yang diabaikan Git. Tangkapan layar bukan aset rilis.

## 4. Temuan dan perbaikan

### 4.1 Scroll tidak dapat digeser

- **Gejala:** drag pada Beranda/Profil tidak menggeser konten.
- **Penyebab:** `ScrollRect` tidak punya `Graphic` raycast target di viewport;
  kartu dan teks memakai `raycastTarget=false`.
- **Perbaikan:** viewport diberi `Image` transparan dengan
  `raycastTarget=true` di `UIFactory.CreateScrollView`.
- **Regresi:** tes PlayMode memastikan viewport punya Graphic raycast target.

### 4.2 Kartu identitas Profil bertumpuk

- **Gejala:** nickname, catatan, dan tombol saling menimpa; avatar melebar.
- **Penyebab:** tinggi kartu tetap 170 dp dan `childForceExpandWidth=true`.
- **Perbaikan:** kartu memakai `VerticalLayoutGroup` dinamis, lebar anak
  dikontrol, avatar 64 dp, dan teks/tombol memakai `flexibleWidth`.
- **Regresi:** tes PlayMode memeriksa tinggi kartu, lebar avatar, dan
  tumpang tindih teks/tombol.

### 4.3 Kartu modal bertumpuk

- **Gejala:** judul, isi, input, dan tombol modal saling menimpa; tombol
  Batal tidak ada.
- **Penyebab:** `ContentSizeFitter` dipasang pada panel tanpa layout group;
  isi diletakkan di child group yang di-stretch.
- **Perbaikan:** panel modal memakai `VerticalLayoutGroup` + fitter; tombol
  `Batal` ditambahkan pada dialog nama.
- **Regresi:** tes PlayMode memeriksa tinggi kartu, urutan tanpa tumpang
  tindih, dan keberadaan tombol Batal/Simpan.

### 4.4 Garis putih di tepi kiri

Garis melengkung putih di tepi kiri terlihat di semua layar aplikasi. Garis
yang sama terlihat di home screen sistem, jadi ini panel tepi OEM, bukan
bagian UI aplikasi. Tidak dihitung sebagai temuan aplikasi.

## 5. Batasan yang belum ditutup

- Tidak ada pelajaran `validated`, jadi alur pelajaran rilis dan audio tidak
  dapat diuji.
- Perangkat/launcher portrait-only; uji landscape perangkat tidak sah.
- TalkBack, keyboard Android, dan skala font sistem belum diuji.
- Banner offline belum diuji dengan mode pesawat.
- Tidak ada profiling performa atau pengukuran startup.
- Build masih memiliki 3 warning TMP/IL2CPP.
