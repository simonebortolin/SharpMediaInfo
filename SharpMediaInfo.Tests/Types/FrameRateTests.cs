namespace SharpMediaInfo.Tests.Types;

public class FrameRateTests
{
    [Test]
    public async Task RationalConstructor_ComputesValue()
    {
        var rate = new FrameRate(30000L, 1001L);

        await Assert.That(rate.Value!.Value).IsEqualTo(29.97).Within(0.01);
        await Assert.That(rate.Numerator).IsEqualTo(30000L);
        await Assert.That(rate.Denominator).IsEqualTo(1001L);
    }

    [Test]
    public async Task RationalConstructor_ZeroDenominator_LeavesValueNull()
    {
        var rate = new FrameRate(30000L, 0L);

        await Assert.That(rate.Value).IsNull();
    }

    [Test]
    public async Task ToString_ValueOnly()
    {
        var rate = new FrameRate(25.0);

        await Assert.That(rate.ToString()).IsEqualTo("25 FPS");
    }

    [Test]
    public async Task ToString_ValueAndRational_IncludesFraction()
    {
        var rate = new FrameRate(29.97, 30000L, 1001L);

        await Assert.That(rate.ToString()).IsEqualTo("29.97 (30000/1001) FPS");
    }

    [Test]
    public async Task ToString_Empty_WhenNothingSet()
    {
        var rate = new FrameRate((double?)null);

        await Assert.That(rate.ToString()).IsEqualTo(string.Empty);
    }
}
