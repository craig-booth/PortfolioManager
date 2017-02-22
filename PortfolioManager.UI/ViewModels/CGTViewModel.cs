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

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class CGTViewModel : PortfolioViewModel
    {
  
        public ObservableCollection<CGTEventViewModel> CGTEvents { get; private set; }

        public decimal CurrentYearCapitalGainsOther { get; private set; }
        public decimal CurrentYearCapitalGainsDiscounted { get; private set; }
        public decimal CurrentYearCapitalGainsTotal
        {
            get
            {
                return CurrentYearCapitalGainsOther + CurrentYearCapitalGainsDiscounted;
            }
        }

        public decimal CurrentYearCapitalLossesOther
        {
            get
            {
                if (CurrentYearCapitalGainsOther > CurrentYearCapitalLossesTotal)
                    return CurrentYearCapitalLossesTotal;
                else
                    return CurrentYearCapitalGainsOther;
            }
        }

        public decimal CurrentYearCapitalLossesDiscounted
        {
            get
            {
                if (CurrentYearCapitalGainsOther > CurrentYearCapitalLossesTotal)
                    return 0.00m;
                else
                    return CurrentYearCapitalLossesTotal - CurrentYearCapitalGainsOther;
            }
        }
        public decimal CurrentYearCapitalLossesTotal { get; private set; }

        public decimal GrossCapitalGainOther
        {
            get
            {
                return CurrentYearCapitalGainsOther - CurrentYearCapitalLossesOther;
            }
        }
        public decimal GrossCapitalGainDiscounted
        {
            get
            {
                return CurrentYearCapitalGainsDiscounted - CurrentYearCapitalLossesDiscounted;
            }
        }
        public decimal GrossCapitalGainTotal
        {
            get
            {
                return GrossCapitalGainOther + GrossCapitalGainDiscounted;
            }
        }

        public decimal Discount
        {
            get
            {
                if (GrossCapitalGainDiscounted > 0)
                    return (GrossCapitalGainDiscounted / 2).ToCurrency(RoundingRule.Round);
                else
                    return 0.00m;
            }
        }


        public decimal NetCapitalGainOther
        {
            get
            {
                return GrossCapitalGainOther;
            }
        }
        public decimal NetCapitalGainDiscounted
        {
            get
            {
                return GrossCapitalGainDiscounted - Discount;
            }
        }
        public decimal NetCapitalGainTotal
        {
            get
            {
                return NetCapitalGainOther + NetCapitalGainDiscounted;
            }
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

        public CGTViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.FinancialYear;

            CGTEvents = new ObservableCollection<CGTEventViewModel>();
        }

        public override void RefreshView()
        {
            Heading = string.Format("CGT Report for {0}/{1} Financial Year", _Parameter.FinancialYear - 1, _Parameter.FinancialYear);

            // Get a list of all the cgt events for the year
            var cgtEvents = _Parameter.Portfolio.CGTService.GetEvents(DateUtils.StartOfFinancialYear(_Parameter.FinancialYear), DateUtils.EndOfFinancialYear(_Parameter.FinancialYear));

            CGTEvents.Clear();
            foreach (var cgtEvent in cgtEvents)
                CGTEvents.Add(new CGTEventViewModel(_Parameter.Portfolio.StockService.Get(cgtEvent.Stock, cgtEvent.EventDate), cgtEvent));

            CalculateCGT(cgtEvents);

            OnPropertyChanged("");
        }

        private void CalculateCGT(IEnumerable<CGTEvent> cgtEvents)
        {

            CurrentYearCapitalGainsOther = 0.00m;
            CurrentYearCapitalGainsDiscounted = 0.00m;
            CurrentYearCapitalLossesTotal = 0.00m;
            
            // Apportion capital gains
            foreach (var cgtEvent in cgtEvents)
            {
                if (cgtEvent.CapitalGain < 0)
                    CurrentYearCapitalLossesTotal += -cgtEvent.CapitalGain;
                else if (cgtEvent.CGTMethod == CGTMethod.Discount)
                    CurrentYearCapitalGainsDiscounted += cgtEvent.CapitalGain;
                else
                    CurrentYearCapitalGainsOther += cgtEvent.CapitalGain;
            }
        }
    }

    class CGTEventViewModel
    {
        public Stock Stock { get; private set; }
        public string CompanyName { get; private set; }
        public DateTime EventDate { get; private set; }
        public decimal CostBase { get; private set; }
        public decimal AmountReceived { get; private set; }
        public decimal CapitalGain { get; private set; }
        public string Method { get; private set; }

        public CGTEventViewModel(Stock stock, CGTEvent cgtEvent)
        {
            Stock = stock;
            CompanyName = string.Format("{0} ({1})", Stock.Name, Stock.ASXCode);
            EventDate = cgtEvent.EventDate;
            CostBase = cgtEvent.CostBase;
            AmountReceived = cgtEvent.AmountReceived;
            CapitalGain = cgtEvent.CapitalGain;
            if (cgtEvent.CGTMethod == CGTMethod.Discount)
                Method = "Discount";
            else if (cgtEvent.CGTMethod == CGTMethod.Indexation)
                Method = "Indexation";
            else
                Method = "Other";

        }
    }

}
