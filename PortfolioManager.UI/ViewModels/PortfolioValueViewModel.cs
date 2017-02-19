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
using PortfolioManager.Service;

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
            var portfolioValueService = _Parameter.PortfolioService.GetService<PortfolioValueService>();

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
                responce = await portfolioValueService.GetPortfolioValue(_Parameter.StartDate, _Parameter.EndDate, valueFrequency);
            else
                responce = await portfolioValueService.GetPortfolioValue(_Parameter.Stock.Id, _Parameter.StartDate, _Parameter.EndDate, valueFrequency);

            // create chart data
            var values = new List<double>();
            foreach (var value in responce.Values)
            {
                DateValues.Add(value.Key.ToShortDateString());
                values.Add((double)value.Value);
            }

            PortfolioValues.AddRange(values);
        }

    }
    

}
