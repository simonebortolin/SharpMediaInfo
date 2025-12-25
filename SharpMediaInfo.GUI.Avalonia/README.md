# SharpMediaInfo.GUI.Avalonia (Future Implementation)

## ?? Note

Questo progetto sarà un'implementazione ridotta di SharpMediaInfo GUI per Avalonia, pensata principalmente come test della portabilità dell'architettura MVVM.

## ?? Funzionalità Previste (Ridotte)

### ? Incluse
- [ ] Open single file
- [ ] Display standard report
- [ ] Display complete report
- [ ] Basic file information
- [ ] Cross-platform support (Windows, Linux, macOS)

### ? Non Incluse (almeno per v1.0)
- Simple Report (formattazione complessa)
- Tree Report
- Cover image support
- History management
- Update system
- Theme customization
- Inter-process communication

## ??? Struttura Progetto

```
SharpMediaInfo.GUI.Avalonia/
??? Services/
?   ??? AvaloniaFileService.cs
?   ??? AvaloniaDialogService.cs
??? Views/
?   ??? MainWindow.axaml
?   ??? MainWindow.axaml.cs
??? Assets/
?   ??? icon.ico
??? App.axaml
??? App.axaml.cs
??? Program.cs
```

## ?? Setup

### Prerequisites
```bash
dotnet new install Avalonia.Templates
```

### Create Project
```bash
dotnet new avalonia.app -n SharpMediaInfo.GUI.Avalonia
cd SharpMediaInfo.GUI.Avalonia
dotnet add reference ../SharpMediaInfo.GUI/SharpMediaInfo.GUI.csproj
dotnet add reference ../SharpMediaInfo/SharpMediaInfo.csproj
dotnet add package CommunityToolkit.Mvvm
dotnet add package Microsoft.Extensions.DependencyInjection
```

## ?? Project File

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <BuiltInComInteropSupport>true</BuiltInComInteropSupport>
    <ApplicationManifest>app.manifest</ApplicationManifest>
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Avalonia" Version="11.0.6" />
    <PackageReference Include="Avalonia.Desktop" Version="11.0.6" />
    <PackageReference Include="Avalonia.Themes.Fluent" Version="11.0.6" />
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\SharpMediaInfo.GUI\SharpMediaInfo.GUI.csproj" />
    <ProjectReference Include="..\SharpMediaInfo\SharpMediaInfo.csproj" />
  </ItemGroup>
</Project>
```

## ?? Key Differences from WPF

### File Dialog
```csharp
// WPF
var dialog = new OpenFileDialog();
dialog.ShowDialog();

// Avalonia
var topLevel = TopLevel.GetTopLevel(this);
var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
{
    Title = "Select a file",
    AllowMultiple = false
});
```

### Message Dialog
```csharp
// WPF
MessageBox.Show(message, title);

// Avalonia (using MessageBox.Avalonia package)
var messageBoxStandardWindow = MessageBoxManager
    .GetMessageBoxStandard(title, message);
await messageBoxStandardWindow.ShowAsync();
```

### Binding Syntax
```xml
<!-- WPF -->
<TextBlock Text="{Binding PropertyName}"/>

<!-- Avalonia (quasi identico) -->
<TextBlock Text="{Binding PropertyName}"/>
```

## ?? Implementation Example

### AvaloniaFileService.cs
```csharp
using Avalonia.Platform.Storage;
using SharpMediaInfo.GUI.Services;

namespace SharpMediaInfo.GUI.Avalonia.Services;

public class AvaloniaFileService : IFileService
{
    private readonly IStorageProvider _storageProvider;

    public AvaloniaFileService(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task<string?> OpenFileAsync()
    {
        var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select a file to analyze",
            AllowMultiple = false
        });

        return files.Count > 0 ? files[0].Path.LocalPath : null;
    }

    // ... altri metodi
}
```

### App.axaml.cs
```csharp
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using SharpMediaInfo.GUI.Services;
using SharpMediaInfo.GUI.ViewModels;

namespace SharpMediaInfo.GUI.Avalonia;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            desktop.MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IDialogService, AvaloniaDialogService>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<MainWindow>();
    }
}
```

### MainWindow.axaml
```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:SharpMediaInfo.GUI.ViewModels"
        x:Class="SharpMediaInfo.GUI.Avalonia.MainWindow"
        x:DataType="vm:MainViewModel"
        Title="SharpMediaInfo GUI (Avalonia)"
        Width="900" Height="600">

    <Grid RowDefinitions="Auto,*,Auto">
        <!-- Menu -->
        <Menu Grid.Row="0">
            <MenuItem Header="_File">
                <MenuItem Header="_Open File" Command="{Binding OpenFileCommand}"/>
                <MenuItem Header="Open _Multiple Files" Command="{Binding OpenFilesCommand}"/>
                <Separator/>
                <MenuItem Header="E_xit" Click="Exit_Click"/>
            </MenuItem>
        </Menu>

        <!-- Content -->
        <TabControl Grid.Row="1" 
                    ItemsSource="{Binding FileTabs}"
                    SelectedItem="{Binding SelectedFileTab}"
                    Margin="5">
            <TabControl.ItemTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding Model.FileName}"/>
                </DataTemplate>
            </TabControl.ItemTemplate>
            <TabControl.ContentTemplate>
                <DataTemplate>
                    <TabControl>
                        <TabItem Header="Standard Report">
                            <TextBox Text="{Binding Model.StandardReport}"
                                     IsReadOnly="True"
                                     TextWrapping="Wrap"
                                     FontFamily="Consolas"/>
                        </TabItem>
                        <TabItem Header="Complete Report">
                            <TextBox Text="{Binding Model.CompleteReport}"
                                     IsReadOnly="True"
                                     TextWrapping="Wrap"
                                     FontFamily="Consolas"/>
                        </TabItem>
                    </TabControl>
                </DataTemplate>
            </TabControl.ContentTemplate>
        </TabControl>

        <!-- Status Bar -->
        <TextBlock Grid.Row="2" 
                   Text="{Binding StatusMessage}"
                   Padding="5"/>
    </Grid>
</Window>
```

## ?? Todo List

- [ ] Create Avalonia project
- [ ] Implement AvaloniaFileService
- [ ] Implement AvaloniaDialogService
- [ ] Create MainWindow.axaml
- [ ] Test on Windows
- [ ] Test on Linux
- [ ] Test on macOS
- [ ] Package for distribution

## ?? Goals

1. **Proof of Concept**: Dimostrare che l'architettura MVVM è veramente cross-platform
2. **Minimal Implementation**: Solo funzionalità base per validare l'approccio
3. **Learning**: Esperienza con Avalonia framework
4. **Future**: Base per una versione completa cross-platform

## ?? Resources

- [Avalonia Documentation](https://docs.avaloniaui.net/)
- [Avalonia MVVM](https://docs.avaloniaui.net/docs/concepts/the-mvvm-pattern/)
- [Avalonia Samples](https://github.com/AvaloniaUI/Avalonia.Samples)
