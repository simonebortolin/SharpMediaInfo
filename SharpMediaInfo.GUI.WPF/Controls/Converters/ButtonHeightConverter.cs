using System;
using System.Windows.Data;
using System.Globalization;

namespace SharpMediaInfo.GUI.WPF.Controls.Converters
{
    /// <summary>
    /// Value Conversion Class for the button height.
    /// Divides the Hight of the control by two to get the height for one button.
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class ButtonHeightConverter : IValueConverter
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
            return (double)value / 2.0;
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