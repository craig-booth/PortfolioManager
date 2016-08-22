using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class SettingsViewModel : ViewModel
    {
        public ApplicationSettings Settings { get; private set; }
        
        public SettingsViewModel(string label, ApplicationSettings settings)
            : base(label)
        {
            SaveSettingsCommand = new Utilities.RelayCommand(SaveSettings);

            Settings = settings;
        }

        public RelayCommand SaveSettingsCommand { get; private set; }
        private void SaveSettings()
        {
            Settings.Save();
        }
    }
}
