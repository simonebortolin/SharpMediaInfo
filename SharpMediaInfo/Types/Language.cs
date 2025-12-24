using System.Globalization;

namespace SharpMediaInfo.Types
{
    /// <summary>
    /// Represents language information with ISO 639 codes and CultureInfo support.
    /// MediaInfo already provides the best available code format.
    /// </summary>
    public readonly struct Language
    {
        /// <summary>
        /// Gets the language code. Can be ISO 639-1 (2 letters), ISO 639-2 (3 letters),
        /// or with country code (e.g., "en", "eng", "en-US").
        /// </summary>
        public string? Code { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> struct.
        /// </summary>
        /// <param name="code">Language code from MediaInfo (already formatted optimally)</param>
        public Language(string? code)
        {
            Code = string.IsNullOrWhiteSpace(code) ? null : code;
        }

        /// <summary>
        /// Creates a Language from a CultureInfo.
        /// </summary>
        /// <param name="culture">The CultureInfo to convert</param>
        /// <returns>A Language instance</returns>
        public static Language FromCultureInfo(CultureInfo culture)
        {
            if (culture == null)
                return default;

            // Prefer full culture name (e.g., "en-US"), fallback to two-letter code
            return new Language(
                !string.IsNullOrEmpty(culture.Name) ? culture.Name : culture.TwoLetterISOLanguageName
            );
        }

        /// <summary>
        /// Attempts to convert this Language to a CultureInfo.
        /// </summary>
        /// <returns>A CultureInfo if conversion is possible, null otherwise</returns>
        public CultureInfo? ToCultureInfo()
        {
            if (string.IsNullOrEmpty(Code))
                return null;

            try
            {
                // Try direct conversion first (works for "en", "en-US", etc.)
                return CultureInfo.GetCultureInfo(Code);
            }
            catch (CultureNotFoundException)
            {
                // Try mapping three-letter ISO 639-2 to two-letter ISO 639-1
                var twoLetterCode = MapThreeLetterToTwoLetter(Code);
                if (twoLetterCode != null)
                {
                    try
                    {
                        return CultureInfo.GetCultureInfo(twoLetterCode);
                    }
                    catch (CultureNotFoundException)
                    {
                        return null;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the human-readable language name if available, otherwise the language code.
        /// </summary>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Code))
                return string.Empty;

            var culture = ToCultureInfo();
            if (culture != null)
            {
                // Return native name (e.g., "Italiano" for Italian, "English" for English)
                return culture.NativeName;
            }

            // Fallback to the code if we can't resolve it
            return Code;
        }

        private static string? MapThreeLetterToTwoLetter(string threeLetterCode)
        {
            // Common ISO 639-2/B to ISO 639-1 mappings
            return threeLetterCode?.ToLowerInvariant() switch
            {
                "eng" => "en",
                "ita" => "it",
                "fra" or "fre" => "fr",
                "deu" or "ger" => "de",
                "spa" => "es",
                "por" => "pt",
                "rus" => "ru",
                "jpn" => "ja",
                "chi" or "zho" => "zh",
                "kor" => "ko",
                "ara" => "ar",
                "hin" => "hi",
                "pol" => "pl",
                "nld" or "dut" => "nl",
                "swe" => "sv",
                "nor" => "no",
                "dan" => "da",
                "fin" => "fi",
                "ces" or "cze" => "cs",
                "ell" or "gre" => "el",
                "heb" => "he",
                "tur" => "tr",
                "vie" => "vi",
                "tha" => "th",
                "ron" or "rum" => "ro",
                "hun" => "hu",
                "ukr" => "uk",
                "cat" => "ca",
                "hrv" => "hr",
                "slk" or "slo" => "sk",
                "bul" => "bg",
                "srp" => "sr",
                "ind" => "id",
                "msa" or "may" => "ms",
                "fas" or "per" => "fa",
                "est" => "et",
                "lav" => "lv",
                "lit" => "lt",
                "slv" => "sl",
                "isl" or "ice" => "is",
                _ => null
            };
        }

        /// <summary>
        /// Implicitly converts to string (returns code).
        /// </summary>
        public static implicit operator string?(Language language) => language.Code;

        /// <summary>
        /// Implicitly converts from string to Language.
        /// </summary>
        public static implicit operator Language(string? code) => new Language(code);
    }
}
