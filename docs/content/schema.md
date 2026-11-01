# Skema Konten Pelajaran

Berkas skema: `content/schema/lesson.schema.json`
Contoh: `content/templates/lesson.template.json`
Contoh tidak tervalidasi: `content/samples/lesson-001.UNVALIDATED.json`

## 1. Status konten

| Status | Arti | Boleh terbit? |
|---|---|---|
| `UNVALIDATED` | Placeholder teknis, bukan materi Dawan | Tidak, pernah |
| `draft` | Sedang disusun | Tidak |
| `in_review` | Menunggu penutur lokal | Tidak |
| `validated` | Sudah diperiksa & disetujui | Ya, lewat publish |
| `published` | Sudah terbit | Sudah |

Aturan keras:

- File di `content/samples/` **wajib** `UNVALIDATED`.
- File di `content/lessons/` **tidak boleh** `UNVALIDATED`.
- Konten `UNVALIDATED` wajib memakai placeholder `[PLACEHOLDER-...]` pada
  setiap field `dawan` dan `indonesian`.
- Konten selain `UNVALIDATED` tidak boleh mengandung placeholder di mana pun.

## 2. Field tingkat pelajaran

| Field | Tipe | Wajib | Keterangan |
|---|---|---|---|
| `schemaVersion` | string | ya | Saat ini `"1.1.0"` |
| `id` | string | ya | Pola `lesson-...` |
| `topicId` | string | ya | Pola `topic-...` |
| `order` | integer ≥1 | ya | Urutan dalam topik, unik |
| `xp` | integer ≥0 | ya (1.1.0) | Nilai tampilan; hadiah aktual ditentukan server |
| `estimatedMinutes` | integer 1–120 | ya (1.1.0) | Perkiraan untuk tampilan, bukan batas waktu |
| `title` | string | ya | Bahasa Indonesia |
| `summary` | string | tidak | Ringkasan |
| `status` | enum | ya | Lihat tabel status |
| `validation` | object | ya | Metadata validasi |
| `objectives` | string[] | ya | Minimal satu |
| `vocabulary` | object[] | ya | Boleh kosong |
| `dialogues` | object[] | ya | Boleh kosong |
| `exercises` | object[] | ya | Minimal satu |
| `examples` | object[] | ya (1.1.0) | Contoh kalimat; lihat bagian 6 |
| `notes` | string | tidak | Catatan editorial |

## 3. Metadata validasi

| Field | Tipe | Wajib saat validated/published |
|---|---|---|
| `reviewedBy` | string | ya |
| `reviewedAt` | string ISO `YYYY-MM-DD` | ya |
| `sources` | array `{id, citation, url?, license?}` | minimal satu |
| `audioLicense` | string | ya |
| `consentRef` | string | ya, bila pelajaran beraudio |

## 4. Kosakata

```json
{
  "id": "vocab-001",
  "dawan": "[PLACEHOLDER-DWN-001]",
  "indonesian": "[PLACEHOLDER-IDN-001]",
  "notes": "Catatan penggunaan",
  "audio": null
}
```

`audio` boleh `null` atau objek audio (lihat bagian 6).

## 5. Dialog

```json
{
  "id": "dlg-001",
  "title": "Judul dialog",
  "lines": [
    {
      "speaker": "A",
      "dawan": "[PLACEHOLDER-DWN-010]",
      "indonesian": "[PLACEHOLDER-IDN-010]",
      "audio": null
    }
  ]
}
```

## 6. Contoh kalimat

```json
{
  "id": "exm-001",
  "dawan": "[PLACEHOLDER-DWN-020]",
  "indonesian": "[PLACEHOLDER-IDN-020]",
  "context": "situasi pemakaian",
  "status": "UNVALIDATED",
  "sourceRef": null,
  "audio": null
}
```

Aturan:

- `id` berpola `exm-...` dan unik dalam satu pelajaran.
- `dawan`, `indonesian`, dan `context` wajib terisi.
- `status` mengikuti enum status pelajaran.
- `sourceRef` wajib berpola `SRC-...` untuk contoh yang diterbitkan.
- Pelajaran `UNVALIDATED` hanya boleh memuat contoh `UNVALIDATED`.
- Pelajaran `validated`/`published` wajib punya minimal satu contoh, dan
  semua contohnya harus `validated` atau `published`.
