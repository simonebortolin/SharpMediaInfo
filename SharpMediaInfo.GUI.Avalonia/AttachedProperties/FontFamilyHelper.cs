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
            FontFamilyStringProperty.Changed.Subscribe(e =>
            {
                if (e.NewValue.Value is not string s)
                {
                    return;
                }

                if (e.Sender is TextBox tb) tb.FontFamily = new FontFamily(s);
                else if (e.Sender is TextBlock tbl) tbl.FontFamily = new FontFamily(s);
                else if (e.Sender is TreeView tr) tr.FontFamily = new FontFamily(s);
            });
        }

        public static string? GetFontFamilyString(Control element)
            => element.GetValue(FontFamilyStringProperty);

        public static void SetFontFamilyString(Control element, string? value)
            => element.SetValue(FontFamilyStringProperty, value);
    }
}
