# SharpMediaInfo

A modern .NET 8 wrapper for the MediaInfo library with source-generated track models and immutable design patterns.

## Features

- ✨ **Immutable by Design** - Thread-safe, predictable MediaInfo objects
- 🔧 **Fluent Builder API** - Clean and intuitive API for creating instances
- 🚀 **Source Generated Models** - Track classes auto-generated from CSV definitions
- 📦 **Strongly Typed** - Full nullable reference type support
- 🎯 **Multiple Input Sources** - Support for files, streams, and cloning
- 🔒 **Memory Safe** - Automatic native handle management
- 📊 **Rich Metadata** - Access to all MediaInfo track types (General, Video, Audio, Text, Image, Menu)

## Installation

```bash
# Coming soon to NuGet
dotnet add package SharpMediaInfo
```

## Quick Start

### Basic Usage

```csharp
using SharpMediaInfo;

// Analyze a video file
var mediaInfo = MediaInfoBuilder
    .FromFile("movie.mkv")
    .Build();

// Access track information
Console.WriteLine($"File: {mediaInfo.FileName}");
Console.WriteLine($"Video tracks: {mediaInfo.VideoCount}");
Console.WriteLine($"Audio tracks: {mediaInfo.AudioCount}");

// Iterate through video tracks
foreach (var video in mediaInfo.Video)
{
    Console.WriteLine($"Codec: {video.Format}");
    Console.WriteLine($"Resolution: {video.Width}x{video.Height}");
    Console.WriteLine($"Frame Rate: {video.FrameRate?.Value} fps");
}

// Iterate through audio tracks
foreach (var audio in mediaInfo.Audio)
{
    Console.WriteLine($"Codec: {audio.Format}");
    Console.WriteLine($"Channels: {audio.Channels}");
    Console.WriteLine($"Sample Rate: {audio.SamplingRate} Hz");
}
```

### Advanced Options

```csharp
// Parse with cover data and reports
var fullInfo = MediaInfoBuilder
    .FromFile("video.mp4")
    .WithCoverData()          // Parse embedded cover images
    .WithReports()            // Generate both Complete and Standard reports
    .Build();

// Access reports
Console.WriteLine(fullInfo.InfoComplete);
Console.WriteLine(fullInfo.InfoStandard);

// Parse only specific reports
var customInfo = MediaInfoBuilder
    .FromFile("audio.flac")
    .WithCompleteReport()     // Only complete report
    .Build();
```

### Stream Analysis

```csharp
// Analyze from a stream
using var stream = File.OpenRead("audio.mp3");
var streamInfo = MediaInfoBuilder
    .FromStream(stream)
    .WithCoverData()
    .Build();

Console.WriteLine($"Duration: {streamInfo.General[0].Duration}");
```

### Copying MediaInfo

```csharp
// Create a copy of an existing MediaInfo instance
var original = MediaInfoBuilder.FromFile("movie.mkv").Build();
var copy = MediaInfoBuilder.FromCopy(original).Build();
```

## Builder Methods

### Factory Methods

| Method | Description |
|--------|-------------|
| `FromFile(string path)` | Create builder from a file path |
| `FromStream(Stream stream)` | Create builder from a stream |
| `FromCopy(MediaInfo mediaInfo)` | Create builder by copying existing data |

### Configuration Methods

| Method | Description |
|--------|-------------|
| `WithCoverData()` | Enable parsing of embedded cover images as base64 |
| `WithReports()` | Parse both Complete and Standard reports |
| `WithCompleteReport()` | Parse only the Complete report |
| `WithStandardReport()` | Parse only the Standard report |
| `Build()` | Build the immutable MediaInfo instance |

## MediaInfo Properties

### Track Collections

| Property | Type | Description |
|----------|------|-------------|
| `General` | `IReadOnlyList<GeneralTrack>` | General/container information |
| `Video` | `IReadOnlyList<VideoTrack>` | Video stream information |
| `Audio` | `IReadOnlyList<AudioTrack>` | Audio stream information |
| `Text` | `IReadOnlyList<TextTrack>` | Subtitle/caption information |
| `Image` | `IReadOnlyList<ImageTrack>` | Image stream information |
| `Menu` | `IReadOnlyList<MenuTrack>` | Menu/chapter information |

### Metadata

