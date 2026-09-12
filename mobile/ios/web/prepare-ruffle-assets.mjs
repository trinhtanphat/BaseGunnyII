import { cpSync, mkdirSync, readFileSync, readdirSync, rmSync, writeFileSync } from "node:fs";
import { createHash } from "node:crypto";
import { dirname, join, resolve } from "node:path";
import { fileURLToPath } from "node:url";

const here = dirname(fileURLToPath(import.meta.url));
const source = join(here, "node_modules", "@ruffle-rs", "ruffle");
const output = resolve(here, "..", "Generated", "RuffleAssets.bundle");
const pkg = JSON.parse(readFileSync(join(source, "package.json"), "utf8"));
if (pkg.version !== "0.6.0") {
  throw new Error(`Expected @ruffle-rs/ruffle 0.6.0, got ${pkg.version}`);
}

rmSync(output, { recursive: true, force: true });
mkdirSync(output, { recursive: true });
const names = readdirSync(source).filter((name) =>
  name === "ruffle.js" || name.endsWith(".wasm") || /^core\.ruffle\..+\.js$/.test(name)
);
if (!names.includes("ruffle.js") || !names.some((x) => x.endsWith(".wasm"))) {
  throw new Error("Pinned Ruffle package is missing required runtime assets");
}
const manifest = { version: pkg.version, files: {} };
for (const name of names.sort()) {
  const src = join(source, name);
  const dst = join(output, name);
  cpSync(src, dst);
  const hash = createHash("sha256").update(readFileSync(src)).digest("hex");
  manifest.files[name] = hash;
}
writeFileSync(
  join(output, "manifest.json"),
  JSON.stringify(manifest, null, 2) + "\n",
  "utf8"
);
console.log(`RUFFLE_WEB_VERSION=${pkg.version}`);
console.log(`RUFFLE_WEB_ASSETS=${names.length}`);
console.log(`RUFFLE_WEB_OUTPUT=${output}`);
