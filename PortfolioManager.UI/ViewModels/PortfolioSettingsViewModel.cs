using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class PortfolioSettingsViewModel : PortfolioViewModel
    {
        public Guid PortfolioId { get; set; }
        public string PortfolioName { get; set; }

        public DateTime PortfolioStartDate { get ; set;}
        public DateTime PortfolioEndDate { get; set; }

        public ObservableCollection<HoldingSettingsItem> HoldingSettings { get; set; } = new ObservableCollection<HoldingSettingsItem>();

        public PortfolioSettingsViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {

        }

        public async override void RefreshView()
        {
            var response = await _Parameter.RestClient.Portfolio.GetProperties();
            if (response == null)
                return;

            PortfolioId = response.Id;
            PortfolioName = response.Name;
            PortfolioStartDate = response.StartDate;
            PortfolioEndDate = response.EndDate;

            HoldingSettings.Clear();
            foreach (var holding in response.Holdings.OrderBy(x => x.Stock.Name))
                HoldingSettings.Add(new HoldingSettingsItem(holding)
                {
                    ChangeDrpParticipationCommand = new RelayCommand<HoldingSettingsItem>(ChangeDrpParticipation)
                });

            OnPropertyChanged("");
        }

        private async void ChangeDrpParticipation(HoldingSettingsItem holding)
        {
            await _Parameter.RestClient.Holdings.ChangeDrpParticipation(holding.Stock.Id, holding.ParticipateInDrp);
        }
    }

    class HoldingSettingsItem
    {
        public StockViewItem Stock { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool ParticipateInDrp { get; set; }

        public RelayCommand<HoldingSettingsItem> ChangeDrpParticipationCommand { get; set; }

        public HoldingSettingsItem(HoldingProperties properties)
        {
            Stock = new StockViewItem(properties.Stock);
            StartDate = properties.StartDate;
            EndDate = properties.EndDate;
            ParticipateInDrp = properties.ParticipatingInDrp;
        }
    }
}
