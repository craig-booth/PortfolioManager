using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Drawing;
using System.Globalization;

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

}
