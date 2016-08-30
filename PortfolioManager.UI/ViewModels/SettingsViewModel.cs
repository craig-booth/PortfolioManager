using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class SettingsViewModel : PageViewModel
    {
        public ApplicationSettings _Settings;

        public string PortfolioDatabasePath { get; set; }
        public string StockDatabasePath { get; set; }

        public SettingsViewModel(string label, ApplicationSettings settings)
            : base(label)
        {
            SaveSettingsCommand = new Utilities.RelayCommand(SaveSettings);

            _Settings = settings;

            PortfolioDatabasePath = _Settings.PortfolioDatabase;
            StockDatabasePath = _Settings.StockDatabase;
        }

        public RelayCommand SaveSettingsCommand { get; private set; }
        private void SaveSettings()
        {
            bool databaseChanged = false;

            if ((PortfolioDatabasePath != _Settings.PortfolioDatabase)
                || (StockDatabasePath != _Settings.StockDatabase))
            {
                _Settings.PortfolioDatabase = PortfolioDatabasePath;
                _Settings.StockDatabase = StockDatabasePath;

                databaseChanged = true;
            }

            _Settings.Save();

            if (databaseChanged)
                _Settings.OnDatabaseChanged();
        }
    }
}
