using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using PortfolioManager.UI.Utilities;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.UI.ViewModels
{
    class PerformanceViewModel : PortfolioViewModel
    {

        public decimal OpeningBalance { get; private set; }
        public decimal Deposits { get; private set; }
        public decimal Withdrawls { get; private set; }
        public decimal Interest { get; private set; }
        public decimal Dividends { get; private set; }
        public decimal Fees { get; private set; }
        public decimal ChangeInMarketValue { get; private set; }
        public decimal OutstandingDRPAmount { get; private set; }

        public decimal TotalIncome
        {
            get
            {
                return Interest + Dividends + Fees;
            }
        }

        public decimal TotalReturn
        {
            get
            {
                return TotalIncome + ChangeInMarketValue;
            }
        }

        public decimal ChangeInValue
        {
            get
            {
                return Deposits + Withdrawls + TotalIncome + ChangeInMarketValue + OutstandingDRPAmount;
            }
        }

        public decimal ClosingBalance
        {
            get
            {
                return OpeningBalance + ChangeInValue;
            }
        }

        public ObservableCollection<StockPerformanceItem> StockPerformance { get; private set; }

        public PerformanceViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.Range;

            StockPerformance = new ObservableCollection<StockPerformanceItem>();
        }


        public async override void RefreshView()
        {
            var portfolioPerformanceService = _Parameter.PortfolioManagerService.GetService<IPortfolioPerformanceService>();
            var responce = await portfolioPerformanceService.GetPerformance(_Parameter.StartDate, _Parameter.EndDate);

            OpeningBalance = responce.OpeningBalance + responce.OpeningCashBalance;
            Deposits = responce.Deposits;
            Withdrawls = responce.Withdrawls;
            Interest = responce.Interest;
            Dividends = responce.Dividends;
            Fees = responce.Fees;
            ChangeInMarketValue = responce.ChangeInMarketValue;
            OutstandingDRPAmount = responce.OutstandingDRPAmount;

            StockPerformance.Clear();
            foreach (var stockPerformance in responce.HoldingPerformance.OrderBy(x => x.Stock.Name))
                StockPerformance.Add(new StockPerformanceItem(stockPerformance));

            StockPerformance.Add(new StockPerformanceItem("Cash")
            {
                OpeningBalance = responce.OpeningCashBalance,
                Dividends = Interest,
                ClosingBalance = responce.ClosingCashBalance
            });

            OnPropertyChanged(""); 
        }
    }



    public class StockPerformanceItem
    {
        public string CompanyName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Purchases { get; set; }
        public decimal Sales { get; set; }      
        public decimal ClosingBalance { get; set; }
        public decimal Dividends { get; set; }
        public decimal CapitalGain { get; set; }
        public decimal DRPCashBalance { get; set; }
        public decimal TotalReturn { get; set; }
        public double IRR { get; set; }

        public StockPerformanceItem(string companyName)
        {
            CompanyName = companyName;
        }

        public StockPerformanceItem(HoldingPerformance stockPerformance)
        {
            CompanyName = stockPerformance.Stock.FormattedCompanyName();
            OpeningBalance = stockPerformance.OpeningBalance;
            Purchases = stockPerformance.Purchases;
            Sales = stockPerformance.Sales;
            Dividends = stockPerformance.Dividends;
            CapitalGain = stockPerformance.CapitalGain;
            ClosingBalance = stockPerformance.ClosingBalance;
            DRPCashBalance = stockPerformance.DRPCashBalance;
            TotalReturn = stockPerformance.TotalReturn;
            IRR = stockPerformance.IRR;
        }



    }
}
