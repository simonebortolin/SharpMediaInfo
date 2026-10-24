using System.Globalization;

namespace SharpMediaInfo.Tests.Types;

public class LanguageTests
{
    [Test]
    public async Task TwoLetterCode_ResolvesToCulture()
    {
        var language = new Language("en");

        await Assert.That(language.ToCultureInfo()).IsNotNull();
        await Assert.That(language.Code).IsEqualTo("en");
    }

    [Test]
    public async Task ThreeLetterCode_MapsToCulture()
    {
        var language = new Language("ita");

        var culture = language.ToCultureInfo();
        await Assert.That(culture).IsNotNull();
        await Assert.That(culture!.TwoLetterISOLanguageName).IsEqualTo("it");
    }

    [Test]
    public async Task ToString_ReturnsNativeName_ForKnownLanguage()
    {
        var language = new Language("ita");

        await Assert.That(language.ToString()).IsEqualTo(new CultureInfo("it").NativeName);
    }

    [Test]
    public async Task KnownCode_ToString_IsNotTheRawCode()
    {
        // A resolvable code returns a human-readable native name rather than the raw code.
        var language = new Language("de");

        await Assert.That(language.ToString()).IsEqualTo(new CultureInfo("de").NativeName);
    }

    [Test]
    public async Task NullOrWhitespace_NormalizesToNull()
    {
        await Assert.That(new Language("   ").Code).IsNull();
        await Assert.That(new Language(null).ToString()).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task FromCultureInfo_PrefersFullName()
    {
        var language = Language.FromCultureInfo(new CultureInfo("en-US"));

        await Assert.That(language.Code).IsEqualTo("en-US");
    }

    [Test]
    public async Task ImplicitConversion_FromString()
    {
        Language language = "fr";

        await Assert.That(language.Code).IsEqualTo("fr");
    }
}
