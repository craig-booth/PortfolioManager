using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Utils;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Services
{

    public class PortfolioValueService : IPortfolioValueService
    {

        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly StockExchange _StockExchange;

        public PortfolioValueService(IPortfolioDatabase portfolioDatabase, StockExchange stockExchange)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockExchange = stockExchange;
        }

        public Task<PortfolioValueResponce> GetPortfolioValue(DateTime fromDate, DateTime toDate, ValueFrequency frequency)
        {
            PortfolioValueResponce responce;

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var parcels = portfolioUnitOfWork.PortfolioQuery.GetAllParcels(fromDate, toDate);
                responce = GetPortfolioValue(parcels, fromDate, toDate, frequency);

                foreach (var value in responce.Values)
                    value.Amount += portfolioUnitOfWork.PortfolioQuery.GetCashBalance(value.Date);

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<PortfolioValueResponce>(responce); 
        }

        public Task<PortfolioValueResponce> GetPortfolioValue(Guid stockId, DateTime fromDate, DateTime toDate, ValueFrequency frequency)
        { 
            PortfolioValueResponce responce;

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var parcels = portfolioUnitOfWork.PortfolioQuery.GetParcelsForStock(stockId, fromDate, toDate);

                responce = GetPortfolioValue(parcels, fromDate, toDate, frequency);

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<PortfolioValueResponce>(responce); 
        }

        private PortfolioValueResponce GetPortfolioValue(IEnumerable<ShareParcel> parcels, DateTime fromDate, DateTime toDate, ValueFrequency frequency)
        {
            var responce = new PortfolioValueResponce();

            var holdingRanges = new List<HoldingRanges>();

            foreach (var parcel in parcels)
            {
                var holdingRange = holdingRanges.FirstOrDefault(x => x.StockId == parcel.Stock);

                if (holdingRange == null)
                {
                    holdingRange = new HoldingRanges(parcel.Stock);
                    holdingRanges.Add(holdingRange);
                }

                holdingRange.Add(parcel.Units, DateUtils.Latest(fromDate, parcel.FromDate), DateUtils.Earlist(toDate, parcel.ToDate));
            }

            // load closing prices
            var closingPrices = new Dictionary<Guid, Dictionary<DateTime, decimal>>();
            foreach (var holdingRange in holdingRanges)
            {
                var stock = _StockExchange.Stocks.Get(holdingRange.StockId);

                var priceData = new Dictionary<DateTime, decimal>();
                foreach (var date in _StockExchange.TradingCalander.TradingDays(new DateRange(fromDate, toDate)))
                {
                    priceData.Add(date, stock.GetPrice(date));
                }

                closingPrices.Add(holdingRange.StockId, priceData);
            }

            foreach (var date in _StockExchange.TradingCalander.TradingDays(new DateRange(fromDate, toDate)))
            {
                if ((frequency == ValueFrequency.Weekly) && (date.DayOfWeek != DayOfWeek.Friday))
                    continue;

                if ((frequency == ValueFrequency.Monthly) && (date.Day != DateTime.DaysInMonth(date.Year, date.Month)))
                    continue;

                decimal value = 0;
                foreach (var holdingRange in holdingRanges)
                {
                    if ((date >= holdingRange.StartDate) && (date <= holdingRange.EndDate))
                    {
                        var range = holdingRange.Ranges.FirstOrDefault(x => x.IsEffectiveAt(date));
                        if (range != null)
                        {
                            var priceData = closingPrices[holdingRange.StockId];

                            value += range.Units * priceData[date];
                        }
                    }
                }

                responce.Values.Add(new DailyAmount(date, value));
            }

            return responce;
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
                    return Ranges.First.Value.FromDate;
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
