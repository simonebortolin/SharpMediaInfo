<#
.SYNOPSIS
  Build the MediaInfo native library from the pinned MediaInfoLib submodule.

.DESCRIPTION
  Windows RIDs (win-x64/win-x86/win-arm64) are built with the Visual Studio C++
  toolset using the MediaInfoLib MSVC solution (.slnx preferred, .sln fallback).
  Linux/macOS RIDs are delegated to build-native.sh running inside WSL, so a
  Windows developer can also produce the linux-x64 binary locally.

.PARAMETER Rid
  Target RID. Defaults to the Windows host architecture.
#>
[CmdletBinding()]
param(
    [string]$Rid = ""
)
$ErrorActionPreference = "Stop"

$here        = Split-Path -Parent $MyInvocation.MyCommand.Path   # .../SharpMediaInfo.Native/build
$nativeProj  = Split-Path -Parent $here                          # .../SharpMediaInfo.Native
$repoRoot    = Split-Path -Parent $nativeProj
$mediaInfoLib = Join-Path $repoRoot "extern/MediaInfoLib"

if (-not $Rid) {
    switch ([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture) {
        "X64"   { $Rid = "win-x64" }
        "X86"   { $Rid = "win-x86" }
        "Arm64" { $Rid = "win-arm64" }
        default { throw "Unsupported host architecture" }
    }
}

if (-not (Test-Path (Join-Path $mediaInfoLib "Project/CMake/CMakeLists.txt"))) {
    throw "MediaInfoLib submodule missing at $mediaInfoLib. Run: git submodule update --init --recursive"
}

# ----- Linux / macOS: delegate to the shared bash script through WSL -----
if ($Rid -like "linux-*" -or $Rid -like "osx-*") {
    $scriptWin = Join-Path $here "build-native.sh"
    # Map a Windows path to its WSL /mnt/<drive> equivalent (avoids wslpath quoting issues).
    $drive = $scriptWin.Substring(0, 1).ToLower()
    $scriptWsl = "/mnt/$drive" + $scriptWin.Substring(2).Replace('\', '/')
    Write-Host "Building $Rid via WSL: $scriptWsl"
    & wsl.exe bash "$scriptWsl" $Rid
    if ($LASTEXITCODE -ne 0) { throw "WSL build failed ($LASTEXITCODE). Ensure the default WSL distro has: build-essential cmake." }
    return
}

# ----- Windows: MSVC build -----
$platform = @{ "win-x64" = "x64"; "win-x86" = "Win32"; "win-arm64" = "ARM64" }[$Rid]
if (-not $platform) { throw "Unsupported RID: $Rid" }

$vswhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio/Installer/vswhere.exe"
if (-not (Test-Path $vswhere)) { throw "vswhere.exe not found; install Visual Studio." }
$msbuild = (& $vswhere -latest -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" |
            Select-Object -First 1)
if (-not $msbuild) { throw "VS MSBuild not found. Install the 'Desktop development with C++' workload." }

# Newest MSVC solution: .slnx (VS 2026+) preferred, otherwise classic .sln
$solution = Get-ChildItem -Path "$mediaInfoLib/Project/MSVC*/MediaInfoLib.slnx" -ErrorAction SilentlyContinue |
            Sort-Object FullName | Select-Object -Last 1
if (-not $solution) {
    $solution = Get-ChildItem -Path "$mediaInfoLib/Project/MSVC*/MediaInfoLib.sln" -ErrorAction SilentlyContinue |
                Sort-Object FullName | Select-Object -Last 1
}
if (-not $solution) { throw "No MediaInfoLib MSVC solution found under $mediaInfoLib/Project." }

# Pick the platform toolset. The MediaInfoLib MSVC solution targets a specific toolset
# (e.g. v143 for MSVC2022, v145 for MSVC2026); that toolset is the most compatible with its pinned sources
# (notably bundled {fmt}), so use it when installed. Only when it is missing do we retarget
# to the newest toolset available, so the build still works on a machine that only ships a
# newer one.
$vsRoot = (& $vswhere -latest -requires Microsoft.Component.MSBuild -property installationPath | Select-Object -First 1)
$installedToolsets = Get-ChildItem "$vsRoot\MSBuild\Microsoft\VC\*\Platforms\x64\PlatformToolsets" -Directory -ErrorAction SilentlyContinue |
                     ForEach-Object { Get-ChildItem $_.FullName -Directory -ErrorAction SilentlyContinue } |
                     Select-Object -ExpandProperty Name -Unique |
                     Where-Object { $_ -match '^v\d+$' }

$wantedToolset = $null
# Read it from the library project next to the chosen solution, not from an older MSVC folder.
$libVcxproj = Join-Path $solution.Directory.FullName "Library/MediaInfoLib.vcxproj"
if (Test-Path $libVcxproj) {
    $m = Select-String -Path $libVcxproj -Pattern '<PlatformToolset>(v\d+)</PlatformToolset>' | Select-Object -First 1
    if ($m) { $wantedToolset = $m.Matches[0].Groups[1].Value }
}

$toolsetArg = @()
if ($wantedToolset -and ($installedToolsets -contains $wantedToolset)) {
    $toolset = $wantedToolset           # use the solution's own toolset (no retarget)
} else {
    $toolset = $installedToolsets | Sort-Object { [int]($_ -replace '[^0-9]', '') } | Select-Object -Last 1
    if ($toolset) { $toolsetArg = @("-p:PlatformToolset=$toolset") }
}

# Relax the /guard:ehcont mismatch between the MediaInfo DLL and the ZenLib submodule
# (see msvc-overrides.props) without editing the submodule projects.
$overrideProps = Join-Path $here "msvc-overrides.props"

Write-Host "Building $Rid ($platform, toolset $toolset) from $($solution.FullName)"
& $msbuild $solution.FullName -t:MediaInfoDll -p:Configuration=Release -p:Platform=$platform `
    @toolsetArg "-p:ForceImportAfterCppTargets=$overrideProps" -m -v:minimal -nologo
if ($LASTEXITCODE -ne 0) { throw "MSVC build failed ($LASTEXITCODE)." }

$dll = Join-Path $solution.Directory.FullName "$platform/Release/MediaInfo.dll"
if (-not (Test-Path $dll)) { throw "Expected DLL not found at $dll" }

$outDir = Join-Path $nativeProj "native/$Rid"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
Copy-Item $dll (Join-Path $outDir "mediainfo.dll") -Force
Write-Host "Native ready: $(Join-Path $outDir 'mediainfo.dll')"
