using System;
using System.Windows.Data;
using System.Globalization;

namespace GetMediaInfoGUI.Control.Converter
{
    /// <summary>
    /// Converter for the Text in the Value Text Box.
    /// Makes sure that the correct decimation is displayed
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DecimationConverter : IValueConverter
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
            double dataValue = (double)value;

            return dataValue.ToString("F" + ((uint)parameter).ToString());
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
            try
            {
                return System.Convert.ToDouble(value);
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}