#!/usr/bin/env bash
# Build MediaInfoLib (+ ZenLib) for browser-wasm as one static archive,
# native/browser-wasm/libmediainfo.a, ready to be linked into a .NET WebAssembly
# app with <NativeFileReference>.
#
# The toolchain is the Emscripten SDK that ships with the .NET `wasm-tools`
# workload (`dotnet workload install wasm-tools`), so the archive is compiled by
# the exact Emscripten/LLVM version that links the .NET runtime: no separate
# emsdk install and no ABI mismatch. Flags follow the runtime defaults
# (WasmEnableExceptionHandling=true -> -fwasm-exceptions, WasmEnableSIMD=true).
#
# zlib is NOT bundled: the .NET browser runtime already links its own libz.a
# (standard zlib API), so MediaInfoLib is compiled against the zlib headers of
# the extern/zlib submodule and resolves the symbols from the runtime at link time.
#
# Usage: build-native-wasm.sh
# Env:   EMSDK_PACK_DIR  override the Emscripten SDK pack directory (…/Microsoft.NET.Runtime.Emscripten.*.Sdk.<host>/<ver>)
set -euo pipefail

here="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"   # .../SharpMediaInfo.Native/build
native_proj="$(cd "$here/.." && pwd)"                   # .../SharpMediaInfo.Native
repo_root="$(cd "$native_proj/.." && pwd)"
mediainfolib="$repo_root/extern/MediaInfoLib"
zlib_src="$repo_root/extern/zlib"
rid="browser-wasm"

if [ ! -f "$mediainfolib/Project/CMake/CMakeLists.txt" ]; then
  echo "MediaInfoLib submodule missing at $mediainfolib. Run: git submodule update --init --recursive" >&2
  exit 1
fi

# --- Locate the workload packs (dotnet install dir, then the user-local workload dir) -------
case "$(uname -s)-$(uname -m)" in
  Linux-x86_64)   host=linux-x64 ;;
  Linux-aarch64)  host=linux-arm64 ;;
  Darwin-x86_64)  host=osx-x64 ;;
  Darwin-arm64)   host=osx-arm64 ;;
  *) echo "Unsupported host: $(uname -s)-$(uname -m)" >&2; exit 1 ;;
esac

pack_roots=()
[ -n "${DOTNET_ROOT:-}" ] && pack_roots+=("$DOTNET_ROOT/packs")
if command -v dotnet >/dev/null 2>&1; then
  pack_roots+=("$(dirname "$(readlink -f "$(command -v dotnet)")")/packs")
fi
pack_roots+=("$HOME/.dotnet/packs")

