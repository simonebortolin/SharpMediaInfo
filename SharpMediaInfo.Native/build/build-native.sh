#!/usr/bin/env bash
# Build libmediainfo from the pinned MediaInfoLib submodule via CMake.
# ZenLib and zlib are bundled statically, producing a single self-contained
# shared library. Used by local Linux/macOS builds, by WSL (driven from the
# PowerShell script on Windows) and by CI.
#
# Usage: build-native.sh [rid]
#   rid: linux-x64 | linux-arm64 | osx-x64 | osx-arm64   (default: autodetect)
set -euo pipefail

here="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"   # .../SharpMediaInfo.Native/build
native_proj="$(cd "$here/.." && pwd)"                   # .../SharpMediaInfo.Native
repo_root="$(cd "$native_proj/.." && pwd)"
mediainfolib="$repo_root/extern/MediaInfoLib"

rid="${1:-}"
if [ -z "$rid" ]; then
  case "$(uname -s)" in
    Linux)  base=linux ;;
    Darwin) base=osx ;;
    *) echo "Unsupported OS: $(uname -s)" >&2; exit 1 ;;
  esac
  case "$(uname -m)" in
    x86_64|amd64)  arch=x64 ;;
    aarch64|arm64) arch=arm64 ;;
    *) echo "Unsupported arch: $(uname -m)" >&2; exit 1 ;;
  esac
  rid="$base-$arch"
fi

case "$rid" in
  linux-*) libname="libmediainfo.so" ;;
  osx-*)   libname="libmediainfo.dylib" ;;
  *) echo "build-native.sh only builds linux/osx RIDs (got: $rid)" >&2; exit 1 ;;
esac

if [ ! -f "$mediainfolib/Project/CMake/CMakeLists.txt" ]; then
  echo "MediaInfoLib submodule missing at $mediainfolib. Run: git submodule update --init --recursive" >&2
  exit 1
fi

build_dir="$mediainfolib/Project/CMake/build-$rid"
mkdir -p "$build_dir"

# On macOS build exactly the requested architecture, so osx-x64 can be cross-compiled
# on an Apple Silicon host (and vice versa) without needing an Intel machine.
extra_args=()
case "$rid" in
  osx-x64)   extra_args+=("-DCMAKE_OSX_ARCHITECTURES=x86_64") ;;
  osx-arm64) extra_args+=("-DCMAKE_OSX_ARCHITECTURES=arm64") ;;
esac

# ZenLib is bundled statically from the sibling submodule; zlib is taken from the
# system (zlib1g-dev) to avoid the bundled-zlib ALIAS-target CMake issue. CURL and
# TinyXML are optional/runtime and may be reported as "not found".
cmake -S "$mediainfolib/Project/CMake" -B "$build_dir" \
  -DBUILD_ZENLIB=1 -DBUILD_SHARED_LIBS=ON \
  -DCMAKE_BUILD_TYPE=Release \
  -DCMAKE_POLICY_VERSION_MINIMUM=3.5 \
  ${extra_args[@]+"${extra_args[@]}"}

jobs="$(nproc 2>/dev/null || sysctl -n hw.ncpu 2>/dev/null || echo 4)"
cmake --build "$build_dir" --config Release -j"$jobs"

# Locate the unversioned library name. It is usually a symlink to the versioned file
# (libmediainfo.so -> libmediainfo.so.0.0.0 on Linux, libmediainfo.dylib ->
# libmediainfo.0.dylib on macOS); cp -L copies the real file behind it.
src="$(find "$build_dir" -name "$libname" \( -type f -o -type l \) -not -path '*/CMakeFiles/*' -print 2>/dev/null | head -n1)"
if [ -z "$src" ]; then
  echo "Build did not produce $libname under $build_dir" >&2
  exit 1
fi

out_dir="$native_proj/native/$rid"
mkdir -p "$out_dir"
cp -fL "$src" "$out_dir/$libname"
echo "Native ready: $out_dir/$libname"
