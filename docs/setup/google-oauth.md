# Setup Google Sign-In

Status: **panduan saja.** Belum ada OAuth client yang dibuat.

## 1. Prasyarat

- Proyek Firebase sudah ada (lihat `firebase.md`).
- Package name Android: `id.uabmeto.app`.
- SHA-1 keystore debug dan (nanti) keystore rilis.

## 2. Mendapatkan SHA-1 debug

Keystore debug Unity/Android biasanya ada di:

```
%USERPROFILE%\.android\debug.keystore
```

Ambil SHA-1 dengan JDK bawaan Unity:

```powershell
& "D:\Unity2022\6000.6.0f1\Editor\Data\PlaybackEngines\AndroidPlayer\OpenJDK\bin\keytool.exe" `
  -list -v `
  -alias androiddebugkey `
  -keystore "$env:USERPROFILE\.android\debug.keystore" `
  -storepass android -keypass android
```

Salin baris **SHA1**.

## 3. OAuth consent screen

1. Google Cloud Console → APIs & Services → OAuth consent screen.
2. Tipe: External (kecuali akun organisasi).
3. Isi nama aplikasi, email dukungan, dan domain kebijakan privasi
   (dapat ditunda sampai sebelum publikasi).
4. Scope minimal: `email`, `profile`, `openid`.
5. Tambahkan akun penguji selama mode testing.

## 4. Client ID

- **Android client**: package name + SHA-1. Tidak ada client secret.
- **Web client**: dipakai Firebase untuk alur Google Sign-In di Unity.
- Client secret web **tidak boleh** masuk APK.

## 5. Integrasi Unity (F1)

- Gunakan Firebase Auth Unity SDK.
- Jika memakai plugin Google Sign-In terpisah, pastikan konfigurasi
  `webClientId` diambil dari konfigurasi publik, bukan hardcode rahasia.
- Uji pada build Development dan Release (SHA-1 berbeda).

## 6. Verifikasi

- [ ] Login Google berhasil pada build debug.
- [ ] Email terverifikasi tidak meminta verifikasi tambahan.
- [ ] Penolakan izin tidak membuat aplikasi crash.
- [ ] Logout menghapus sesi.

## 7. Catatan keamanan

- Client ID Android bukan rahasia, tetapi tetap jangan disalahgunakan sebagai
  otorisasi server.
- Client secret web hanya boleh berada di layanan server bila benar-benar
  diperlukan.
