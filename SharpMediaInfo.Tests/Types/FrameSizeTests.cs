namespace SharpMediaInfo.Tests.Types;

public class FrameSizeTests
{
    [Test]
    public async Task ToString_WithUnit_Pluralizes()
    {
        await Assert.That(new FrameSize(1920L).ToString(FrameSizeFormat.WithUnit)).IsEqualTo("1920 pixels");
        await Assert.That(new FrameSize(1L).ToString(FrameSizeFormat.WithUnit)).IsEqualTo("1 pixel");
    }

    [Test]
    public async Task ToString_ValueOnly_OmitsUnit()
    {
        await Assert.That(new FrameSize(1080L).ToString(FrameSizeFormat.ValueOnly)).IsEqualTo("1080");
    }

    [Test]
    public async Task ToString_WithSpaces_UsesSpaceThousandsSeparator()
    {
        await Assert.That(new FrameSize(1920L).ToString(FrameSizeFormat.WithSpaces)).IsEqualTo("1 920 pixels");
    }

    [Test]
    public async Task CustomUnit_IsRespected()
    {
        await Assert.That(new FrameSize(80L, "character").ToString(FrameSizeFormat.WithUnit)).IsEqualTo("80 characters");
    }

    [Test]
    public async Task NullUnit_DefaultsToPixel()
    {
        await Assert.That(new FrameSize(10L, null!).Unit).IsEqualTo("pixel");
    }

    [Test]
    public async Task Null_ProducesEmptyString()
    {
        await Assert.That(new FrameSize(null).ToString()).IsEqualTo(string.Empty);
    }
}
