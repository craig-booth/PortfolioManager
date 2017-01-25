﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

using PortfolioManager.UI.Utilities;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.UI.ViewModels
{
    class AssetAllocationViewModel : PortfolioViewModel 
    {
        public ChartValues<ObservableValue> GrowthAssets { get; private set; }
        public ChartValues<ObservableValue> IncomeAssets { get; private set; }

        public ChartValues<ObservableValue> AustralianShares { get; private set; }
        public ChartValues<ObservableValue> InternationalShares { get; private set; }
        public ChartValues<ObservableValue> AustralianProperty { get; private set; }
        public ChartValues<ObservableValue> InternationalProperty { get; private set; }
        public ChartValues<ObservableValue> AustralianFixedInterest { get; private set; }
        public ChartValues<ObservableValue> InternationalFixedInterest { get; private set; }
        public ChartValues<ObservableValue> Cash { get; private set; }

        public SeriesCollection IndividualStocks { get; private set; } 

        public Func<ChartPoint, string> LabelFormatter { get; set; }

        public AssetAllocationViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.Single;

            IndividualStocks = new SeriesCollection();

            GrowthAssets = new ChartValues<ObservableValue>() { new ObservableValue(0) };
            IncomeAssets = new ChartValues<ObservableValue>() { new ObservableValue(0) };

            AustralianShares = new ChartValues<ObservableValue>() { new ObservableValue(0) };
            InternationalShares = new ChartValues<ObservableValue>() { new ObservableValue(0) };
            AustralianProperty = new ChartValues<ObservableValue>() { new ObservableValue(0) };
            InternationalProperty = new ChartValues<ObservableValue>() { new ObservableValue(0) };
            AustralianFixedInterest = new ChartValues<ObservableValue>() { new ObservableValue(0) };
            InternationalFixedInterest = new ChartValues<ObservableValue>() { new ObservableValue(0) };
            Cash = new ChartValues<ObservableValue>() { new ObservableValue(0) };

            LabelFormatter = chartPoint => string.Format("{0:c}", chartPoint.Y);
        }

        public override void RefreshView()
        {
            Cash[0].Value = (double)_Parameter.Portfolio.CashAccountService.GetBalance(_Parameter.Date);

            var holdings = _Parameter.Portfolio.ShareHoldingService.GetHoldings(_Parameter.Date);

            IndividualStocks.Clear();

            decimal value;
            decimal growthValue = 0m;
            decimal incomeValue = 0m;

            value = AddAssetCategory(holdings, AssetCategory.AustralianStocks);
            AustralianShares[0].Value = (double)value;
            growthValue += value;

            value = AddAssetCategory(holdings, AssetCategory.InternationalStocks);
            InternationalShares[0].Value = (double)value;
            growthValue += value;

            value = AddAssetCategory(holdings, AssetCategory.AustralianProperty);
            AustralianProperty[0].Value = (double)value;
            growthValue += value;

            value = AddAssetCategory(holdings, AssetCategory.InternationalProperty);
            InternationalProperty[0].Value = (double)value;
            growthValue += value;

            value = AddAssetCategory(holdings, AssetCategory.AustralianFixedInterest);
            AustralianFixedInterest[0].Value = (double)value;
            incomeValue += value;

            value = AddAssetCategory(holdings, AssetCategory.InternationlFixedInterest);
            InternationalFixedInterest[0].Value = (double)value;
            incomeValue += value;
            
            GrowthAssets[0].Value = (double)growthValue;
            IncomeAssets[0].Value = (double)incomeValue;

            var series = new PieSeries()
            {
                Title = "Cash",
                Values = Cash,
                LabelPoint = LabelFormatter
            };
            IndividualStocks.Add(series);
        }

        private decimal AddAssetCategory(IEnumerable<Service.ShareHolding> holdings, AssetCategory category)
        {
            decimal total = 0m;
            foreach (var holding in holdings)
            {
                if (holding.Stock.Category == category)
                {
                    var series = new PieSeries()
                    {
                        Title = holding.Stock.Name,
                        Values = new ChartValues<ObservableValue>() { new ObservableValue((double)holding.MarketValue) },
                        LabelPoint = LabelFormatter
                    };
                    IndividualStocks.Add(series);

                    total += holding.MarketValue;
                }
            }

            return total;
        } 
    }

}