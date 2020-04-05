using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace GetMediaInfoGUI.Controls.Converters
{
    /// <summary>
    /// Value Converter for the Button Show Property.
    /// Converts from <see cref="bool"/> to <see cref="Visibility"/>
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class ButtonVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (System.Convert.ToBoolean(value))
            {
                return System.Windows.Visibility.Visible;
            }
            else
            {
                return System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}