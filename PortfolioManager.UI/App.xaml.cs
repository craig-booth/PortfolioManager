using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;

using PortfolioManager.UI.Utilities;
using PortfolioManager.UI.ViewModels;

namespace PortfolioManager.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {


            // Ensure the current culture passed into bindings is the OS culture.
            // By default, WPF uses en-US as the culture, regardless of the system settings.
            FrameworkElement.LanguageProperty.OverrideMetadata(
                    typeof(FrameworkElement),
                    new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            var navigator = Application.Current.FindResource("ViewNavigator") as ViewNavigator;

            navigator.RegisterViewModel("PortfolioSummary", new PortfolioSummaryViewModel());
            navigator.RegisterViewModel("TransactionSummary", new TransactionSummaryViewModel());
            navigator.RegisterViewModel("HoldingSummary", new HoldingSummaryViewModel());
            navigator.RegisterViewModel("TaxableIncome", new TaxableIncomeViewModel());
            navigator.RegisterViewModel("CGT", new CGTViewModel());
        }
    }
}
