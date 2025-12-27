using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMediaInfo.GUI.Avalonia.Controls
{
    public class FontFamilyComboBox : ComboBox
    {
        protected override Type StyleKeyOverride => typeof(ComboBox);

        public static readonly StyledProperty<string?> SelectedFontFamilyProperty =
            AvaloniaProperty.Register<FontFamilyComboBox, string?>(
                nameof(SelectedFontFamily),
                defaultBindingMode: BindingMode.TwoWay);

        public string? SelectedFontFamily
        {
            get => GetValue(SelectedFontFamilyProperty);
            set => SetValue(SelectedFontFamilyProperty, value);
        }

        private bool _updating;

        public FontFamilyComboBox()
        {
            var fonts = FontManager.Current.SystemFonts
                .OrderBy(f => f.Name)
                .ToList();

            ItemsSource = fonts;

            SelectionChanged += (_, _) => UpdateSelectedFontFamily();
            this.GetObservable(SelectedFontFamilyProperty)
                .Subscribe(_ => UpdateSelectedItem());
        }

        private void UpdateSelectedItem()
        {
            if (_updating)
                return;

            _updating = true;

            if (SelectedFontFamily == null)
            {
                SelectedItem = null;
            }
            else
            {
                SelectedItem = ((IEnumerable<FontFamily>)ItemsSource!)
                    .FirstOrDefault(f => f.Name == SelectedFontFamily);
            }

            _updating = false;
        }

        private void UpdateSelectedFontFamily()
        {
            if (_updating)
                return;

            _updating = true;

            SelectedFontFamily =
                (SelectedItem as FontFamily)?.Name;

            _updating = false;
        }
    }
}
