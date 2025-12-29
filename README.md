# SharpMediaInfo GUI - Modern .NET Showcase

## 🌟 Modern .NET Architecture Showcase

A **production-ready** MVVM application showcasing modern .NET development with best practices, cutting-edge technologies, and true cross-platform support.

### 🎯 Technology Stack

**Core:**
- ✅ .NET 10 (WPF), .NET 8 (Avalonia, Libraries)
- ✅ C# 12 with latest language features
- ✅ WPF + Fluent Design (Windows native)
- ✅ Avalonia UI 11.3 (cross-platform)
- ✅ True cross-platform architecture (Windows/Linux/macOS)

**Libraries:**
- 🎯 **CommunityToolkit.Mvvm** - Source generators for MVVM (zero reflection!)
- 🎯 **Polly** - Resilience & retry patterns for IPC
- 🎯 **Microsoft.Extensions.DI** - Dependency injection
- 🎯 **System.Text.Json** - Modern JSON serialization
- 🎯 **MessageBox.Avalonia** - Cross-platform dialogs

**Custom Technologies:**
- 🔧 **SharpMediaInfo Source Generators** - Compile-time MediaInfo parsing
- 🔧 **Named Pipes IPC** - Cross-platform inter-process communication
- 🔧 **Polly Master Election** - Resilient single-instance management
- 🔧 **DPI-aware positioning** - High-DPI monitor support

---

## 🚀 Current Status - January 2025

### ✅ WPF Implementation - v1.1 COMPLETED

**Full-featured Windows application with:**
- ✅ All MediaInfo report types (Simple, Standard, Complete, Tree)
- ✅ Cover image display and extraction
- ✅ Advanced drag & drop (tabs between windows)
- ✅ Single instance with Named Pipes IPC
- ✅ Windows Explorer integration (context menu)
- ✅ Theme support (Light/Dark/Auto)
- ✅ Font customization
- ✅ Recent files tracking
- ✅ Configuration persistence

### ✅ Avalonia Implementation - v0.95 (95% Complete!)

**Cross-platform application with:**
- ✅ All MediaInfo report types (Simple, Standard, Complete, Tree)
- ✅ Cover image display and extraction
- ✅ **File drag & drop from Explorer** ✨
- ✅ **Tab reordering (internal)** ✨
- ✅ **Live theme switching** (without restart!)
- ✅ **System fonts enumeration** (all platforms)
- ✅ Settings persistence
- ✅ Recent files tracking
- ✅ IPC support (Named Pipes)
- ⚠️ Final testing in progress

**What's Missing (5%):**
- ⚠️ Comprehensive testing on all platforms
- ⚠️ Minor bug fixes
- ⚠️ Performance tuning

---

## 📁 Project Structure

