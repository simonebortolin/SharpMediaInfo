namespace SharpMediaInfo.Tests.Types;

public class MeasuredValueTests
{
    [Test]
    public async Task ToString_IncludesUnit()
    {
        var measured = new MeasuredValue<int>(48000, "Hz");

        await Assert.That(measured.ToString()).IsEqualTo("48000 Hz");
    }

    [Test]
    public async Task ToString_NoUnit_ReturnsValueOnly()
    {
        var measured = new MeasuredValue<int>(42, "");

        await Assert.That(measured.ToString()).IsEqualTo("42");
    }

    [Test]
    public async Task Null_ProducesEmptyString()
    {
        var measured = new MeasuredValue<int>(null, "Hz");

        await Assert.That(measured.ToString()).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task ImplicitConversion_ReturnsUnderlyingValue()
    {
        int? value = new MeasuredValue<int>(7, "px");

        await Assert.That(value).IsEqualTo(7);
    }

    [Test]
    public async Task NullUnit_NormalizedToEmpty()
    {
        var measured = new MeasuredValue<double>(1.5, null!);

        await Assert.That(measured.Unit).IsEqualTo(string.Empty);
    }
}
