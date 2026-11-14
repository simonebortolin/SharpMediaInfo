#!/usr/bin/env bash
# Publishes the browser GUI and serves it on http://localhost:${PORT:-8080}/.
# Needs the wasm-tools workload and SharpMediaInfo.Native/native/browser-wasm/libmediainfo.a
# (bash SharpMediaInfo.Native/build/build-native-wasm.sh, or the CI's native-browser-wasm artifact).
set -euo pipefail
cd "$(dirname "$0")/.."

OUT="${OUT:-artifacts/web}"
PORT="${PORT:-8080}"

if [ ! -f SharpMediaInfo.Native/native/browser-wasm/libmediainfo.a ]; then
    echo "Missing SharpMediaInfo.Native/native/browser-wasm/libmediainfo.a: build it first with" >&2
    echo "  bash SharpMediaInfo.Native/build/build-native-wasm.sh" >&2
    exit 1
fi

dotnet publish SharpMediaInfo.GUI.Avalonia.Browser -c Release -o "$OUT"

echo "Serving $OUT/wwwroot on http://localhost:$PORT/ (Ctrl+C to stop)"
cd "$OUT/wwwroot"
exec python3 -m http.server "$PORT" --bind 127.0.0.1