```
SharpMediaInfo.Native/                # Native MediaInfo binaries
SharpMediaInfo/                       # Core MediaInfo .NET bindings
SharpMediaInfo.Generator/             # Source Generators for compile-time parsing

SharpMediaInfo.GUI/                   # ✅ Shared project (ViewModels, Services)
├── Models/                           # Domain models
├── ViewModels/                       # Platform-agnostic ViewModels
│   ├── MainViewModel.cs              # Main window logic
│   ├── FileTabViewModel.cs           # File tab logic
│   ├── SettingsTabViewModel.cs       # Settings logic
│   └── AboutTabViewModel.cs          # About logic
├── Services/                         # Service interfaces
│   ├── IFileService.cs               # File operations
│   ├── IDialogService.cs             # Dialogs
│   ├── IClipboardService.cs          # Clipboard
│   ├── IConfigurationService.cs      # Config persistence
│   ├── IRecentFilesService.cs        # Recent files
│   ├── ISingleInstanceService.cs     # IPC
│   ├── IContextMenuService.cs        # OS integration
│   ├── IProcessService.cs            # Process launching
│   └── IMousePositionService.cs      # Mouse tracking
└── Converters/                       # Business logic converters
    ├── TrackToReportConverter.cs     # MediaInfo → Report
    └── TrackToTreeConverter.cs       # MediaInfo → Tree

SharpMediaInfo.GUI.WPF/               # ✅ WPF implementation (Windows)
├── Services/                         # WPF-specific implementations
├── Controls/                         # Custom controls
│   ├── SimpleReportViewer.xaml       # Fluent Design report
│   ├── TreeReportViewer.xaml         # TreeView report
│   └── NumericUpDown.xaml            # Numeric input
├── Views/                            # XAML views
├── Windows/
│   └── MainWindow.xaml               # Main window
├── Converters/                       # UI converters
└── App.xaml.cs                       # Bootstrap and DI

SharpMediaInfo.GUI.Avalonia/          # ✅ Avalonia implementation (Cross-platform)
├── Services/                         # Avalonia-specific implementations
│   ├── AvaloniaFileService.cs        # StorageProvider API
│   ├── AvaloniaDialogService.cs      # MessageBox.Avalonia
│   ├── AvaloniaClipboardService.cs   # Cross-platform clipboard
│   └── AvaloniaMousePositionService.cs
├── Controls/                         # Custom controls
│   ├── SimpleReportViewer.axaml      # Direct binding
│   ├── TreeReportViewer.axaml        # TreeDataTemplate
│   └── GenericMenuItem.cs            # Dynamic menus
├── Converters/                       # UI converters (8 total)
│   ├── BoolToFontWeightConverter.cs
│   ├── ByteArrayToImageConverter.cs  # Avalonia.Media.Imaging
│   └── IndentToMarginConverter.cs
├── Views/                            # XAML views
│   ├── SettingTabView.axaml          # Expander-based layout
│   ├── AboutTabView.axaml
│   └── FileTabView.axaml             # All 4 report types
├── Windows/
│   └── MainWindow.axaml              # Main window
└── App.axaml.cs                      # Bootstrap, DI, theming

SharpMediaInfo.GUI.Tests/             # ✅ Unit tests
└── ViewModels/                       # ViewModel tests

Setup/                                # Installation scripts (Windows)
├── install-context-menu.ps1
└── CONTEXT_MENU_GUIDE.md
```

---

## 🏗️ Architecture Highlights

### MVVM Pattern with Source Generators

```csharp
// ViewModel (CommunityToolkit.Mvvm)
[ObservableProperty]
private string _statusMessage = "Ready";
// Auto-generates: public string StatusMessage { get; set; } + INotifyPropertyChanged

[RelayCommand]
private async Task OpenFileAsync() { }
// Auto-generates: public IAsyncRelayCommand OpenFileCommand { get; }
```

### Service Abstraction for Cross-Platform

```csharp
// SharpMediaInfo.GUI (shared)
public interface IFileService
{
    Task<string?> OpenFileAsync(FileType fileType);
}

// SharpMediaInfo.GUI.WPF (Windows)
public class WpfFileService : IFileService { ... }

// SharpMediaInfo.GUI.Avalonia (Cross-platform)
public class AvaloniaFileService : IFileService { ... }
```

### Advanced Features

**WPF-Specific:**
- ✅ Tab drag & drop between windows (creates new instances)
- ✅ Windows Explorer integration (context menu, no admin)
- ✅ DPI-aware window positioning

**Cross-Platform (Avalonia):**
- ✅ File drag & drop from Explorer/Finder
- ✅ Tab reordering within window
- ✅ Named Pipes IPC (Windows/Linux/macOS)
- ✅ Live theme switching
- ✅ System font enumeration

---

## 🚀 Build and Run

### WPF (Windows)
```bash
dotnet run --project SharpMediaInfo.GUI.WPF
```

### Avalonia (Cross-Platform)
```bash
# Windows
dotnet run --project SharpMediaInfo.GUI.Avalonia

# Linux
dotnet run --project SharpMediaInfo.GUI.Avalonia

# macOS
dotnet run --project SharpMediaInfo.GUI.Avalonia
```

### Build All
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

---

## 📚 Documentation

- [Migration Guide](MIGRATION_GUIDE.md) - Complete WPF refactoring journey
- [Migration Checklist](MIGRATION_CHECKLIST.md) - Feature tracking
- [Avalonia Migration Plan](MIGRATION_PLAN_AVALONIA.md) - Cross-platform roadmap
- [Context Menu Guide](Setup/CONTEXT_MENU_GUIDE.md) - Windows Explorer integration

