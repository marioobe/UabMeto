-- Skema D1 untuk mode online lokal.
-- Ini BUKAN skema produksi. Di produksi, progres/XP ditulis ke Firestore
-- dan D1 hanya menyimpan kuota, idempotensi, serta proyeksi leaderboard.

CREATE TABLE IF NOT EXISTS content_versions (
  lesson_id       TEXT    NOT NULL,
  content_version TEXT    NOT NULL,
  xp              INTEGER NOT NULL,
  status          TEXT    NOT NULL,
  PRIMARY KEY (lesson_id, content_version)
);

CREATE TABLE IF NOT EXISTS operations (
  operation_id      TEXT    PRIMARY KEY,
  user_id           TEXT    NOT NULL,
  lesson_id         TEXT    NOT NULL,
  content_version   TEXT    NOT NULL,
  payload_hash      TEXT    NOT NULL,
  client_claimed_xp INTEGER NOT NULL,
  awarded_xp        INTEGER NOT NULL DEFAULT 0,
  state             TEXT    NOT NULL,
  created_at        TEXT    NOT NULL,
  updated_at        TEXT    NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_operations_user
  ON operations (user_id, updated_at);

CREATE TABLE IF NOT EXISTS applied_effects (
  operation_id TEXT    PRIMARY KEY,
  user_id      TEXT    NOT NULL,
  nickname     TEXT    NOT NULL,
  lesson_id    TEXT    NOT NULL,
  awarded_xp   INTEGER NOT NULL,
  applied_at   TEXT    NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_applied_effects_user_time
  ON applied_effects (user_id, applied_at);

CREATE TABLE IF NOT EXISTS quota (
  user_id             TEXT    NOT NULL,
  day_wita            TEXT    NOT NULL,
  tutor_used          INTEGER NOT NULL DEFAULT 0,
  conversation_used   INTEGER NOT NULL DEFAULT 0,
  pronunciation_used  INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY (user_id, day_wita)
);
