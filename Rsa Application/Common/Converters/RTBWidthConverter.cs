using System;
using System.Globalization;
using System.Windows.Data;

namespace Rsa_Application.Common.Converters
{
    internal class RTBWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = ((double)value - 54) / 2.0;
            if (x < 300)
                x = x * 2 + 16;
            return x;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
