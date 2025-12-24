# SharpMediaInfo Architecture

This document describes the architectural decisions and design patterns used in SharpMediaInfo.

## Table of Contents

- [Overview](#overview)
- [Core Design Principles](#core-design-principles)
- [Project Structure](#project-structure)
- [Immutable MediaInfo Pattern](#immutable-mediainfo-pattern)
- [Builder Pattern](#builder-pattern)
- [Source Generation](#source-generation)
- [Native Interop Layer](#native-interop-layer)
- [Memory Management](#memory-management)
- [Type System](#type-system)

## Overview

SharpMediaInfo is a modern .NET wrapper for the MediaInfo library that prioritizes:

- **Type Safety** - Leveraging C# 12 and nullable reference types
- **Immutability** - Thread-safe, predictable objects
- **Performance** - Minimal allocations, efficient parsing
- **Maintainability** - Source-generated models, clean separation of concerns
- **Developer Experience** - Fluent APIs, comprehensive documentation

## Core Design Principles

### 1. Immutability First

All `MediaInfo` instances are **fully immutable** once constructed:

```csharp
public sealed partial class MediaInfo : IMediaInfo
{
    // All fields are readonly
    private readonly List<VideoTrack>? _video;
    private readonly int _videoCount;
    private readonly string? _fileName;
    // ...
}
```

**Benefits:**
- Thread-safe by design
- Predictable behavior
- No defensive copying needed
- Clear ownership semantics

### 2. Builder Pattern for Construction

All construction logic is isolated in `MediaInfoBuilder`:

```csharp
public sealed class MediaInfoBuilder
{
    // Mutable during building
    private List<VideoTrack>? _video;

    public MediaInfoBuilder WithCoverData() { /* ... */ return this; }
    public MediaInfo Build() { /* ... */ }
}
```

**Benefits:**
- Clear separation of mutable (building) and immutable (built) phases
- Fluent API for configuration
- Centralized validation and error handling
- Native handle lifecycle contained in builder

### 3. Source Generation

Track models are auto-generated from CSV definitions:

```
Source/Video.csv → VideoTrack.g.cs (generated)
Source/VideoOverride.json → Custom overrides applied
```

**Benefits:**
- Single source of truth (CSV from MediaInfo)
- Easy to update when MediaInfo adds new fields
- Consistent property naming
- Reduced manual coding errors

## Project Structure

### SharpMediaInfo (Main Library)

```
SharpMediaInfo/
├── MediaInfo.cs                 # Immutable data container
├── MediaInfoBuilder.cs          # Builder for construction
├── IMediaInfo.cs               # Public interface
├── Tracks/
│   ├── VideoTrack.cs           # Partial class for custom logic
│   ├── VideoTrack.g.cs        # Source-generated (auto)
│   ├── AudioTrack.cs
│   └── ...
├── Info/
│   └── FrameRate.cs       # Helper types
├── Exception/
│   └── MediaInfoAnalysisException.cs
└── Source/
    ├── Video.csv              # Field definitions
    └── VideoOverride.json     # Type/name overrides
```

**Responsibilities:**
- Public API surface
- Data structures and models
- Helper methods (parsing, formatting)
- Static library information methods

### SharpMediaInfo.Native (Interop Layer)

```
SharpMediaInfo.Native/
├── IMediaInfoNative.cs        # Public native interface
├── MediaInfoNative.cs         # Implementation
├── Interop/
│   ├── MediaInfoNativeMethods.cs  # P/Invoke declarations
│   └── MediaInfoHandle.cs         # SafeHandle implementation
├── Enum/
│   ├── StreamKind.cs
│   └── InfoKind.cs
└── Exception/
    └── MediaInfoNativeException.cs
```

**Responsibilities:**
- P/Invoke to native MediaInfo.dll
- Handle lifecycle management
- Low-level parameter retrieval
- Native exception handling

### SharpMediaInfo.Generator (Source Generator)

```
SharpMediaInfo.Generator/
├── SourceGenerator.cs           # Roslyn source generator
├── TrackFieldInfoParser.cs     # CSV parser
├── TrackCodeBuilder.cs         # Code generation
└── PopulateMediaInfoBuilder.cs # Populate methods
```

**Responsibilities:**
- Parse CSV field definitions
- Apply JSON overrides
- Generate Track partial classes
- Generate populate methods for MediaInfo

## Immutable MediaInfo Pattern

### Construction Flow

```
┌─────────────────┐
│ User Code       │
└────────┬────────┘
         │
         v
┌─────────────────┐
│ MediaInfoBuilder│ ← Mutable during construction
│  .FromFile()    │
│  .WithReports() │
│  .Build()       │
└────────┬────────┘
         │ Parses all data
         │ Closes native handle
         v
┌─────────────────┐
│ MediaInfo       │ ← Immutable after construction
│  (readonly)     │
└─────────────────┘
```

### Key Characteristics

1. **Constructor is Internal**
   ```csharp
   internal MediaInfo(MediaInfoBuilder builder)
   {
       _video = builder.Video;  // Transfer ownership
       _videoCount = builder.VideoCount;
       // ...
   }
   ```

2. **No Setters**
   ```csharp
   public IReadOnlyList<VideoTrack> Video => _video ?? new List<VideoTrack>();
   public int VideoCount => _videoCount;
   public string? FileName => _fileName;
   ```

3. **No IDisposable**
   - No native handles held after construction
   - Builder closes handles in `Build()`
   - No cleanup needed by consumers

## Builder Pattern

### State Machine

```
       FromFile/FromStream/FromCopy
                  │
                  v
            ┌──────────┐
            │ Created  │
            └────┬─────┘
                 │
                 v  With* methods
            ┌──────────┐
            │ Config   │
            └────┬─────┘
                 │
                 v  Build()
            ┌──────────┐
            │  Built   │ ← Terminal state
            └──────────┘
```

### Fluent API Design

```csharp
public sealed class MediaInfoBuilder
{
    private bool _isBuilt;

    public MediaInfoBuilder WithCoverData()
    {
        ThrowIfBuilt();  // Prevent use after Build()
        _parseCoverData = true;
        return this;
    }

    public MediaInfo Build()
    {
        ThrowIfBuilt();
        // Parse everything
        // Close native handle
        _isBuilt = true;  // Mark as built
        return MediaInfo.CreateFromBuilder(this);
    }
}
```

### Benefits

- **Single Use** - Builder can only be used once
- **Clear Lifecycle** - Native handle closed immediately after parsing
- **No Leaks** - Impossible to forget to close handles
- **Testable** - Easy to mock builder for unit tests

## Source Generation

### CSV Format

```csv
Name;Type;Info;Version;Description
Width;;;N YIY;Width in pixels
Height;;;N YIY;Height in pixels
FrameRate;fps;;Y YTY;Frames per second
```

### Override JSON

```json
{
  "OverrideType": "Field",
  "CsvName": "FrameRate",
  "Type": "double?",
  "ParseFunction": "GetDoubleValueNullable"
}
```

### Generated Code

```csharp
// VideoTrack.g.cs (auto-generated)
public partial class VideoTrack
{
    /// <summary>
    /// Width in pixels
    /// </summary>
    public int? Width { get; internal set; }

    /// <summary>
    /// Height in pixels
    /// </summary>
    public int? Height { get; internal set; }

    /// <summary>
    /// Frames per second
    /// </summary>
    public double? FrameRate { get; internal set; }
}
```

### Populate Methods

```csharp
// MediaInfo.Video.g.cs (auto-generated)
internal static partial VideoTrack PopulateVideoTrack(IMediaInfoNative native, int index)
{
    var track = new VideoTrack();
    track.Width = GetIntValueNullable(native.GetParameter(StreamKind.Video, index, "Width"));
    track.Height = GetIntValueNullable(native.GetParameter(StreamKind.Video, index, "Height"));
    // ...
    return track;
}
```

## Native Interop Layer

### Interface Design

```csharp
public interface IMediaInfoNative
{
    string GetParameter(StreamKind kind, int trackIndex, string parameterName);
    int GetStreamCount(StreamKind kind);
    void OpenFile(string path);
    void OpenStream(Stream stream);
    void Close();

    static IMediaInfoNative Create(bool parseCoverData)
        => new MediaInfoNative(parseCoverData);

    static string GetLibraryInfo(string option, string value = "")
        => MediaInfoNative.GetLibraryInfoStatic(option, value);
}
```

### SafeHandle Pattern

```csharp
internal sealed class MediaInfoHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public MediaInfoHandle() : base(true) { }

    protected override bool ReleaseHandle()
    {
        MediaInfoNativeMethods.DeleteHandle(handle);
        return true;
    }
}
```

**Benefits:**
- Automatic cleanup via finalizer
- Protection against handle leaks
- Integration with GC

### P/Invoke Declarations

```csharp
internal class MediaInfoNativeMethods
{
    private const string MEDIAINFO_LIB_FILENAME = "MediaInfo";

    [DllImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_New")]
    public static extern IntPtr CreateHandle();

    [DllImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Delete")]
    internal static extern void DeleteHandle(IntPtr Handle);

    // ...
}
```

## Memory Management

### Lifecycle

```
┌────────────────┐
│ Builder.Build()│
└───────┬────────┘
        │
        v Creates
┌────────────────┐
│ Native Handle  │ ← Wrapped in SafeHandle
└───────┬────────┘
        │
        v Opens file/stream
┌────────────────┐
│ Parse All Data │
└───────┬────────┘
        │
        v
┌────────────────┐
│ Close Handle   │ ← Explicit close
└───────┬────────┘
        │
        v Returns
┌────────────────┐
│ MediaInfo      │ ← No native resources
│ (immutable)    │
└────────────────┘
```

### Key Points

1. **Short-Lived Handles** - Created and destroyed during `Build()`
2. **No User Cleanup** - Users don't manage handles
3. **SafeHandle Backup** - Finalizer cleans up if exception occurs
4. **No Dispose Pattern** - MediaInfo doesn't implement IDisposable

## Type System

### Nullable Reference Types

All reference types properly annotated:

```csharp
public sealed class MediaInfo
{
    private readonly string? _fileName;        // Can be null
    private readonly List<VideoTrack>? _video; // Can be null

    public string? FileName => _fileName;      // Nullable return
    public IReadOnlyList<VideoTrack> Video    // Non-null return
        => _video ?? new List<VideoTrack>();   // Always returns collection
}
```

### Helper Types

```csharp
public sealed class FrameRate
{
    public double? Value { get; }         // FPS as double
    public long? Numerator { get; }       // Rational representation
    public long? Denominator { get; }

    public FrameRate(double? value, long? num, long? den)
    {
        Value = value;
        Numerator = num;
        Denominator = den;
    }
}
```

### Parsing Helpers

All parsing is centralized in `MediaInfo`:

```csharp
internal static class MediaInfo
{
    internal static int? GetIntValueNullable(string input) { /* ... */ }
    internal static long? GetLongValueNullable(string input) { /* ... */ }
    internal static double? GetDoubleValueNullable(string input) { /* ... */ }
    internal static TimeSpan GetTimeSpanValue(string input) { /* ... */ }
    internal static bool? GetBoolValueNullable(string input) { /* ... */ }
    // ...
}
```

## Extensibility Points

### 1. Custom Track Properties

Add custom logic to partial track classes:

```csharp
// VideoTrack.cs (user-written)
public partial class VideoTrack
{
    public double? AspectRatioNumeric
    {
        get
        {
            if (Width.HasValue && Height.HasValue && Height.Value > 0)
                return (double)Width.Value / Height.Value;
            return null;
        }
    }
}
```

### 2. Custom Overrides

Modify `Source/*Override.json` to customize generation:

```json
{
  "OverrideType": "Field",
  "CsvName": "Duration",
  "Name": "DurationInternal",
  "Modifier": "private"
}
```

### 3. Additional Parsers

Add new parsing methods to `MediaInfo`:

```csharp
internal static partial class MediaInfo
{
    internal static MyCustomType GetCustomValue(string input)
    {
        // Custom parsing logic
    }
}
```

## Performance Considerations

1. **Lazy Lists** - Empty lists created only when accessed
2. **Single Parse** - All data parsed once during Build()
3. **No Reflection** - Source-generated populate methods
4. **Struct Where Appropriate** - FrameRate could be a struct
5. **String Interning** - Consider interning common codec names

## Future Enhancements

- **Async API** - Async file/stream opening
- **Streaming Analysis** - Partial analysis without loading full file
- **Custom Formatters** - User-defined output formats
- **Caching** - Cache parsed data for repeated access
- **Validation** - Validate media files meet requirements

## Testing Strategy

1. **Unit Tests** - Test parsing helpers, builders
2. **Integration Tests** - Test with real media files
3. **Generator Tests** - Test source generation logic
4. **Performance Tests** - Benchmark parsing speed
5. **Memory Tests** - Verify no leaks

## Conclusion

This architecture provides a clean, modern, and maintainable wrapper for MediaInfo that prioritizes developer experience while maintaining high performance and type safety.
