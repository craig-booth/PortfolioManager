using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Service.Utils;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class CGTViewModel : PortfolioViewModel, IViewModelWithData
    {
        private ReportParmeter _Parameter;
        public ReportParmeter Parameter
        {
            get
            {
                return _Parameter;
            }

            set
            {
                if (value != _Parameter)
                {
                    _Parameter = value;
                    OnPropertyChanged();

                    ShowReport();
                }
            }
        }

        public ObservableCollection<CGTEventViewModel> CGTEvents { get; private set; }

        public decimal NonDiscountedGains { get; private set; }
        public decimal NonDiscountedOffsetLosses { get; private set; }
        public decimal TotalNonDiscountedGains { get; private set; }
        public decimal DiscountedGains { get; private set; }
        public decimal DiscountedOffsetLosses { get; private set; }
        public decimal PriorYearLosses { get; private set; }
        public decimal Discount { get; private set; }
        public decimal TotalDiscountedGains { get; private set; }
        public decimal TotalCapitalGain { get; private set; }

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

        public CGTViewModel()
        {
            CGTEvents = new ObservableCollection<CGTEventViewModel>();
        }

        public void ShowReport()
        {
            Heading = string.Format("CGT Repoort for {0}/{1} financial year", _Parameter.FromDate.Year, _Parameter.ToDate.Year);

            // Get a list of all the cgt events for the year
            var cgtEvents = _Portfolio.CGTService.GetEvents(_Parameter.FromDate, _Parameter.ToDate);

            CGTEvents.Clear();
            foreach (var cgtEvent in cgtEvents)
                CGTEvents.Add(new CGTEventViewModel(_Portfolio.StockService.Get(cgtEvent.Stock, cgtEvent.EventDate), cgtEvent));

            CalculateCGT(cgtEvents);

            OnPropertyChanged("");
        }

        public void SetData(object data)
        {
            Parameter = data as ReportParmeter;
        }

        private void CalculateCGT(IEnumerable<CGTEvent> cgtEvents)
        {
            var capitalLosses = cgtEvents.Where(x => x.CapitalGain < 0).Sum(x => x.CapitalGain);

            NonDiscountedGains = cgtEvents.Where(x => x.CGTMethod != CGTMethod.Discount).Sum(x => x.CapitalGain);
            if (capitalLosses > NonDiscountedGains)
            {
                NonDiscountedOffsetLosses = NonDiscountedGains;
                capitalLosses = 0.00m;
            }
            else
            {
                NonDiscountedOffsetLosses = capitalLosses;
                capitalLosses -= NonDiscountedOffsetLosses;
            }
            TotalNonDiscountedGains = NonDiscountedGains - NonDiscountedOffsetLosses;

            DiscountedGains = cgtEvents.Where(x => x.CGTMethod == CGTMethod.Discount).Sum(x => x.CapitalGain);
            DiscountedOffsetLosses = capitalLosses;
            var capitalGain = DiscountedGains - capitalLosses - PriorYearLosses;
            if (capitalGain > 0)
                Discount = CGTCalculator.CGTDiscount(capitalGain);
            else
                Discount = 0.00m;

            TotalDiscountedGains = capitalGain - Discount;

            TotalCapitalGain = TotalNonDiscountedGains + TotalDiscountedGains;
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
