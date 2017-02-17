using System;
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
using PortfolioManager.Service;

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

        public async override void RefreshView()
        {
            var portfolioSummaryService = _Parameter.PortfolioService.GetService<PortfolioSummaryService>();
            var responce = await portfolioSummaryService.GetSummary(_Parameter.Date);

            Cash[0].Value = (double)responce.CashBalance;

            IndividualStocks.Clear();

            decimal value;
            decimal growthValue = 0m;
            decimal incomeValue = 0m;


            value = AddAssetCategory(responce.Holdings, AssetCategory.AustralianStocks);
            AustralianShares[0].Value = (double)value;
            growthValue += value; 

            value = AddAssetCategory(responce.Holdings, AssetCategory.InternationalStocks);
            InternationalShares[0].Value = (double)value;
            growthValue += value;

            value = AddAssetCategory(responce.Holdings, AssetCategory.AustralianProperty);
            AustralianProperty[0].Value = (double)value;
            growthValue += value; 

            value = AddAssetCategory(responce.Holdings, AssetCategory.InternationalProperty);
            InternationalProperty[0].Value = (double)value;
            growthValue += value; 

            value = AddAssetCategory(responce.Holdings, AssetCategory.AustralianFixedInterest);
            AustralianFixedInterest[0].Value = (double)value;
            incomeValue += value; 

            value = AddAssetCategory(responce.Holdings, AssetCategory.InternationlFixedInterest);
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

        private decimal AddAssetCategory(IEnumerable<Holding> holdings, AssetCategory category)
        {
            decimal total = 0m;
            foreach (var holding in holdings)
            {
                if (holding.Category == category)
                {
                    var series = new PieSeries()
                    {
                        Title = FormattedCompanyName(holding.ASXCode, holding.CompanyName),
                        Values = new ChartValues<ObservableValue>() { new ObservableValue((double)holding.Value) },
                        LabelPoint = LabelFormatter
                    };
                    IndividualStocks.Add(series);

                    total += holding.Value;
                }
            }

            return total;
        } 
    }

}
