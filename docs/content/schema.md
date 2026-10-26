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
| `schemaVersion` | string | ya | Saat ini `"1.0.0"` |
| `id` | string | ya | Pola `lesson-...` |
| `topicId` | string | ya | Pola `topic-...` |
| `order` | integer ≥1 | ya | Urutan dalam topik, unik |
| `title` | string | ya | Bahasa Indonesia |
| `summary` | string | tidak | Ringkasan |
| `status` | enum | ya | Lihat tabel status |
| `validation` | object | ya | Metadata validasi |
| `objectives` | string[] | ya | Minimal satu |
| `vocabulary` | object[] | ya | Boleh kosong |
| `dialogues` | object[] | ya | Boleh kosong |
| `exercises` | object[] | ya | Minimal satu |
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

## 6. Referensi audio

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

## 7. Latihan

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

## 8. Aturan penerbitan (gate)

Konten hanya dapat diterbitkan bila **semua** benar:

1. `status` = `validated`.
2. `reviewedBy` terisi.
3. `reviewedAt` valid ISO-8601.
4. `sources` minimal satu dan setiap sumber punya `id` + `citation`.
5. `audioLicense` terisi.
6. `consentRef` terisi bila ada audio.
7. Tidak ada placeholder di mana pun.
8. Semua file audio yang dirujuk ada.

Aturan ini diterapkan di dua tempat yang harus konsisten:

- `tools/content-validator/src/rules.mjs`
- `app/Assets/_Project/Scripts/Core/Content/PublishGate.cs`

## 9. Versi skema

- Perubahan yang memecah klien menaikkan `schemaVersion`.
- Klien menolak memuat konten dengan versi mayor yang tidak dikenal.
- Migrasi konten dilakukan oleh tool, bukan manual massal.
