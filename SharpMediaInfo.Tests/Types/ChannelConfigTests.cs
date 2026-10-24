namespace SharpMediaInfo.Tests.Types;

public class ChannelConfigTests
{
    [Test]
    public async Task ExplicitCount_IsUsed()
    {
        var config = new ChannelConfig(6);

        await Assert.That(config.Count).IsEqualTo(6L);
        await Assert.That(config.ToString()).IsEqualTo("6 channels");
    }

    [Test]
    public async Task SingleChannel_IsNotPluralized()
    {
        await Assert.That(new ChannelConfig(1).ToString()).IsEqualTo("1 channel");
    }

    [Test]
    public async Task Count_DerivedFromLayout_WhenCountMissing()
    {
        var config = new ChannelConfig(null, layout: "L R C LFE Ls Rs");

        await Assert.That(config.Count).IsEqualTo(6L);
    }

    [Test]
    public async Task Count_DerivedFromPositions_SkippingPrefixes()
    {
        var config = new ChannelConfig(null, positions: "Front: L C R, Side: L R, LFE");

        await Assert.That(config.Count).IsEqualTo(6L);
    }

    [Test]
    public async Task ToString_IncludesPositionsWhenPresent()
    {
        var config = new ChannelConfig(2, positions: "Front: L R");

        await Assert.That(config.ToString()).IsEqualTo("2 channels (Front: L R)");
    }

    [Test]
    public async Task WhitespacePositions_TreatedAsNull()
    {
        var config = new ChannelConfig(2, positions: "   ");

        await Assert.That(config.Positions).IsNull();
    }

    [Test]
    public async Task NoData_ProducesNullCountAndEmptyString()
    {
        var config = new ChannelConfig(null);

        await Assert.That(config.Count).IsNull();
        await Assert.That(config.ToString()).IsEqualTo(string.Empty);
    }
}
