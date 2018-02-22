using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Stocks
{
    public class Stock : EffectiveEntity<StockProperties>
    {
        private SortedList<DateTime, decimal> _Prices { get; } = new SortedList<DateTime, decimal>();

        public Stock(Guid id, DateTime listingDate, string asxCode, string name, StockType type, AssetCategory category, RoundingRule dividendRoundingRule, DRPMethod drpMethod)
            : base(id, listingDate)
        {
            _CurrentValues = new StockProperties(id, listingDate, DateUtils.NoEndDate, asxCode, name, type, category, dividendRoundingRule, drpMethod);
            _Data.Add(_CurrentValues);
        }

        public void AddPrice(DateTime date, decimal price)
        {
            if (_Prices.ContainsKey(date))
                _Prices[date] = price;
            else
                _Prices.Add(date, price);
        }

        public decimal GetPrice(DateTime date)
        {
            if (_Prices.ContainsKey(date))
                return _Prices[date];
            else
                return ClosestPrice(date);
        }

        private decimal ClosestPrice(DateTime date)
        {
            int begin = 0;
            int end = _Prices.Keys.Count;
            while (end > begin)
            {
                int index = (begin + end) / 2;
                var el = _Prices.Keys[index];
                if (el.CompareTo(date) >= 0)
                    end = index;
                else
                    begin = index + 1;
            }

            return _Prices.Values[end - 1];
        }
    }

    public class StockProperties : EffectiveData
    {
        public string ASXCode { get; }
        public string Name { get; }
        public DateTime ListingDate { get; }
        public StockType Type { get; }
        public AssetCategory Category { get; }
        public RoundingRule DividendRoundingRule { get; }
        public DRPMethod DRPMethod { get; }

        public StockProperties(Guid id, DateTime fromDate, DateTime toDate, string asxCode, string name, StockType type, AssetCategory category, RoundingRule dividendRoundingRule, DRPMethod drpMethod)
            : base(id, fromDate, toDate)
        {
            ASXCode = asxCode;
            Name = name;
            Type = type;
            Category = category;
            DividendRoundingRule = dividendRoundingRule;
            DRPMethod = drpMethod;
        }

        public override EffectiveData Copy(DateRange newRange)
        {
            var newStockProperties = new StockProperties(Id, newRange.FromDate, newRange.ToDate, ASXCode, Name, Type, Category, DividendRoundingRule, DRPMethod);

            return newStockProperties;
        }
    }
}
