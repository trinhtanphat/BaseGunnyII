#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
destination="${1:-$script_dir/_deps/ruffle-android}"
revision="$(tr -d '\r\n' < "$script_dir/ruffle-android.revision")"

if [[ -e "$destination" ]]; then
  echo "Destination already exists: $destination" >&2
  exit 2
fi

git clone https://github.com/ruffle-rs/ruffle-android.git "$destination"
git -C "$destination" checkout --detach "$revision"
actual="$(git -C "$destination" rev-parse HEAD)"
if [[ "$actual" != "$revision" ]]; then
  echo "Pinned revision mismatch: expected=$revision actual=$actual" >&2
  exit 3
fi
printf 'RUFFLE_ANDROID_REVISION=%s\n' "$actual"
