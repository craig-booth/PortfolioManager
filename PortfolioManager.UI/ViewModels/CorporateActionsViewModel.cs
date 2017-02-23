using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Service.Interface;

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
                var corporateActionService = _Parameter.PortfolioManagerService.GetService<ICorporateActionService>();

                var responce = await corporateActionService.TransactionsForCorporateAction(corporateAction.Id);

                _CreateTransactionsViewModel.AddTransactions(responce.Transactions);
            } 
        }

        public async override void RefreshView()
        {
            var corporateActionService = _Parameter.PortfolioManagerService.GetService<ICorporateActionService>();

            var responce = await corporateActionService.GetUnappliedCorporateActions();

            CorporateActions.Clear();
            
            foreach (var corporateAction in responce.CorporateActions)
                CorporateActions.Add(new CorporateActionViewModel(corporateAction));

            OnPropertyChanged(""); 
        }

    }

    class CorporateActionViewModel
    {
        public Guid Id { get; set; }
        public DateTime ActionDate { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }

        public CorporateActionViewModel(CorporateActionItem corporateAction)
        {
            Id = corporateAction.Id;
            ActionDate = corporateAction.ActionDate;
            CompanyName = PortfolioViewModel.FormattedCompanyName(corporateAction.Stock);
            Description = corporateAction.Description;
        }
    }

}
