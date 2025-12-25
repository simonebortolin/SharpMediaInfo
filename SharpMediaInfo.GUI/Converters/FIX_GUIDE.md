# ?? Fix Necessari per i Converter

## Problema

I tipi personalizzati di SharpMediaInfo (`ByteSize`, `BitRate`, `AspectRatio`, `Language`, `TimeSpan`, `ChannelConfig`) sono `struct` che contengono property nullable, non sono loro stessi nullable.

## Soluzione

Invece di:
```csharp
if (general.FileSize.HasValue)  // ? ERRORE: ByteSize non ha HasValue
```

Usa:
```csharp
if (general.FileSize.Bytes.HasValue)  // ? CORRETTO: Bytes è long?
```

## Fix da Applicare

### TrackToReportConverter.cs & TrackToTreeConverter.cs

**Cerca e sostituisci:**

1. `general.FileSize.HasValue` ? `general.FileSize.Bytes.HasValue`
2. `general.Duration.HasValue` ? `general.Duration != default`  (TimeSpan è struct, confronta con default)
3. `general.OverallBitRate.HasValue` ? `general.OverallBitRate.BitsPerSecond.HasValue`
4. `video.Language?.ToString()` ? `video.Language.IsUndetermined ? null : video.Language.ToString()`
5. `video.BitRate.HasValue` ? `video.BitRate.BitsPerSecond.HasValue`
6. `video.DisplayAspectRatio.HasValue` ? `video.DisplayAspectRatio.Value.HasValue`
7. `audio.Language?.ToString()` ? `audio.Language.IsUndetermined ? null : audio.Language.ToString()`
8. `audio.Duration.HasValue` ? `audio.Duration != default`
9. `audio.BitRate.HasValue` ? `audio.BitRate.BitsPerSecond.HasValue`
10. `audio.Channels.HasValue` ? `audio.Channels.Count.HasValue`
11. `audio.Delay?.Milliseconds` ? `audio.Delay.TotalMilliseconds` (poi controlla > 0)
12. `text.Language?.ToString()` ? `text.Language.IsUndetermined ? null : text.Language.ToString()`
13. `text.Delay?.Milliseconds` ? `text.Delay.TotalMilliseconds`
14. `video.Delay?.Milliseconds` ? `video.Delay.TotalMilliseconds`
15. `audio.BitRateMaximum.HasValue` ? `audio.BitRateMaximum.BitsPerSecond.HasValue`

## Esempi Corretti

### ByteSize
```csharp
// ? Sbagliato
if (general.FileSize.HasValue)
    items.Add($"File Size: {general.FileSize}");

// ? Corretto
if (general.FileSize.Bytes.HasValue)
    items.Add($"File Size: {general.FileSize}");
```

### Language
```csharp
// ? Sbagliato
var lang = video.Language?.ToString() ?? "Undetermined";

// ? Corretto
var lang = video.Language.IsUndetermined ? "Undetermined" : video.Language.ToString();
```

### TimeSpan
```csharp
// ? Sbagliato
if (audio.Delay?.Milliseconds > 0)

// ? Corretto
if (audio.Delay.TotalMilliseconds > 0)
```

### BitRate
```csharp
// ? Sbagliato
if (video.BitRate.HasValue)
    items.Add($"BitRate: {video.BitRate}");

// ? Corretto
if (video.BitRate.BitsPerSecond.HasValue)
    items.Add($"BitRate: {video.BitRate}");
```

### AspectRatio
```csharp
// ? Sbagliato
if (video.DisplayAspectRatio.HasValue)

// ? Corretto
if (video.DisplayAspectRatio.Value.HasValue)
```

### ChannelConfig
```csharp
// ? Sbagliato
if (audio.Channels.HasValue)

// ? Corretto
if (audio.Channels.Count.HasValue)
```

## Alternativa: Metodi Helper

Puoi creare metodi helper nel converter:

```csharp
private bool HasValue(ByteSize size) => size.Bytes.HasValue;
private bool HasValue(BitRate rate) => rate.BitsPerSecond.HasValue;
private bool HasValue(AspectRatio ratio) => ratio.Value.HasValue;
private bool HasValue(ChannelConfig config) => config.Count.HasValue;
private bool IsSet(TimeSpan timeSpan) => timeSpan != default;
private string GetLanguageOrDefault(Language lang) => 
    lang.IsUndetermined ? "Undetermined" : lang.ToString();
```

Poi usa:
```csharp
if (HasValue(general.FileSize))
    items.Add($"File Size: {general.FileSize}");

if (IsSet(general.Duration))
    items.Add($"Duration: {general.Duration}");

var language = GetLanguageOrDefault(video.Language);
```

## Prossimi Passi

1. Applica queste fix a `TrackToReportConverter.cs`
2. Applica queste fix a `TrackToTreeConverter.cs`
3. Rebuild del progetto
4. Test dell'applicazione

## Note Importanti

- Tutti questi tipi (`ByteSize`, `BitRate`, etc.) sono già `struct` immutabili con `ToString()` implementato
- Non serve controllare null sui struct, ma sulle loro property nullable interne
- `Language` ha la property `IsUndetermined` per controllare se è definita
- `TimeSpan` di .NET è struct, confronta con `default` o controlla `TotalMilliseconds > 0`
