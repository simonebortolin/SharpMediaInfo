# Context Menu Installation Guide

## 🎯 Overview

SharpMediaInfo supports Windows Explorer context menu integration, allowing you to right-click any media file and analyze it directly.

## 📋 Windows 10 / Windows 11

### Installation Methods

#### Method 1: PowerShell Script (Recommended)

1. **Right-click** `install-context-menu.ps1`
2. Select **"Run with PowerShell"** (as Administrator)
3. The script will auto-detect the executable location
4. ✅ Done! Right-click any file to test

**Custom path:**
```powershell
.\install-context-menu.ps1 -ExePath "C:\Your\Path\SharpMediaInfo.GUI.WPF.exe"
```

#### Method 2: Registry File

1. **Edit** `install-context-menu.reg`
2. Change all paths to your SharpMediaInfo installation directory
3. **Double-click** the `.reg` file
4. Click **Yes** to confirm
5. ✅ Done!

### Uninstallation

**PowerShell:**
```powershell
# Right-click and "Run with PowerShell" (as Administrator)
.\uninstall-context-menu.ps1
```

**Registry:**
```
Double-click uninstall-context-menu.reg
```

## 🪟 Windows 11 Note

On Windows 11, context menu items appear under **"Show more options"** by default.

To access:
1. Right-click a file
2. Click **"Show more options"** at the bottom
3. Select **"Analyze with SharpMediaInfo"**

### Windows 11 Modern Context Menu (Future)

For native Windows 11 context menu integration, we need to create a **Shell Extension** (requires C++ or .NET Native AOT).

This is planned for v2.0.

## 🔧 Manual Registry Entries

If scripts don't work, add these registry keys manually:

### For All Files
```
HKEY_CLASSES_ROOT\*\shell\SharpMediaInfo
(Default) = "Analyze with SharpMediaInfo"
Icon = "C:\Program Files\SharpMediaInfo\SharpMediaInfo.GUI.WPF.exe,0"

HKEY_CLASSES_ROOT\*\shell\SharpMediaInfo\command
(Default) = "C:\Program Files\SharpMediaInfo\SharpMediaInfo.GUI.WPF.exe" "%1"
```

### For Folders
```
HKEY_CLASSES_ROOT\Directory\shell\SharpMediaInfo
(Default) = "Analyze folder with SharpMediaInfo"
Icon = "C:\Program Files\SharpMediaInfo\SharpMediaInfo.GUI.WPF.exe,0"

HKEY_CLASSES_ROOT\Directory\shell\SharpMediaInfo\command
(Default) = "C:\Program Files\SharpMediaInfo\SharpMediaInfo.GUI.WPF.exe" "%1"
```

## 🧪 Testing

After installation:

1. **Right-click** any media file (`.mp4`, `.mkv`, `.flac`, etc.)
2. Look for **"Analyze with SharpMediaInfo"**
3. Click it → SharpMediaInfo should open with the file analyzed!

**Windows 11:**
- Right-click → **"Show more options"** → **"Analyze with SharpMediaInfo"**

## 🐛 Troubleshooting

### "Access Denied" Error
- Run scripts as **Administrator** (right-click → "Run as Administrator")

### Menu Doesn't Appear
1. Restart Windows Explorer:
   - Open Task Manager (Ctrl+Shift+Esc)
   - Find "Windows Explorer"
   - Right-click → "Restart"

2. Check registry paths are correct (must match your installation)

3. Try logging out and back in

### Wrong Executable Path
- Edit the `.reg` or `.ps1` file
- Replace all paths with your actual installation directory
- Re-run installation

## 📝 Notes

- ✅ Works on **Windows 10** and **Windows 11**
- ✅ Supports **single file** and **folder** analysis
- ✅ Uses **IPC** - opens in existing instance (no duplicate windows)
- ⚠️ Windows 11 requires **"Show more options"** (legacy menu)

## 🚀 Future Plans

- [ ] Native Windows 11 context menu (v2.0)
- [ ] File type filtering (show only for media files)
- [ ] Multiple file selection support
- [ ] Custom icon for menu item

---

**Version**: 1.0  
**Last Updated**: 2025-01-20
