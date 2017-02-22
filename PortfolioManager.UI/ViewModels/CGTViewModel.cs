using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

using PortfolioManager.Service.Interface;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class CGTViewModel : PortfolioViewModel
    {
  
        public ObservableCollection<CGTEventViewModel> CGTEvents { get; private set; }

        public decimal CurrentYearCapitalGainsOther { get; private set; }
        public decimal CurrentYearCapitalGainsDiscounted { get; private set; }
        public decimal CurrentYearCapitalGainsTotal { get; private set; }
        public decimal CurrentYearCapitalLossesOther { get; private set; }
        public decimal CurrentYearCapitalLossesDiscounted { get; private set; }
        public decimal CurrentYearCapitalLossesTotal { get; private set; }
        public decimal GrossCapitalGainOther { get; private set; }
        public decimal GrossCapitalGainDiscounted { get; private set; }
        public decimal GrossCapitalGainTotal { get; private set; }
        public decimal Discount { get; private set; }
        public decimal NetCapitalGainOther { get; private set; }
        public decimal NetCapitalGainDiscounted { get; private set; }
        public decimal NetCapitalGainTotal { get; private set; }

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

        public CGTViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.FinancialYear;

            CGTEvents = new ObservableCollection<CGTEventViewModel>();
        }

        public async override void RefreshView()
        {
            var capitalGainService = _Parameter.PortfolioManagerService.GetService<ICapitalGainService>();

            Heading = string.Format("CGT Report for {0}/{1} Financial Year", _Parameter.FinancialYear - 1, _Parameter.FinancialYear);

            var responce = await capitalGainService.GetCGTLiability(DateUtils.StartOfFinancialYear(_Parameter.FinancialYear), DateUtils.EndOfFinancialYear(_Parameter.FinancialYear));

            CurrentYearCapitalGainsOther = responce.CurrentYearCapitalGainsDiscounted;
            CurrentYearCapitalGainsDiscounted = responce.CurrentYearCapitalGainsDiscounted;
            CurrentYearCapitalGainsTotal = responce.CurrentYearCapitalGainsTotal;
            CurrentYearCapitalLossesOther = responce.CurrentYearCapitalLossesOther;
            CurrentYearCapitalLossesDiscounted = responce.CurrentYearCapitalLossesDiscounted;
            CurrentYearCapitalLossesTotal = responce.CurrentYearCapitalLossesTotal;
            GrossCapitalGainOther = responce.GrossCapitalGainOther;
            GrossCapitalGainDiscounted = responce.GrossCapitalGainDiscounted;
            GrossCapitalGainTotal = responce.GrossCapitalGainTotal;
            Discount = responce.Discount;
            NetCapitalGainOther = responce.NetCapitalGainOther;
            NetCapitalGainDiscounted = responce.NetCapitalGainDiscounted;
            NetCapitalGainTotal = responce.NetCapitalGainTotal;

            CGTEvents.Clear();
            foreach (var item in responce.Items)
                CGTEvents.Add(new CGTEventViewModel(item));

            OnPropertyChanged("");
        }
    }

    class CGTEventViewModel
    {
        public string ASXCode { get; private set; }
        public string CompanyName { get; private set; }
        public DateTime EventDate { get; private set; }
        public decimal CostBase { get; private set; }
        public decimal AmountReceived { get; private set; }
        public decimal CapitalGain { get; private set; }
        public string Method { get; private set; }

        public CGTEventViewModel(CGTLiabilityItem cgtItem)
        {
            ASXCode = cgtItem.ASXCode;
            CompanyName = PortfolioViewModel.FormattedCompanyName(cgtItem.ASXCode, cgtItem.CompanyName);
            EventDate = cgtItem.EventDate;
            CostBase = cgtItem.CostBase;
            AmountReceived = cgtItem.AmountReceived;
            CapitalGain = cgtItem.CapitalGain;
            if (cgtItem.Method == CGTMethod.Discount)
                Method = "Discount";
            else if (cgtItem.Method == CGTMethod.Indexation)
                Method = "Indexation";
            else
                Method = "Other";

        }
    }

}
