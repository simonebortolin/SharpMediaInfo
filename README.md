# SharpMediaInfo GUI - MVVM Architecture

## 🌟 Modern .NET Showcase Project

A clean and modular MVVM (Model-View-ViewModel) architecture **designed as a showcase of modern .NET development** with best practices, cutting-edge technologies, and cross-platform preparation.

### 🎯 Technology Stack (v1.1)

**Core:**
- ✅ .NET 10 (latest runtime)
- ✅ C# 12 (latest language features)
- ✅ WPF + Fluent Design
- ✅ Cross-platform architecture (Avalonia-ready)

**Libraries:**
- 🎯 **CommunityToolkit.Mvvm** - Source generators for MVVM (zero reflection!)
- 🎯 **Polly** - Resilience & retry patterns for IPC election
- 🎯 **Microsoft.Extensions.DI** - Dependency injection
- 🎯 **System.Text.Json** - Modern JSON serialization

**Custom Technologies:**
- 🔧 **SharpMediaInfo Source Generators** - Custom source generators for MediaInfo parsing
- 🔧 **Named Pipes IPC** - Cross-platform inter-process communication
- 🔧 **DPI-aware window positioning** - High-DPI monitor support
- 🔧 **Master election with Polly** - Automatic failover and resilience

### 🚀 Features

**Core Features:**
- ✅ MediaInfo analysis with multiple report formats (Simple, Standard, Complete, Tree)
- ✅ Cover image display and extraction
- ✅ Theme support (Light/Dark/Auto)
- ✅ Font customization for reports
- ✅ Recent files management
- ✅ Configuration persistence

**Advanced Features:**
- ✅ **Advanced Drag & Drop** - Drag tabs outside to create new instances
- ✅ **Inter-Process Communication** - Single instance with Named Pipes + Polly
- ✅ **Windows Explorer Integration** - Context menu (no admin required)
- ✅ **Process Management** - Centralized and safe process launching
- ✅ **Master Election** - Automatic failover with resilience patterns
- ✅ **Cross-platform IPC** - Named Pipes work on Windows/Linux/macOS

---

## 📁 Project Structure

```
SharpMediaInfo.Native/				  # Native MediaInfo binaries

SharpMediaInfo/				          # Core MediaInfo .NET bindings

SharpMediaInfo.Generator/			  # Source Generators for MediaInfo bindings

SharpMediaInfo.GUI/                   # Shared project (business logic)
├── Models/                           # Domain models
│   ├── FileTabModel.cs               # Model for a file tab
│   └── AppConfiguration.cs           # Application configuration
├── ViewModels/                       # ViewModels (presentation logic)
│   ├── MainViewModel.cs              # Main ViewModel
│   ├── FileTabViewModel.cs           # ViewModel for a file tab
│   ├── SettingsTabViewModel.cs       # Settings ViewModel
│   └── AboutTabViewModel.cs          # About ViewModel
└── Services/                         # Service interfaces
    ├── IFileService.cs               # File system service
    ├── IDialogService.cs             # Dialog service
    ├── IConfigurationService.cs      # Configuration persistence
    ├── IRecentFilesService.cs        # Recent files management
    ├── ISingleInstanceService.cs     # IPC service
    ├── IContextMenuService.cs        # Windows Explorer integration
    ├── IProcessService.cs            # Process launching
    └── IMousePositionService.cs      # Mouse position tracking

SharpMediaInfo.GUI.WPF/               # WPF implementation
├── Services/                         # WPF service implementations
│   ├── WpfFileService.cs
│   ├── WpfDialogService.cs
│   ├── WpfContextMenuService.cs      # Registry-based context menu
│   └── WpfMousePositionService.cs    # Win32 mouse position
├── Converters/                       # WPF converters
│   ├── BoolToVisibilityConverter.cs
│   ├── ByteArrayToImageConverter.cs
│   └── IndentToMarginConverter.cs
├── Controls/                         # Custom controls
│   ├── SimpleReportViewer.xaml       # Fluent Design report viewer
│   ├── TreeReportViewer.xaml         # TreeView report viewer
│   └── NumericUpDown.xaml            # Numeric input control
├── Views/                            # XAML views
│   ├── SettingsTabView.xaml
│   └── AboutTabView.xaml
├── Windows/
│   ├── MainWindow.xaml               # Main view
│   └── MainWindow.xaml.cs            # Code-behind
├── CommandLineParser.cs              # Argument parsing
├── App.xaml
└── App.xaml.cs                       # Bootstrap and DI

SharpMediaInfo.GUI.Avalonia/          # Avalonia implementation (future)

Setup/                                # Installation scripts
├── install-context-menu.ps1          # PowerShell installer
├── uninstall-context-menu.ps1        # PowerShell uninstaller
├── install-context-menu.bat          # Batch helper
├── install-context-menu.reg          # Manual registry install
└── CONTEXT_MENU_GUIDE.md             # Installation guide
```

# Build and Run
Build e Run

```bash
# Build progetto condiviso
dotnet build SharpMediaInfo.GUI

# Build e run WPF
dotnet run --project SharpMediaInfo.GUI.WPF
```

# Documentation & Resources

- [Context Menu Guide](Setup/CONTEXT_MENU_GUIDE.md) - Windows Explorer integration
- [CommunityToolkit.Mvvm Documentation](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [Polly Documentation](https://www.pollydocs.org/)
- [MVVM Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

## Credits

Built with modern .NET technologies to showcase best practices in:
- MVVM architecture with source generators
- Dependency injection and service abstraction
- Resilience patterns with Polly
- Cross-platform preparation
- Clean code and maintainability