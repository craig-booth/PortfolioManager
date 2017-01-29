using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Utils;
using PortfolioManager.UI.Utilities;
using PortfolioManager.Model.Stocks;

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

        public override void RefreshView()
        {
            DateValues.Clear();
            PortfolioValues.Clear();

            var holdingRanges = new List<HoldingRanges>();

            // Generate list of holdings
            IReadOnlyCollection<ShareParcel> parcels;
            if (_Parameter.Stock.Id == Guid.Empty)
                parcels = _Parameter.Portfolio.ParcelService.GetParcels(_Parameter.StartDate, _Parameter.EndDate);
            else
                parcels = _Parameter.Portfolio.ParcelService.GetParcels(_Parameter.Stock, _Parameter.StartDate, _Parameter.EndDate);
            foreach (var parcel in parcels)
            {
                var holdingRange = holdingRanges.FirstOrDefault(x => x.StockId == parcel.Stock);

                if (holdingRange == null)
                {
                    holdingRange = new HoldingRanges(parcel.Stock);
                    holdingRanges.Add(holdingRange);
                }

                holdingRange.Add(parcel.Units, DateUtils.Latest(_Parameter.StartDate, parcel.FromDate), DateUtils.Earlist(_Parameter.EndDate, parcel.ToDate));
            }

            // load closing prices
            var closingPrices = new Dictionary<Guid, Dictionary<DateTime, decimal>>();
            foreach (var holdingRange in holdingRanges)
            {
                var stock = _Parameter.Portfolio.StockService.Get(holdingRange.StockId, holdingRange.StartDate);

                var priceData = _Parameter.Portfolio.StockService.GetClosingPrices(stock, holdingRange.StartDate, holdingRange.EndDate);

                closingPrices.Add(holdingRange.StockId, priceData);
            }

            // create chart data
            foreach (var date in DateUtils.WeekDays(_Parameter.StartDate, _Parameter.EndDate))
            {
                double value;

                DateValues.Add(date.ToShortDateString());

                value = 0;
                foreach (var holdingRange in holdingRanges)
                {
                    if ((date >= holdingRange.StartDate) && (date <= holdingRange.EndDate))
                    {
                        var range = holdingRange.Ranges.FirstOrDefault(x => x.IsEffectiveAt(date));
                        if (range != null)
                        {
                            var priceData = closingPrices[holdingRange.StockId];

                            value += (double)(range.Units * priceData[date]);
                        }
                    }
                }

                PortfolioValues.Add(value);
            }
        }

    }

    class HoldingRanges
    {
        public Guid StockId;
        public DateTime StartDate
        {
            get
            {
                if (Ranges.First != null)
                   return  Ranges.First.Value.FromDate;
                else
                    return DateUtils.NoDate;
            }
        }
        public DateTime EndDate
        {
            get
            {
                if (Ranges.Last != null)
                    return Ranges.Last.Value.ToDate;
                else
                    return DateUtils.NoDate;
            }
        }

        public LinkedList<HoldingRange> Ranges;
        
        public HoldingRanges(Guid stockId)
        {
            StockId = stockId;

            Ranges = new LinkedList<HoldingRange>();
        }

        public void Add(int units, DateTime startDate, DateTime endDate)
        {
            if (Ranges.Count == 0)
            {
                Ranges.AddFirst(new HoldingRange(units, startDate, endDate));
                return;
            }

            // Add overlapping range
            if ((startDate >= StartDate) && (startDate <= EndDate))
            {
                var listItem = Ranges.First;

                while (listItem != null)
                {
                    var range = listItem.Value;

                    // if range covers entire range
                    if ((startDate <= range.FromDate) && (endDate >= range.ToDate))
                    {
                        range.Units += units;
                    }
                    // if range at start only
                    else if ((startDate <= range.FromDate) && 
                             ((endDate >= range.FromDate) && (endDate < range.ToDate)))
                    {
                        var newRange = new HoldingRange(range.Units, endDate.AddDays(1), range.ToDate);
                        listItem = Ranges.AddAfter(listItem, newRange);

                        range.Units += units;
                        range.EndEntity(endDate);
                    }
                    // if range at end only
                    else if (((startDate > range.FromDate) && (startDate <= range.ToDate)) &&
                             (endDate >= range.ToDate))
                    {
                        var newRange = new HoldingRange(range.Units + units, startDate, range.ToDate);
                        listItem = Ranges.AddAfter(listItem, newRange);

                        range.EndEntity(startDate.AddDays(-1));
                    }
                    // if range in middle
                    else if ((startDate > range.FromDate) && (endDate < range.ToDate))
                    {
                        var newRange = new HoldingRange(range.Units + units, startDate, range.ToDate);
                        listItem = Ranges.AddAfter(listItem, newRange);

                        var newRange2 = new HoldingRange(range.Units, endDate.AddDays(1), range.ToDate);
                        listItem = Ranges.AddAfter(listItem, newRange2);

                        range.EndEntity(startDate.AddDays(-1));
                    }

                    listItem = listItem.Next;                 
                }
            }

            // Add range at the start
            if (startDate < StartDate)
            {
                if (endDate < StartDate)
                {
                    Ranges.AddFirst(new HoldingRange(0, endDate.AddDays(1), StartDate.AddDays(-1)));
                    Ranges.AddFirst(new HoldingRange(units, startDate, endDate));
                    
                }
                else
                    Ranges.AddFirst(new HoldingRange(units, startDate, StartDate.AddDays(-1)));
            }
       

            // Add range at the end
            if (endDate > EndDate)
            {
                if (startDate <= EndDate)
                    Ranges.AddLast(new HoldingRange(units, EndDate.AddDays(1), endDate));
                else
                {
                    Ranges.AddLast(new HoldingRange(0, EndDate.AddDays(1), startDate.AddDays(-1)));
                    Ranges.AddLast(new HoldingRange(units, startDate, endDate));
                }
            }
       
        }
    }

    class HoldingRange : EffectiveDatedEntity
    {
        public int Units;

        public HoldingRange(int units, DateTime fromDate, DateTime toDate)
            : base(Guid.Empty, fromDate, toDate)
        {
            Units = units;
        }
    }

}
