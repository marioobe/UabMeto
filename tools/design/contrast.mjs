#!/usr/bin/env node
/**
 * Mengukur kontras pasangan warna yang benar-benar dipakai UI F1.
 * Sumber warna: ThemeTokens.cs (bukan salinan terpisah).
 *
 * Jalankan: node tools/design/contrast.mjs
 */
import { readFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { dirname, join } from "node:path";

const here = dirname(fileURLToPath(import.meta.url));
const themePath = join(
  here,
  "..",
  "..",
  "app",
  "Assets",
  "_Project",
  "Scripts",
  "App",
  "UI",
  "ThemeTokens.cs"
);

const source = readFileSync(themePath, "utf8");
const colors = new Map();
for (const match of source.matchAll(/public Color (\w+) = Hex\("#([0-9A-Fa-f]{6})"\);/g)) {
  colors.set(match[1], match[2]);
}

if (colors.size === 0) {
  throw new Error("Tidak ada token warna yang terbaca dari ThemeTokens.cs");
}

const channels = (hex) =>
  hex.match(/../g).map((part) => parseInt(part, 16) / 255);

const linear = (value) =>
  value <= 0.04045
    ? value / 12.92
    : Math.pow((value + 0.055) / 1.055, 2.4);

const luminance = (hex) => {
  const [r, g, b] = channels(hex).map(linear);
  return 0.2126 * r + 0.7152 * g + 0.0722 * b;
};

const contrast = (foreground, background) => {
  const a = luminance(foreground);
  const b = luminance(background);
  const light = Math.max(a, b);
  const dark = Math.min(a, b);
  return (light + 0.05) / (dark + 0.05);
};

// Pasangan yang dipakai di layar F1. Teks kecil memakai ambang 4.5:1.
const pairs = [
  ["TextPrimary", "Canvas"],
  ["TextMuted", "Canvas"],
  ["TextMuted", "SurfaceCard"],
  ["TextMuted", "SurfaceHigh"],
  ["Outline", "SurfaceCard"],
  ["Accent", "Canvas"],
  ["OnAccent", "Accent"],
  ["Warning", "SurfaceCard"],
  ["Success", "Canvas"],
  ["Success", "SurfaceCard"],
  ["DangerText", "DangerPanel"],
  ["OnAccent", "Warning"]
];

let failed = 0;
console.log("Pasangan kontras F1 (ambang 4.5:1):");
for (const [foreground, background] of pairs) {
  const fg = colors.get(foreground);
  const bg = colors.get(background);
  if (!fg || !bg) {
    console.log(`  MISSING ${foreground} / ${background}`);
    failed++;
    continue;
  }

  const ratio = contrast(fg, bg);
  const status = ratio >= 4.5 ? "PASS" : "FAIL";
  if (status === "FAIL") failed++;
  console.log(
    `  ${status} ${foreground} on ${background}: ${ratio.toFixed(2)}:1`
  );
}

console.log(
  "\nCatatan: DangerFill tidak dipakai sebagai warna teks di F1; " +
    "pasangan DangerFill/SurfaceCard (3.61:1) sengaja tidak dimasukkan."
);

if (failed > 0) {
  process.exitCode = 1;
}