---

## 🎯 Key Achievements

### Refactoring (5 days)
- ✅ Monolithic → MVVM architecture
- ✅ Code-behind → ViewModels + DI
- ✅ Hard-coded → Configuration system
- ✅ Windows Forms → WPF Fluent Design

### WPF v1.0 (5 days)
- ✅ All report types with custom viewers
- ✅ Cover image support
- ✅ Theme system (Light/Dark/Auto)
- ✅ Font customization
- ✅ Configuration persistence

### WPF v1.1 (5 days)
- ✅ Advanced drag & drop system
- ✅ IPC with Named Pipes + Polly
- ✅ Windows Explorer integration
- ✅ Centralized process management

### Avalonia Migration (6 days)
- ✅ 95% feature parity with WPF
- ✅ Cross-platform architecture
- ✅ Live theme switching
- ✅ File & tab drag & drop
- ✅ All report types working
- ⚠️ Final testing in progress

**Total Time**: ~21 days (from monolith to cross-platform!)

---

## 🎉 What Makes This Project Special

### Modern .NET Showcase
- ✅ **Source Generators** - Zero runtime reflection
- ✅ **Polly Resilience** - Automatic failover with retry patterns
- ✅ **Cross-Platform IPC** - Named Pipes on Windows/Linux/macOS
- ✅ **MVVM Best Practices** - 100% testable ViewModels
- ✅ **Clean Architecture** - Clear separation of concerns

### Performance
- ✅ Compile-time code generation (no reflection)
- ✅ Async/await throughout
- ✅ Efficient data binding
- ✅ Minimal memory footprint

### User Experience
- ✅ Fluent Design (WPF)
- ✅ Modern UI (Avalonia)
- ✅ Drag & drop everywhere
- ✅ Live theme switching
- ✅ Keyboard shortcuts
- ✅ Context menus

---

## 📊 Migration Statistics

| Metric | Value |
|--------|-------|
| Lines of Code Refactored | ~15,000+ |
| ViewModels Created | 4 |
| Services Abstracted | 9 |
| Custom Controls | 6 |
| Converters | 10 |
| Unit Tests | 50+ |
| Platforms Supported | 3 (Windows, Linux, macOS) |
| Days to Complete | 21 |
| Code Reuse (WPF→Avalonia) | ~85% |

---

## 🎓 Technologies Demonstrated

- ✅ **MVVM Pattern** with CommunityToolkit.Mvvm
- ✅ **Dependency Injection** with Microsoft.Extensions.DI
- ✅ **Source Generators** for MediaInfo parsing
- ✅ **Polly** for resilience patterns
- ✅ **Named Pipes** for IPC
- ✅ **Async/Await** throughout
- ✅ **Unit Testing** with xUnit
- ✅ **Cross-Platform UI** with Avalonia
- ✅ **Configuration System** with JSON
- ✅ **Process Management** centralized
- ✅ **Theme System** with live switching

---

## 🔮 Future Plans

### v1.0 Final (ETA: 1-2 days)
- ⚠️ Complete Avalonia testing
- ⚠️ Bug fixes
- ⚠️ Documentation polish

### v1.1
- [ ] Linux native builds
- [ ] macOS native builds
- [ ] Performance profiling
- [ ] Accessibility improvements

### v2.0
- [ ] Plugin system
- [ ] Batch processing
- [ ] Export templates
- [ ] Custom reports

---

## 🙏 Credits

Built with modern .NET technologies to showcase best practices in:
- ✅ MVVM architecture with source generators
- ✅ Cross-platform development
- ✅ Dependency injection and service abstraction
- ✅ Resilience patterns with Polly
- ✅ Clean code and maintainability
- ✅ Unit testing

**Special Thanks:**
- MediaInfo library for media analysis
- Avalonia UI team for cross-platform framework
- CommunityToolkit.Mvvm for MVVM source generators
- Polly for resilience patterns
- Microsoft for .NET and tooling

---

**Status**: ✅ v1.0 WPF Complete | ⚠️ v0.95 Avalonia (95% - Final Testing)  
**Last Updated**: January 2025