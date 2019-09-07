using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBControls;

using PortfolioManager.RestApi.Client;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class LogonViewModel : PopupWindow
    {
        private readonly RestClient _RestClient;
        public string UserName { get; set; }
        public string Password { get; set; }

        public LogonViewModel(RestClient restClient)
        {
            _RestClient = restClient;
            LogonCommand = new RelayCommand(Logon);
            CancelCommand = new RelayCommand(Cancel);

            Commands.Add(new DialogCommand("Logon", LogonCommand) { IsDefault = true });
            Commands.Add(new DialogCommand("Cancel", CancelCommand) { IsCancel = true });
        }

        public RelayCommand LogonCommand { get; private set; }
        private async void Logon()
        {
            var result = await _RestClient.Authenticate(UserName, Password);

            if (result)
            {
                _RestClient.SetPortfolio(new Guid("5D5DE669-726C-4C5D-BB2E-6520C924DB90"));

                Close();

                OnLoggedOn(EventArgs.Empty);
            }

        }

        public RelayCommand CancelCommand { get; private set; }
        private void Cancel()
        {
            Close();
        }

        public event EventHandler LoggedOn;

        private void OnLoggedOn(EventArgs e)
        {
            var handler = LoggedOn;
            if (handler != null)
                handler(this, e);
        }
    }
}
