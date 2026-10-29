using Avalonia.Media;
using SharpMediaInfo.GUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMediaInfo.GUI.Avalonia.Services
{
    /// <summary>
    /// Avalonia implementation of <see cref="IFontService"/> using the glyph metrics exposed by
    /// <see cref="FontManager"/> / <see cref="GlyphTypeface"/>.
    /// </summary>
    public sealed class AvaloniaFontService : IFontService
    {
        // Preferred monospaced fonts, most-wanted first; the first installed one wins.
        private static readonly string[] PreferredMonospace =
        {
            "Cascadia Mono", "Cascadia Code", "Consolas",
            "DejaVu Sans Mono", "Liberation Mono", "Noto Sans Mono",
            "Menlo", "Monaco", "Courier New", "monospace"
        };

        private readonly HashSet<string> _systemFonts = new(StringComparer.OrdinalIgnoreCase);

        public AvaloniaFontService()
        {
            try
            {
                foreach (var f in FontManager.Current.SystemFonts)
                    _systemFonts.Add(f.Name);
            }
            catch
            {
                // Font manager not ready / headless: leave the set empty.
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
                var glyphTypeface = new Typeface(new FontFamily(fontFamily)).GlyphTypeface;

                // Compare the advance of a narrow and a wide glyph; equal => monospaced.
                var glyphs = glyphTypeface.CharacterToGlyphMap;
                if (!glyphs.TryGetGlyph('i', out var narrow) ||
                    !glyphs.TryGetGlyph('W', out var wide) ||
                    !glyphTypeface.TryGetHorizontalGlyphAdvance(narrow, out var narrowAdvance) ||
                    !glyphTypeface.TryGetHorizontalGlyphAdvance(wide, out var wideAdvance))
                    return false;

                return narrowAdvance == wideAdvance;
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

            // Fall back to any installed monospaced font, then to the first system font.
            var anyMono = _systemFonts.FirstOrDefault(IsMonospace);
            return anyMono ?? _systemFonts.FirstOrDefault() ?? "monospace";
        }
    }
}
