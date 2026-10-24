namespace SharpMediaInfo.Native.Tests;

/// <summary>
/// Exercises the static library-information surface. Passing these proves that the
/// native library resolves and loads correctly via the DllImport resolver.
/// </summary>
public class LibraryInfoTests
{
    [Test]
    public async Task LibraryVersion_IsReportedAndNonEmpty()
    {
        var version = IMediaInfoNative.GetLibraryInfo("Info_Version");

        await Assert.That(version).IsNotNull();
        await Assert.That(version).IsNotEmpty();
        // MediaInfo reports something like "MediaInfoLib - v25.10"
        await Assert.That(version).Contains("MediaInfo");
    }

    [Test]
    public async Task Capacities_AreListed()
    {
        var capacities = IMediaInfoNative.GetLibraryInfo("Info_Capacities");

        await Assert.That(capacities).IsNotNull();
        await Assert.That(capacities).IsNotEmpty();
    }

    [Test]
    public async Task ParametersCsv_ContainsKnownColumns()
    {
        var parameters = IMediaInfoNative.GetLibraryInfo("Info_Parameters_CSV");

        await Assert.That(parameters).IsNotEmpty();
        // The CSV parameter listing always contains the General "Format" parameter.
        await Assert.That(parameters).Contains("Format");
    }

    [Test]
    public async Task KnownCodecs_AreListed()
    {
        var codecs = IMediaInfoNative.GetLibraryInfo("Info_Codecs");

        await Assert.That(codecs).IsNotEmpty();
    }
}
