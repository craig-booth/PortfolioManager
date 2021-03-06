﻿using System;
using System.Linq;
using System.Collections.ObjectModel;

using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.UI.Utilities;

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
            var responce = await _Parameter.RestClient.Portfolio.GetPerformance(_Parameter.DateRange);
            if (responce == null)
                return;

            OpeningBalance = responce.OpeningBalance + responce.OpeningCashBalance;
            Deposits = responce.Deposits;
            Withdrawls = responce.Withdrawls;
            Interest = responce.Interest;
            Dividends = responce.Dividends;
            Fees = responce.Fees;
            ChangeInMarketValue = responce.ChangeInMarketValue;
            OutstandingDRPAmount = responce.OutstandingDRPAmount;

            StockPerformance.Clear();
            foreach (var holdingPerformance in responce.HoldingPerformance.OrderBy(x => x.Stock.Name))
                StockPerformance.Add(new StockPerformanceItem(holdingPerformance));

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
        public StockViewItem Stock { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Purchases { get; set; }
        public decimal Sales { get; set; }      
        public decimal ClosingBalance { get; set; }
        public decimal Dividends { get; set; }
        public decimal CapitalGain { get; set; }
        public decimal DRPCashBalance { get; set; }
        public decimal TotalReturn { get; set; }
        public decimal IRR { get; set; }

        public StockPerformanceItem(string name)
        {
            Stock = new StockViewItem(Guid.Empty, "", name);
        }

        public StockPerformanceItem(PortfolioPerformanceResponse.HoldingPerformanceItem holdingPerformance)
        {
            Stock = new StockViewItem(holdingPerformance.Stock);
            OpeningBalance = holdingPerformance.OpeningBalance;
            Purchases = holdingPerformance.Purchases;
            Sales = holdingPerformance.Sales;
            Dividends = holdingPerformance.Dividends;
            CapitalGain = holdingPerformance.CapitalGain;
            ClosingBalance = holdingPerformance.ClosingBalance;
            DRPCashBalance = holdingPerformance.DRPCashBalance;
            TotalReturn = holdingPerformance.TotalReturn;
            IRR = holdingPerformance.IRR;
        }



    }
}
