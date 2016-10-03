using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using PortfolioManager.Service;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.UI.Utilities
{
    class ViewParameter : NotifyClass
    {
        private Portfolio _Portfolio;
        public Portfolio Portfolio
        {
            get
            {
                return _Portfolio;
            }

            set
            {
                if (_Portfolio != value)
                {
                    if (_Portfolio != null)
                        _Portfolio.PortfolioChanged -= Portfolio_PortfolioChanged;

                    _Portfolio = value;
                    _Portfolio.PortfolioChanged += Portfolio_PortfolioChanged;
                   
                    OnPropertyChanged();
                }
            }
        }

        private void Portfolio_PortfolioChanged(PortfolioChangedEventArgs e)
        {
            OnPropertyChanged("Portfolio");
        }

        private Stock _Stock;
        public Stock Stock
        {
            get
            {
                return _Stock;
            }

            set
            {
                _Stock = value;
                OnPropertyChanged();
            }
        }

        private DateTime _Date;
        public DateTime Date
        {
            get
            {
                return _Date;
            }

            set
            {
                _Date = value;
                OnPropertyChanged();
            }
        }
        private DateTime _StartDate;
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }

            set
            {
                _StartDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _EndDate;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }

            set
            {
                _EndDate = value;
                OnPropertyChanged();
            }
        }

        private int _FinancialYear;
        public int FinancialYear
        {
            get
            {
                return _FinancialYear;
            }

            set
            {
                _FinancialYear = value;
                OnPropertyChanged();
            }
        }

        public ViewParameter()
        {
            _Date = DateTime.Now;

            _StartDate = DateTime.Now.AddYears(-1).AddDays(1);
            _EndDate = DateTime.Now;

            _FinancialYear = DateTime.Today.FinancialYear();
        }

    }
}
