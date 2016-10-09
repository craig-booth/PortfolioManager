using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Drawing;
using System.Globalization;
using System.Windows;

using PortfolioManager.UI.ViewModels;


namespace PortfolioManager.UI.Style
{

    [ValueConversion(typeof(bool), typeof(bool))]
    class NotTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (! (bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (!(bool)value);
        }
    }


    [ValueConversion(typeof(bool), typeof(Visibility))]
    class VisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } 
        public Visibility FalseValue { get; set; }

        public VisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TrueValue: FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value == TrueValue);
        }
    }
}