| Property | Type | Description |
|----------|------|-------------|
| `FileName` | `string?` | File name or path |
| `ParseCoverData` | `bool` | Whether cover data parsing was enabled |
| `InfoComplete` | `string?` | Complete MediaInfo report |
| `InfoStandard` | `string?` | Standard MediaInfo report |
| `InfoCustom` | `string` | Custom formatted report with all properties |

### Track Counts

| Property | Type | Description |
|----------|------|-------------|
| `GeneralCount` | `int` | Number of general streams |
| `VideoCount` | `int` | Number of video streams |
| `AudioCount` | `int` | Number of audio streams |
| `TextCount` | `int` | Number of text streams |
| `ImageCount` | `int` | Number of image streams |
| `MenuCount` | `int` | Number of menu streams |

## Static Library Methods

Access MediaInfo library information without creating instances:

```csharp
// Get library version
string version = MediaInfo.LibraryVersion();
Console.WriteLine($"MediaInfo Version: {version}");

// Get supported codecs
string codecs = MediaInfo.KnownCodecs();

// Get library capabilities
string capabilities = MediaInfo.Capacities();

// Get available parameters
string parameters = MediaInfo.ParametersList(csvFormat: false);

// Get library URL
string url = MediaInfo.LibraryUrl();
```

## Track Models

### VideoTrack

Common properties include:
- `Format` - Video codec (e.g., "AVC", "HEVC")
- `Width` / `Height` - Video dimensions
- `FrameRate` - Frame rate information (value, numerator, denominator)
- `BitRate` - Bitrate in bits per second
- `Duration` - Duration in milliseconds
- `DisplayAspectRatio` - Display aspect ratio
- `ColorSpace` - Color space (e.g., "YUV")

### AudioTrack

Common properties include:
- `Format` - Audio codec (e.g., "AAC", "AC-3")
- `Channels` - Number of audio channels
- `SamplingRate` - Sample rate in Hz
- `BitRate` - Bitrate in bits per second
- `Duration` - Duration in milliseconds
- `Language` - Language code (ISO 639-2)

### GeneralTrack

Common properties include:
- `Format` - Container format (e.g., "Matroska", "MPEG-4")
- `FileSize` - File size in bytes
- `Duration` - Total duration
- `OverallBitRate` - Overall bitrate
- `Title` / `Movie` - Media title
- `EncodedDate` - Encoding date

### TextTrack

Common properties include:
- `Format` - Subtitle format (e.g., "UTF-8", "ASS")
- `Language` - Language code
- `Title` - Track title
- `Default` / `Forced` - Track flags

### MenuTrack

Common properties include:
- `Chapters` - List of chapter information
- Chapter details (title, language, timecode)

## Architecture

### Project Structure

```
SharpMediaInfo/                 # Main library
├── MediaInfo.cs               # Immutable data class
├── MediaInfoBuilder.cs        # Builder for creating instances
├── IMediaInfo.cs             # Public interface
├── Tracks/                   # Track model classes (generated)
├── Info/                     # Helper types (FrameRate, etc.)
└── Exception/                # Custom exceptions

SharpMediaInfo.Native/         # Native interop layer
├── IMediaInfoNative.cs       # Native interface
├── MediaInfoNative.cs        # Native implementation
└── Interop/                  # P/Invoke declarations

SharpMediaInfo.Generator/      # Source generator
├── SourceGenerator.cs        # Main generator
├── Source/*.csv              # MediaInfo field definitions
└── Source/*Override.json     # Custom type overrides
```

### Design Principles

1. **Immutability** - `MediaInfo` objects are fully immutable after construction
2. **Builder Pattern** - All construction logic isolated in `MediaInfoBuilder`
3. **Source Generation** - Track models auto-generated from CSV definitions
4. **Separation of Concerns** - Native interop isolated in separate project
5. **Modern .NET** - Leverages .NET 8 features and nullable reference types

## Requirements

- .NET 8.0 or later
- MediaInfo native library (MediaInfo.dll on Windows, libmediainfo on Unix)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the terms specified in the LICENSE file.

## Credits

Built on top of the excellent [MediaInfo library](https://mediaarea.net/MediaInfo) by MediaArea.

## See Also

- [MediaInfo Official Website](https://mediaarea.net/MediaInfo)
- [MediaInfo Documentation](https://mediaarea.net/MediaInfo/Support/SDK)
