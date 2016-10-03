using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Service;
using PortfolioManager.Service.Utils;

using PortfolioManager.UI.Utilities;

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
        private void ApplyCorporateAction(CorporateActionViewModel corporateAction)
        {
            if (corporateAction != null)
            {
                var transactions = _Parameter.Portfolio.CorporateActionService.CreateTransactionList(corporateAction.CorporateAction);

                _CreateTransactionsViewModel.AddTransactions(transactions);
            }
        }

        public override void RefreshView()
        {
            CorporateActions.Clear();

            var corporateActions = _Parameter.Portfolio.CorporateActionService.GetUnappliedCorporateActions();
            
            foreach (var corporateAction in corporateActions)
                CorporateActions.Add(new CorporateActionViewModel(corporateAction, _Parameter.Portfolio.StockService));

            OnPropertyChanged(""); 
        }

    }

    class CorporateActionViewModel
    {
        public CorporateAction CorporateAction { get; private set; }
        public Guid Id { get; set; }
        public DateTime ActionDate { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }

        public CorporateActionViewModel(CorporateAction corporateAction, StockService stockService)
        {
            CorporateAction = corporateAction;
            Id = corporateAction.Id;
            ActionDate = corporateAction.ActionDate;

            var stock= stockService.Get(corporateAction.Stock, ActionDate);
            if (stock != null)
                CompanyName = string.Format("{0} ({1})", stock.Name, stock.ASXCode);

            Description = corporateAction.Description;
        }
    }

}
