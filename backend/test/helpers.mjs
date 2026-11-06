import { DatabaseSync } from 'node:sqlite';
import { readFileSync } from 'node:fs';

export function createTestEnv(overrides = {}) {
  const db = new DatabaseSync(':memory:');
  db.exec(readFileSync(new URL('../schema.sql', import.meta.url), 'utf8'));

  return {
    DB: createD1(db),
    APP_ENV: 'test',
    ...overrides
  };
}

function createD1(db) {
  return {
    prepare(sql) {
      const bound = (params) => ({
        first: async () => db.prepare(sql).get(...params) ?? null,
        all: async () => ({ results: db.prepare(sql).all(...params) }),
        run: async () => {
          const info = db.prepare(sql).run(...params);
          return {
            success: true,
            meta: {
              changes: Number(info.changes),
              last_row_id: Number(info.lastInsertRowid)
            }
          };
        }
      });

      return {
        ...bound([]),
        bind: (...params) => bound(params)
      };
    }
  };
}

export function request(path, { method = 'GET', body, user = 'dev-user' } = {}) {
  const headers = new Headers();
  if (user) {
    headers.set('x-dev-user', user);
  }

  if (body !== undefined) {
    headers.set('content-type', 'application/json');
  }

  return new Request(`http://local.test${path}`, {
    method,
    headers,
    body: body === undefined ? undefined : JSON.stringify(body)
  });
}

export async function seedValidatedLesson(env, lessonId = 'lesson-test', xp = 10) {
  await env.DB.prepare(
    `INSERT INTO content_versions (lesson_id, content_version, xp, status)
     VALUES (?, '1.1.0', ?, 'validated')`
  )
    .bind(lessonId, xp)
    .run();
}

/**
 * Menyisipkan pelajaran sintetis berstatus in_review. Jangan memakai id yang
 * ada di contentSeed karena seedContent menimpa statusnya pada setiap sync.
 */
export async function seedInReviewLesson(
  env,
  lessonId = 'lesson-in-review-test',
  xp = 10
) {
  await env.DB.prepare(
    `INSERT INTO content_versions (lesson_id, content_version, xp, status)
     VALUES (?, '1.1.0', ?, 'in_review')`
  )
    .bind(lessonId, xp)
    .run();
}

export function syncBody(overrides = {}) {
  return {
    operationId: 'op-00001',
    lessonId: 'lesson-test',
    contentVersion: '1.1.0',
    clientClaimedXp: 999,
    nickname: 'Penguji',
    ...overrides
  };
}
