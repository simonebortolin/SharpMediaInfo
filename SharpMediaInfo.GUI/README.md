# SharpMediaInfo GUI - Architettura MVVM

## Panoramica

Questo progetto è stato convertito in un'architettura MVVM (Model-View-ViewModel) pulita e modulare, separata in due progetti principali:

### ?? Struttura dei Progetti

```
SharpMediaInfo.GUI/                    # Progetto condiviso (logica di business)
??? Models/                            # Modelli di dominio
?   ??? FileTabModel.cs               # Modello per un tab di file
?   ??? AppConfiguration.cs           # Configurazione applicazione
??? ViewModels/                        # ViewModels (logica di presentazione)
?   ??? MainViewModel.cs              # ViewModel principale
?   ??? FileTabViewModel.cs           # ViewModel per file tab
??? Services/                          # Interfacce servizi
    ??? IFileService.cs               # Servizio file system
    ??? IDialogService.cs             # Servizio dialoghi

SharpMediaInfo.GUI.WPF/               # Implementazione WPF
??? Services/                         # Implementazioni servizi WPF
?   ??? WpfFileService.cs
?   ??? WpfDialogService.cs
??? Converters/                       # Converter WPF
?   ??? BoolToVisibilityConverter.cs
??? MainWindow.xaml                   # View principale
??? MainWindow.xaml.cs
??? App.xaml
??? App.xaml.cs                       # Bootstrap e DI

SharpMediaInfo.GUI.Avalonia/          # (Futuro) Implementazione Avalonia
??? ...                               # Implementazioni specifiche Avalonia
```

## ?? Vantaggi dell'Architettura

### 1. **Separazione delle Responsabilità**
- **Models**: Contengono solo dati e logica di dominio
- **ViewModels**: Gestiscono lo stato e la logica di presentazione
- **Views**: Solo interfaccia utente (XAML)
- **Services**: Operazioni I/O e interazioni sistema

### 2. **Riusabilità del Codice**
- La logica di business in `SharpMediaInfo.GUI` è completamente indipendente dalla UI
- È possibile creare implementazioni per diverse piattaforme (WPF, Avalonia, UWP, MAUI)
- I test unitari possono concentrarsi sulla logica senza dipendenze UI

### 3. **CommunityToolkit.Mvvm**
Utilizziamo il toolkit ufficiale Microsoft per MVVM che fornisce:

```csharp
// Prima (codice manuale)
private string _name;
public string Name
{
    get => _name;
    set
    {
        if (_name != value)
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }
}

// Dopo (con CommunityToolkit.Mvvm)
[ObservableProperty]
private string _name;
```

**Caratteristiche principali:**
- `[ObservableProperty]` - Genera automaticamente proprietà con INotifyPropertyChanged
- `[RelayCommand]` - Genera automaticamente ICommand da metodi
- `[NotifyPropertyChangedFor]` - Notifica dipendenze tra proprietà
- Source Generators per prestazioni ottimali (zero reflection a runtime)

### 4. **Dependency Injection**
```csharp
// App.xaml.cs - Configurazione DI
private void ConfigureServices(IServiceCollection services)
{
    // Servizi
    services.AddSingleton<IFileService, WpfFileService>();
    services.AddSingleton<IDialogService, WpfDialogService>();
    
    // ViewModels
    services.AddSingleton<MainViewModel>();
    
    // Views
    services.AddSingleton<MainWindow>();
}
```

### 5. **Testabilità**
I ViewModels possono essere testati facilmente con mock dei servizi:

```csharp
// Test esempio
var mockFileService = new Mock<IFileService>();
var mockDialogService = new Mock<IDialogService>();
var viewModel = new MainViewModel(mockFileService.Object, mockDialogService.Object);
```

## ?? Pattern e Pratiche

### Command Pattern
```csharp
[RelayCommand]
private async Task OpenFileAsync()
{
    var filePath = await _fileService.OpenFileAsync();
    // ...
}

// Uso in XAML
<MenuItem Command="{Binding OpenFileCommand}"/>
```

### Service Abstraction
```csharp
// Interfaccia nel progetto condiviso
public interface IFileService
{
    Task<string?> OpenFileAsync();
}

// Implementazione WPF
public class WpfFileService : IFileService
{
    public Task<string?> OpenFileAsync()
    {
        var dialog = new OpenFileDialog();
        // ...
    }
}

// Implementazione Avalonia (futuro)
public class AvaloniaFileService : IFileService
{
    public async Task<string?> OpenFileAsync()
    {
        var dialog = new OpenFileDialog();
        // API diverse ma stessa interfaccia
    }
}
```

### Observable Collections
```csharp
[ObservableProperty]
private ObservableCollection<FileTabViewModel> _fileTabs = new();

// Binding automatico in XAML
<TabControl ItemsSource="{Binding FileTabs}"/>
```

## ?? Passaggi per Avalonia

Per creare la versione Avalonia:

1. **Creare nuovo progetto**
   ```xml
   <Project Sdk="Microsoft.NET.Sdk">
     <PropertyGroup>
       <TargetFramework>net8.0</TargetFramework>
       <OutputType>WinExe</OutputType>
       <UseAvalonia>true</UseAvalonia>
     </PropertyGroup>
     <ItemGroup>
       <ProjectReference Include="..\SharpMediaInfo.GUI\SharpMediaInfo.GUI.csproj" />
     </ItemGroup>
   </Project>
   ```

2. **Implementare i servizi**
   - `AvaloniaFileService` (usa API Avalonia)
   - `AvaloniaDialogService` (usa API Avalonia)

3. **Creare le Views**
   - Stessi ViewModels
   - XAML leggermente diverso (sintassi Avalonia)

4. **Configurare DI**
   ```csharp
   services.AddSingleton<IFileService, AvaloniaFileService>();
   services.AddSingleton<IDialogService, AvaloniaDialogService>();
   services.AddSingleton<MainViewModel>(); // Stesso ViewModel!
   ```

## ?? Migrazione dal Vecchio Codice

### Checklist

- [x] Creare struttura progetti (GUI + GUI.WPF)
- [x] Definire Models con CommunityToolkit.Mvvm
- [x] Definire interfacce Services
- [x] Creare ViewModels con commands
- [x] Implementare servizi WPF
- [x] Creare Views WPF base
- [x] Configurare Dependency Injection
- [ ] Migrare funzionalità complete da GetMediaInfoGUI
  - [ ] Simple Report view
  - [ ] Tree Report view
  - [ ] History management
  - [ ] Configuration management
  - [ ] Theme support
  - [ ] Update system
  - [ ] Inter-process communication
  - [ ] Drag & drop avanzato

## ?? Build e Run

```bash
# Build progetto condiviso
dotnet build SharpMediaInfo.GUI

# Build e run WPF
dotnet run --project SharpMediaInfo.GUI.WPF

# In futuro: Build e run Avalonia
dotnet run --project SharpMediaInfo.GUI.Avalonia
```

## ?? Risorse

- [CommunityToolkit.Mvvm Documentation](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [MVVM Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