- `audio` opsional; bila ada, mengikuti aturan audio di bagian 7.
- Jangan mengarang kalimat untuk mengisi field ini.

Status contoh harus dapat ditelusuri. Contoh tidak boleh "naik status"
hanya karena pelajarannya diterbitkan.

## 7. Referensi audio

```json
{
  "file": "audio/lesson-001/vocab-001.ogg",
  "durationMs": 1200,
  "bitrateKbps": 32,
  "license": "CC-BY-4.0",
  "consentRef": "CONSENT-0001"
}
```

- Ekstensi wajib `.ogg` (Ogg Vorbis).
- Path relatif terhadap root `content/`, tanpa `..`.
- Untuk status `validated`/`published`, file harus benar-benar ada.

## 8. Latihan

Setiap latihan memiliki `id`, `type`, `skill`, `prompt`, `content`, dan
opsional `explanation`.

| `type` | `skill` umum | Field wajib di `content` | Catatan |
|---|---|---|---|
| `multiple_choice` | reading | `options[]`, `correctOptionId` | `explanation` wajib |
| `match` | reading | `pairs[]` (`leftId`, `rightId`) | Minimal 2 pasangan |
| `sentence_order` | writing | `tokens[]`, `correctOrder[]` | Token harus sama |
| `listening` | listening | `audioRef`, `options[]`, `correctOptionId` | `explanation` wajib |
| `writing` | writing | `expectedAnswer` | Penilaian otomatis terbatas |
| `speaking` | speaking | `targetAudioRef` | Tidak memberi lulus/gagal |

Contoh pilihan ganda:

```json
{
  "id": "ex-001",
  "type": "multiple_choice",
  "skill": "reading",
  "prompt": "Pilih padanan yang benar.",
  "explanation": "Alasan jawaban.",
  "content": {
    "options": [
      { "id": "opt-a", "text": "..." },
      { "id": "opt-b", "text": "..." }
    ],
    "correctOptionId": "opt-a"
  }
}
```

## 9. Aturan penerbitan (gate)

Konten hanya dapat diterbitkan bila **semua** benar:

1. `schemaVersion` = `1.1.0`.
2. `status` = `validated`.
3. `reviewedBy` terisi.
4. `reviewedAt` valid ISO-8601.
5. `sources` minimal satu dan setiap sumber punya `id` + `citation`.
6. `audioLicense` terisi.
7. `consentRef` terisi bila ada audio.
8. Tidak ada placeholder di mana pun.
9. Semua file audio yang dirujuk ada.
10. Minimal satu `examples[]`, semuanya `validated`/`published` dan
    punya `sourceRef`.

Aturan ini diterapkan di dua tempat yang harus konsisten:

- `tools/content-validator/src/rules.mjs`
- `app/Assets/_Project/Scripts/Core/Content/PublishGate.cs`

## 10. XP dan server

`xp` pada JSON adalah nilai tampilan dan saran. Hadiah XP aktual dihitung
atau divalidasi server dari versi materi yang diterbitkan. Nilai `xp` yang
dikirim klien tidak boleh dipakai untuk menentukan hadiah.

`estimatedMinutes` hanya untuk tampilan. Nilai ini bukan batas waktu,
syarat kelulusan, atau dasar penalti.

## 11. Versi skema dan migrasi

- Versi saat ini: `1.1.0`.
- Versi `1.0.0` masih dapat dibaca validator, tetapi **tidak dapat
  diterbitkan**.
- Migrasi mekanis: `node src/migrate.mjs --in <file> --out <file>`.
- Konten `validated`/`published` wajib memakai
  `--demote-to-in-review`; status diturunkan dan review diulang.
- Migrasi tidak menambah teks Dawan, tidak menaikkan status, dan tidak
  mengarang contoh.
- Aturan lengkap: `docs/content/migration-1.0.0-to-1.1.0.md`.
