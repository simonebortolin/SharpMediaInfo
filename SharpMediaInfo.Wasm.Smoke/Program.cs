using SharpMediaInfo;
using SharpMediaInfo.Wasm.Smoke;

// Exercises the native MediaInfoLib linked into the .NET WebAssembly runtime.
// Prints one line per check and returns a non-zero exit code on failure.
var failures = 0;

void Check(string name, bool ok, string detail)
{
    Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name}: {detail}");
    if (!ok) failures++;
}

try
{
    var version = MediaInfo.LibraryVersion();
    Check("library version", version.Contains("MediaInfoLib"), version);

    using (var small = new MemoryStream(VirtualWavStream.CreateSmall()))
    {
        var media = await MediaInfoBuilder.FromStream(small).WithReports().BuildAsync();
        Check("small WAV (BuildAsync)", media.AudioCount == 1 && media.InfoStandard?.Contains("PCM") == true,
            $"audio={media.AudioCount}, format={media.Audio?[0].Format}");
    }

    // 2 GB virtual WAV: more than the 32-bit wasm address space could ever hold in memory.
    // The title lives in a chunk after the data, so MediaInfo must ask for a seek to find it.
    using (var large = new VirtualWavStream(2L * 1024 * 1024 * 1024, title: "Chunked reader"))
    {
        var media = await MediaInfoBuilder.FromStream(large).BuildAsync();
        var title = media.General?[0].Title;
        Check("2 GB virtual WAV (chunked, seeking)",
            media.AudioCount == 1 && title == "Chunked reader" && large.SeekCount >= 1 && large.BytesRead < 16L * 1024 * 1024,
            $"audio={media.AudioCount}, title={title}, read {large.BytesRead / 1024} KB of {large.Length / (1024 * 1024)} MB in {large.SeekCount} seek(s)");
    }

    using (var sync = new MemoryStream(VirtualWavStream.CreateSmall()))
    {
        var media = MediaInfoBuilder.FromStream(sync).Build();
        Check("small WAV (Build, sync)", media.AudioCount == 1, $"audio={media.AudioCount}");
    }
}
catch (Exception ex)
{
    Check("exception", false, ex.ToString());
}

Console.WriteLine(failures == 0 ? "SMOKE OK" : $"SMOKE FAILED ({failures})");
return failures == 0 ? 0 : 1;