# newest version directory of a pack, searched across all pack roots
find_pack() {
  local pattern="$1" root dir best=""
  for root in "${pack_roots[@]}"; do
    for dir in "$root"/$pattern/*/; do
      [ -d "$dir" ] || continue
      best="$(printf '%s\n%s\n' "$best" "${dir%/}" | sed '/^$/d' | sort -V | tail -n1)"
    done
  done
  printf '%s' "$best"
}

emsdk="${EMSDK_PACK_DIR:-$(find_pack "Microsoft.NET.Runtime.Emscripten.*.Sdk.$host")}"
node_pack="$(find_pack "Microsoft.NET.Runtime.Emscripten.*.Node.$host")"
cache_pack="$(find_pack "Microsoft.NET.Runtime.Emscripten.*.Cache.$host")"
runtime_pack="$(find_pack "Microsoft.NETCore.App.Runtime.Mono.browser-wasm")"

if [ -z "$emsdk" ] || [ ! -x "$emsdk/tools/emscripten/emcc" ]; then
  echo "Emscripten SDK pack not found. Install it with: dotnet workload install wasm-tools" >&2
  exit 1
fi
echo "Emscripten SDK pack: $emsdk"

tools="$emsdk/tools"
export DOTNET_EMSCRIPTEN_LLVM_ROOT="$tools/bin"
export DOTNET_EMSCRIPTEN_BINARYEN_ROOT="$tools"
export DOTNET_EMSCRIPTEN_NODE_JS="$node_pack/tools/bin/node"
export EMSDK_PATH="$tools"
export PATH="$tools/emscripten:$tools/bin:$node_pack/tools/bin:$PATH"
export PYTHONUTF8=1
# Use the prebuilt (frozen) sysroot cache from the workload, like the .NET build does.
if [ -n "$cache_pack" ] && [ -d "$cache_pack/tools/emscripten/cache" ]; then
  export EM_CACHE="$cache_pack/tools/emscripten/cache"
  export EM_FROZEN_CACHE=1
fi

# Same code-generation flags as the .NET runtime (wasm EH + SIMD).
# SIZE_T_IS_LONG: on wasm32 size_t is `unsigned long` (32-bit), distinct from int32u/int64u,
# so ZenLib must declare its size_t overloads (it only auto-detects this on LP64/macOS).
wasm_flags="-fwasm-exceptions -msimd128 -O2 -DSIZE_T_IS_LONG"

# zlib: headers from the submodule, library from the .NET runtime pack (only used so
# find_package(ZLIB) succeeds; a static archive does not link anything itself).
zlib_lib="$runtime_pack/runtimes/browser-wasm/native/libz.a"
[ -f "$zlib_lib" ] || zlib_lib="$zlib_src/zlib.h"   # any existing file satisfies FindZLIB

build_dir="$mediainfolib/Project/CMake/build-$rid"
mkdir -p "$build_dir"

# CMP0077=NEW makes ZenLib's option(BUILD_SHARED_LIBS) honour the OFF MediaInfoLib sets.
# CURL is disabled: MediaInfoLib then only builds the runtime-loaded (dlopen) stub.
emcmake cmake -S "$mediainfolib/Project/CMake" -B "$build_dir" \
  -DBUILD_ZENLIB=1 -DBUILD_ZLIB=0 -DBUILD_SHARED_LIBS=OFF \
  -DZLIB_INCLUDE_DIR="$zlib_src" -DZLIB_LIBRARY="$zlib_lib" \
  -DCMAKE_DISABLE_FIND_PACKAGE_CURL=ON -DCMAKE_DISABLE_FIND_PACKAGE_TinyXML=ON \
  -DCMAKE_BUILD_TYPE=Release \
  -DCMAKE_C_FLAGS="$wasm_flags" -DCMAKE_CXX_FLAGS="$wasm_flags" \
  -DCMAKE_POLICY_VERSION_MINIMUM=3.5 \
  -DCMAKE_POLICY_DEFAULT_CMP0077=NEW

jobs="$(nproc 2>/dev/null || sysctl -n hw.ncpu 2>/dev/null || echo 4)"
cmake --build "$build_dir" --config Release -j"$jobs"

mi_lib="$(find "$build_dir" -name 'libmediainfo.a' -not -path '*/CMakeFiles/*' | head -n1)"
zen_lib="$(find "$build_dir" -name 'libzen.a' -not -path '*/CMakeFiles/*' | head -n1)"
if [ -z "$mi_lib" ] || [ -z "$zen_lib" ]; then
  echo "Build did not produce libmediainfo.a / libzen.a under $build_dir" >&2
  exit 1
fi

# Merge MediaInfoLib + ZenLib into one archive, so consumers reference a single file.
out_dir="$native_proj/native/$rid"
mkdir -p "$out_dir"
out="$out_dir/libmediainfo.a"
rm -f "$out"
printf 'CREATE %s\nADDLIB %s\nADDLIB %s\nSAVE\nEND\n' "$out" "$mi_lib" "$zen_lib" | "$tools/bin/llvm-ar" -M
"$tools/bin/llvm-ranlib" "$out"
echo "Native ready: $out"
