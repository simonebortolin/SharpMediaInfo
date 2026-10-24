namespace SharpMediaInfo.Tests.Types;

public class AspectRatioTests
{
    [Test]
    [Arguments(1.778, "16:9")]
    [Arguments(1.333, "4:3")]
    [Arguments(1.6, "16:10")]
    [Arguments(1.85, "1.85:1")]
    [Arguments(1.0, "1:1")]
    public async Task ToString_Ratio_DetectsCommonRatios(double value, string expected)
    {
        var aspect = new AspectRatio(value);

        await Assert.That(aspect.ToString(AspectRatioFormat.Ratio)).IsEqualTo(expected);
    }

    [Test]
    public async Task ToString_Ratio_UnknownValue_FallsBackToXto1()
    {
        var aspect = new AspectRatio(3.12);

        await Assert.That(aspect.ToString(AspectRatioFormat.Ratio)).IsEqualTo("3.12:1");
    }

    [Test]
    public async Task ToString_Decimal_FormatsThreeDecimals()
    {
        var aspect = new AspectRatio(1.7778);

        await Assert.That(aspect.ToString(AspectRatioFormat.Decimal)).IsEqualTo("1.778");
    }

    [Test]
    public async Task Null_ProducesEmptyString()
    {
        var aspect = new AspectRatio(null);

        await Assert.That(aspect.ToString()).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task ImplicitConversions_RoundTrip()
    {
        AspectRatio aspect = (double?)1.5;
        double? value = aspect;

        await Assert.That(value).IsEqualTo(1.5);
    }
}
