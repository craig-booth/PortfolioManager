using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.UI.Utilities
{

    interface IViewParameter
    {
        string Description { get; }
    }

    interface IDateRangeParameter : IViewParameter
    {
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }

    interface IFinancialYearParameter : IDateRangeParameter
    {
        int FinancialYear { get; }
    }

    class FinancialYearParameter : IFinancialYearParameter
    {
        public int FinancialYear { get; set; }

        public DateTime StartDate
        {
            get
            {
                return new DateTime(FinancialYear - 1, 7, 1);
            }
        }

        public DateTime EndDate
        {
            get
            {
                return new DateTime(FinancialYear, 6, 30);
            }
        }

        public string Description
        {
            get
            {
                return string.Format("{0}/{1} Financial Year", FinancialYear - 1, FinancialYear);
            }
        }

        public FinancialYearParameter()
        {
            if (DateTime.Today.Month <= 6)
                FinancialYear = DateTime.Today.Year;
            else
                FinancialYear = DateTime.Today.Year + 1;
        }

        public FinancialYearParameter(int year)
        {
            FinancialYear = year;
        }
    }

    class  DateRangeParameter: IDateRangeParameter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public string Description
        {
            get
            {
                return string.Format("{0:d} - {1:d}", StartDate, EndDate);
            }
        }

        public DateRangeParameter()
        {
            StartDate = DateTime.Now.AddYears(-1);
            EndDate = DateTime.Now;
        }

        public DateRangeParameter(DateTime start, DateTime end)
        {
            StartDate = start;
            EndDate = end;
        }
    }

    class Module : NotifyClass
    {
        public string Label { get; private set; }
        public Geometry Image { get; private set; }

        public Visibility ViewParameterAreaVisible { get; set; }
        public IViewParameter ViewParameter { get; set; }

        public Visibility ViewSelectionAreaVisible { get; set; }
        public List<IViewModel> Views { get; private set; }

        private IViewModel _SelectedView;
        public IViewModel SelectedView
        {
            get
            {
                return _SelectedView;
            }
            set
            {
                _SelectedView = value;
                if (_SelectedView != null)
                    _SelectedView.SetData(null);
                OnPropertyChanged();
            }
        }

        public Module(string label, string image)
        {
            Label = label;
            Image = App.Current.FindResource(image) as Geometry;

            Views = new List<IViewModel>();

            ViewParameterAreaVisible = Visibility.Collapsed;
            ViewSelectionAreaVisible = Visibility.Collapsed;
        }
    }
    
}
