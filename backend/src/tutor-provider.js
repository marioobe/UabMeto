import {
  buildMockAnswer,
  buildSystemPrompt,
  buildUserPrompt
} from './tutor-prompt.js';

export class TutorProviderError extends Error {
  constructor(code, message) {
    super(message);
    this.name = 'TutorProviderError';
    this.code = code;
  }
}

/**
 * Provider dipilih lewat env TUTOR_PROVIDER. "mock" adalah default lokal agar
 * pengembangan dan tes tidak bergantung pada akun Cloudflare atau internet.
 */
export function createTutorProvider(env) {
  const name = (env.TUTOR_PROVIDER ?? 'mock').toLowerCase();

  if (name === 'workers-ai') {
    return new WorkersAiProvider(env);
  }

  return new MockTutorProvider();
}

export class MockTutorProvider {
  constructor() {
    this.name = 'mock';
  }

  async complete({ results }) {
    return { answer: buildMockAnswer(results), model: 'mock' };
  }
}

export class WorkersAiProvider {
  constructor(env) {
    this.name = 'workers-ai';
    this.env = env;
    this.model = env.TUTOR_MODEL ?? '@cf/meta/llama-3.1-8b-instruct';
  }

  async complete({ question, results, history }) {
    if (!this.env.AI || typeof this.env.AI.run !== 'function') {
      throw new TutorProviderError(
        'ai_binding_missing',
        'Binding Workers AI tidak tersedia. Jalankan wrangler login atau pakai TUTOR_PROVIDER=mock.'
      );
    }

    let response;
    try {
      response = await this.env.AI.run(this.model, {
        messages: [
          { role: 'system', content: buildSystemPrompt() },
          { role: 'user', content: buildUserPrompt(question, results, history) }
        ],
        max_tokens: 320,
        temperature: 0.2
      });
    } catch (error) {
      throw new TutorProviderError(
        'provider_error',
        `Workers AI gagal: ${error?.message ?? error}`
      );
    }

    const answer = extractAnswer(response);
    if (!answer) {
      throw new TutorProviderError(
        'provider_empty',
        'Workers AI tidak mengembalikan jawaban.'
      );
    }

    return { answer, model: this.model };
  }
}

function extractAnswer(response) {
  if (typeof response === 'string') {
    return response.trim();
  }

  if (response && typeof response.response === 'string') {
    return response.response.trim();
  }

  if (response && typeof response.result?.response === 'string') {
    return response.result.response.trim();
  }

  return '';
}
