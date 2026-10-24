using SharpMediaInfo.GUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace SharpMediaInfo.GUI.WPF.Services
{
    /// <summary>
    /// WPF implementation of <see cref="IFontService"/> using <see cref="GlyphTypeface"/> metrics.
    /// </summary>
    public sealed class WpfFontService : IFontService
    {
        private static readonly string[] PreferredMonospace =
        {
            "Cascadia Mono", "Cascadia Code", "Consolas",
            "Courier New", "Lucida Console", "DejaVu Sans Mono"
        };

        private readonly HashSet<string> _systemFonts = new(StringComparer.OrdinalIgnoreCase);

        public WpfFontService()
        {
            try
            {
                foreach (var f in Fonts.SystemFontFamilies)
                    _systemFonts.Add(f.Source);
            }
            catch
            {
                // ignore
            }
        }

        public bool IsAvailable(string? fontFamily)
            => !string.IsNullOrWhiteSpace(fontFamily) && _systemFonts.Contains(fontFamily!);

        public bool IsMonospace(string? fontFamily)
        {
            if (string.IsNullOrWhiteSpace(fontFamily))
                return false;

            try
            {
                var typeface = new Typeface(fontFamily);
                if (!typeface.TryGetGlyphTypeface(out var gt))
                    return false;

                if (!gt.CharacterToGlyphMap.TryGetValue('i', out var narrow) ||
                    !gt.CharacterToGlyphMap.TryGetValue('W', out var wide))
                    return false;

                return Math.Abs(gt.AdvanceWidths[narrow] - gt.AdvanceWidths[wide]) < 1e-6;
            }
            catch
            {
                return false;
            }
        }

        public string GetDefaultReportFont()
        {
            var preferred = PreferredMonospace.FirstOrDefault(IsAvailable);
            if (preferred != null)
                return preferred;

            var anyMono = _systemFonts.FirstOrDefault(IsMonospace);
            return anyMono ?? _systemFonts.FirstOrDefault() ?? "Consolas";
        }
    }
}
