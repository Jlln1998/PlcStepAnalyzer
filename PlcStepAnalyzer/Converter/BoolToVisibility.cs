using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PlcStepAnalyzer.Converter
{
    public class BoolToVisibility: IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return Visibility.Collapsed;

            bool isNegate = parameter is not null && (string)parameter == "true";

            if (value is bool show)
            {
                if(show)
                {
                    return isNegate ? Visibility.Collapsed : Visibility.Visible;
                }

                return isNegate ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
