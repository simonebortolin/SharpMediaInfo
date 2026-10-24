namespace SharpMediaInfo.Tests.Types;

public class ByteSizeTests
{
    [Test]
    public async Task Null_ProducesEmptyStringAndNullConversions()
    {
        var size = new ByteSize(null);

        await Assert.That(size.Bytes).IsNull();
        await Assert.That(size.KiloBytes).IsNull();
        await Assert.That(size.MegaBytes).IsNull();
        await Assert.That(size.GigaBytes).IsNull();
        await Assert.That(size.ToString()).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task Conversions_UseBinaryDivisors()
    {
        var size = new ByteSize(1024L * 1024L);

        await Assert.That(size.KiloBytes!.Value).IsEqualTo(1024.0).Within(0.0001);
        await Assert.That(size.MegaBytes!.Value).IsEqualTo(1.0).Within(0.0001);
        await Assert.That(size.GigaBytes!.Value).IsEqualTo(1.0 / 1024.0).Within(0.0001);
    }

    [Test]
    [Arguments(512L, "512 bytes")]
    [Arguments(1536L, "1.5 KiB")]
    [Arguments(1572864L, "1.5 MiB")]
    public async Task ToString_Auto_PicksAppropriateUnit(long bytes, string expected)
    {
        var size = new ByteSize(bytes);

        await Assert.That(size.ToString(ByteSizeFormat.Auto)).IsEqualTo(expected);
    }

    [Test]
    public async Task ToString_AutoWithPercentage_AppendsProportion()
    {
        var size = new ByteSize(1024L, 0.5);

        await Assert.That(size.ToString(ByteSizeFormat.AutoWithPercentage)).IsEqualTo("1.0 KiB (50%)");
    }

    [Test]
    public async Task ToString_AutoWithPercentage_NoProportion_OmitsPercentage()
    {
        var size = new ByteSize(1024L);

        await Assert.That(size.ToString(ByteSizeFormat.AutoWithPercentage)).IsEqualTo("1.0 KiB");
    }

    [Test]
    [Arguments(ByteSizeFormat.Bytes, "2048 bytes")]
    [Arguments(ByteSizeFormat.KiloBytes, "2.00 KiB")]
    public async Task ToString_ExplicitUnit_FormatsAccordingly(ByteSizeFormat format, string expected)
    {
        var size = new ByteSize(2048L);

        await Assert.That(size.ToString(format)).IsEqualTo(expected);
    }
}
