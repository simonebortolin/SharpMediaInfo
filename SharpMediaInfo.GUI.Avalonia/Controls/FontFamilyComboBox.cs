using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMediaInfo.GUI.Avalonia.Controls
{
    public partial class FontFamilyComboBox : ComboBox
    {
        protected override Type StyleKeyOverride => typeof(ComboBox);

        [GeneratedStyledProperty(DefaultBindingMode = BindingMode.TwoWay)]
        public partial string? SelectedFontFamily { get; set; }

        private bool _updating;

        public FontFamilyComboBox()
        {
            var fonts = FontManager.Current.SystemFonts
                .OrderBy(f => f.Name)
                .ToList();

            ItemsSource = fonts;

            SelectionChanged += (_, _) => UpdateSelectedFontFamily();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SelectedFontFamilyProperty)
                UpdateSelectedItem();
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
