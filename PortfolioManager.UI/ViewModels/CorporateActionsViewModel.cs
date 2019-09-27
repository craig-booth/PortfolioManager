using System;
using System.Collections.ObjectModel;
using System.Linq;

using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.UI.Utilities;
using PortfolioManager.UI.ViewModels.Transactions;

namespace PortfolioManager.UI.ViewModels
{

    class CorporateActionsViewModel : PortfolioViewModel
    {
        private CreateMulitpleTransactionsViewModel _CreateTransactionsViewModel;

        public CorporateActionsViewModel(string label, ViewParameter parameter, CreateMulitpleTransactionsViewModel createTransactionsViewModel)
            : base(label, parameter)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.None;

            _Heading = label;
            _CreateTransactionsViewModel = createTransactionsViewModel;

            CorporateActions = new ObservableCollection<CorporateActionViewModel>();

            ApplyCorporateActionCommand = new RelayCommand<CorporateActionViewModel>(ApplyCorporateAction);
        }

        private string _Heading;
        new public string Heading
        {
            get
            {
                return _Heading;
            }
            private set
            {
                _Heading = value;
                OnPropertyChanged();
            }
        } 

        public ObservableCollection<CorporateActionViewModel> CorporateActions { get; private set; }

        public RelayCommand<CorporateActionViewModel> ApplyCorporateActionCommand { get; private set; }
        private async void ApplyCorporateAction(CorporateActionViewModel corporateAction)
        {
            if (corporateAction != null)
            {
                var transactions = await _Parameter.RestClient.Transactions.GetTransactionsForCorporateAction(corporateAction.Stock.Id, corporateAction.Id);
                
                _CreateTransactionsViewModel.AddTransactions(transactions);
            } 
        }

        public async override void RefreshView()
        {
            var response = await _Parameter.RestClient.Portfolio.GetCorporateActions();
            if (response == null)
                return;

            CorporateActions.Clear();
            
            foreach (var corporateAction in response.CorporateActions.OrderBy(x => x.ActionDate))
                CorporateActions.Add(new CorporateActionViewModel(corporateAction));

            OnPropertyChanged(""); 
        }

    }

    class CorporateActionViewModel
    {
        public Guid Id { get; set; }
        public DateTime ActionDate { get; set; }
        public StockViewItem Stock {get; set;}
        public string Description { get; set; }

        public CorporateActionViewModel(CorporateActionsResponse.CorporateActionItem corporateAction)
        {
            Id = corporateAction.Id;
            ActionDate = corporateAction.ActionDate;
            Stock = new StockViewItem(corporateAction.Stock);
            Description = corporateAction.Description;
        }
    }

}
