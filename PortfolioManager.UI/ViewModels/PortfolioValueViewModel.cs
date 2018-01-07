using System;
using System.Collections.Generic;

using LiveCharts;

using PortfolioManager.UI.Utilities;

using PortfolioManager.Service.Interface;

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
            var timeSpan = _Parameter.EndDate - _Parameter.StartDate;
            if (timeSpan.Days > 365 * 5)
                valueFrequency = ValueFrequency.Monthly;
            else if (timeSpan.Days > 365)
                valueFrequency = ValueFrequency.Weekly;

            PortfolioValueResponce responce;
            if (_Parameter.Stock.Id == Guid.Empty)
                responce = await _Parameter.RestWebClient.GetPortfolioValueAsync(_Parameter.StartDate, _Parameter.EndDate, valueFrequency);
            else
                responce = await _Parameter.RestWebClient.GetPortfolioValueAsync(_Parameter.Stock.Id, _Parameter.StartDate, _Parameter.EndDate, valueFrequency);
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
