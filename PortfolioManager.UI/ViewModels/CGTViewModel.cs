using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;

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
                CGTEvents.Add(new CGTEventViewModel(_Portfolio. cgtEvent));

            OnPropertyChanged("");
        }

        public void SetData(object data)
        {
            Parameter = data as ReportParmeter;
        }

    }

    class CGTEventViewModel
    {
        public Stock Stock { get; private set; }
        public string CompanyName { get; private set; }

        public CGTEventViewModel(Stock stock, CGTEvent cgtEvent)
        {
            Stock = stock;
            CompanyName = string.Format("{0} ({1})", Stock.Name, Stock.ASXCode);
          
        }
    }

}
