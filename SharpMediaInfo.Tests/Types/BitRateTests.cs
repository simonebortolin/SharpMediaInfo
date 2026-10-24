namespace SharpMediaInfo.Tests.Types;

public class BitRateTests
{
    [Test]
    public async Task Null_ProducesNullConversionsAndEmptyString()
    {
        var rate = new BitRate(null);

        await Assert.That(rate.BitsPerSecond).IsNull();
        await Assert.That(rate.KiloBitsPerSecond).IsNull();
        await Assert.That(rate.MegaBitsPerSecond).IsNull();
        await Assert.That(rate.ToString()).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task Conversions_UseDecimalDivisors()
    {
        var rate = new BitRate(2_000_000);

        await Assert.That(rate.BitsPerSecond).IsEqualTo(2_000_000L);
        await Assert.That(rate.KiloBitsPerSecond!.Value).IsEqualTo(2000.0).Within(0.0001);
        await Assert.That(rate.MegaBitsPerSecond!.Value).IsEqualTo(2.0).Within(0.0001);
    }

    [Test]
    [Arguments(500L, "500 Bps")]
    [Arguments(1024L, "1 Kbps")]
    [Arguments(1536L, "1.5 Kbps")]
    [Arguments(1048576L, "1 Mbps")]
    [Arguments(1073741824L, "1 Gbps")]
    [Arguments(1099511627776L, "1 Tbps")]
    public async Task ToString_FormatsWithBinaryThresholds(long bps, string expected)
    {
        var rate = new BitRate(bps);

        await Assert.That(rate.ToString()).IsEqualTo(expected);
    }

    [Test]
    public async Task ImplicitConversion_ReturnsBitsPerSecond()
    {
        long? value = new BitRate(128_000);

        await Assert.That(value).IsEqualTo(128_000L);
    }
}
