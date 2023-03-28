using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rsa_Application.Common.Converters
{
    internal class RTBHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double x = ((double)values[0] - 54) / 2.0;
            double y = (double)values[1] - 139.5;
            if (x < 300)
                y = y / 2 - 8;
            return y;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
