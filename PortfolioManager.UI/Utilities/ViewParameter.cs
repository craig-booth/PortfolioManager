using System;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.UI.Utilities
{
    class ViewParameter : NotifyClass
    {
        public RestWebClient RestWebClient;
        public RestClient RestClient;

        private StockItem _Stock;
        public StockItem Stock
        {
            get
            {
                return _Stock;
            }

            set
            {
                if (_Stock != value)
                {
                    _Stock = value;
                    OnPropertyChanged();
                }
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
                if (_Date != value)
                {
                    _Date = value;
                    OnPropertyChanged();
                }
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
                if (_StartDate != value)
                {
                    _StartDate = value;
                    OnPropertyChanged();
                }
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
                if (_EndDate != value)
                {
                    _EndDate = value;
                    OnPropertyChanged();
                }
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
                if (_FinancialYear != value)
                {
                    _FinancialYear = value;
                    OnPropertyChanged();
                }
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
