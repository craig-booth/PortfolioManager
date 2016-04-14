using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
            var navigator = Application.Current.FindResource("ViewNavigator") as ViewNavigator;

            navigator.RegisterViewModel("PortfolioSummary", new PortfolioSummaryViewModel());
            navigator.RegisterViewModel("TransactionSummary", new TransactionSummaryViewModel());
            navigator.RegisterViewModel("HoldingSummary", new HoldingSummaryViewModel());
        }
    }
}
