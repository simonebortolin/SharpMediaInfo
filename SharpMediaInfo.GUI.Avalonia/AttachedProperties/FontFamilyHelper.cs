using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using System;

namespace SharpMediaInfo.GUI.Avalonia.AttachedProperties
{
    public static class FontFamilyHelper
    {
        public static readonly AttachedProperty<string?> FontFamilyStringProperty =
            AvaloniaProperty.RegisterAttached<Control, string?>(
                "FontFamilyString",
                typeof(FontFamilyHelper),
                defaultBindingMode: BindingMode.TwoWay);

        static FontFamilyHelper()
        {
            FontFamilyStringProperty.Changed.AddClassHandler<Control>((sender, e) =>
            {
                if (e.GetNewValue<string?>() is not string s)
                {
                    return;
                }

                if (sender is TextBox tb) tb.FontFamily = new FontFamily(s);
                else if (sender is TextBlock tbl) tbl.FontFamily = new FontFamily(s);
                else if (sender is TreeView tr) tr.FontFamily = new FontFamily(s);
            });
        }

        public static string? GetFontFamilyString(Control element)
            => element.GetValue(FontFamilyStringProperty);

        public static void SetFontFamilyString(Control element, string? value)
            => element.SetValue(FontFamilyStringProperty, value);
    }
}
