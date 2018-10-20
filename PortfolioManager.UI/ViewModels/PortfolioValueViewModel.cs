using System;
using System.Collections.Generic;

using LiveCharts;

using PortfolioManager.UI.Utilities;

using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.RestApi.Client;

namespace PortfolioManager.UI.ViewModels
{
    class PortfolioValueViewModel : PortfolioViewModel 
    {

        public ChartValues<double> PortfolioValues { get; private set; } 
        public List<string> DateValues { get; set; }

        public Func<ChartPoint, string> LabelFormatter { get; set; }
        public Func<double, string> YAxisFormatter { get; set; }

        public PortfolioValueViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = true;
            Options.DateSelection = DateSelectionType.Range;

            PortfolioValues = new ChartValues<double>();
            DateValues = new List<string>();

            YAxisFormatter = value => string.Format("{0:c0}", value);
            LabelFormatter = chartPoint => string.Format("{0:c0}", chartPoint.Y);
        }

        public async override void RefreshView()
        {
            DateValues.Clear();
            PortfolioValues.Clear();

            // Determine frequency to use
            var valueFrequency = ValueFrequency.Daily;
            var timeSpan = _Parameter.DateRange.ToDate - _Parameter.DateRange.FromDate;
            if (timeSpan.Days > 365 * 5)
                valueFrequency = ValueFrequency.Monthly;
            else if (timeSpan.Days > 365)
                valueFrequency = ValueFrequency.Weekly;

            PortfolioValueResponse responce;
            if (_Parameter.Stock.Id == Guid.Empty)
                responce = await _Parameter.RestClient.Portfolio.GetValue(_Parameter.DateRange, valueFrequency);
            else
                responce = await _Parameter.RestClient.Portfolio.GetValue(_Parameter.Stock.Id, _Parameter.DateRange, valueFrequency);
            if (responce == null)
                return;

            // create chart data
            var values = new List<double>();
            foreach (var value in responce.Values)
            {
                DateValues.Add(value.Date.ToShortDateString());
                values.Add((double)value.Amount);
            }

            PortfolioValues.AddRange(values);
        }

    }
    

}
