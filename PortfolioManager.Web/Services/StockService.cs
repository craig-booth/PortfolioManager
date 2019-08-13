using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;

using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{

    public interface IStockService
    {
        void ListStock(Guid id, string asxCode, string name, DateTime listingDate, bool trust, AssetCategory category);
        void ListStapledSecurity(Guid id, string asxCode, string name, DateTime listingDate, AssetCategory category, IEnumerable<StapledSecurityChild> childSecurities);
        void DelistStock(Guid id, DateTime date);

        void ChangeStock(Guid id, DateTime changeDate, string newAsxCode, string newName, AssetCategory newAssetCategory);
        void UpdateClosingPrices(Guid id, IEnumerable<Tuple<DateTime, decimal>> closingPrices);
        void ChangeDividendRules(Guid id, DateTime changeDate, decimal companyTaxRate, RoundingRule newDividendRoundingRule, bool drpActive, DRPMethod newDrpMethod);
        void ChangeRelativeNTAs(Guid id, DateTime date, decimal[] percentages);
    }

    public class StockService : IStockService
    {
        private IStockQuery _StockQuery;
        private IEntityCache<Stock> _StockCache;
        private IRepository<Stock> _StockRepository;
        private IEntityCache<StockPriceHistory> _StockPriceHistoryCache;
        private IRepository<StockPriceHistory> _StockPriceHistoryRepository;

        public StockService(IStockQuery stockQuery, IEntityCache<Stock> stockCache, IRepository<Stock> stockRepository, IEntityCache<StockPriceHistory> stockPriceHistoryCache, IRepository<StockPriceHistory> stockPriceHistoryRepository)
        {
            _StockQuery = stockQuery;
            _StockCache = stockCache;
            _StockRepository = stockRepository;
            _StockPriceHistoryCache = stockPriceHistoryCache;
            _StockPriceHistoryRepository = stockPriceHistoryRepository;
        }

        public void ListStock(Guid id, string asxCode, string name, DateTime listingDate, bool trust, AssetCategory category)
        {
            // Check that id is unique
            var stock = _StockRepository.Get(id);
            if (stock != null)
                throw new Exception("Id not unique");

            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_StockQuery.Find(effectivePeriod, y => y.ASXCode == asxCode).Any())
                throw new Exception(String.Format("Stock already exists with the code {0} at {1}", asxCode, listingDate));

            stock = new Stock(id);
            stock.List(asxCode, name, trust, category);
            _StockRepository.Add(stock);
            _StockCache.Add(stock);

            var stockPriceHistory = new StockPriceHistory(id);
            _StockPriceHistoryRepository.Add(stockPriceHistory);
            _StockPriceHistoryCache.Add(stockPriceHistory);
        }

        public void ListStapledSecurity(Guid id, string asxCode, string name, DateTime listingDate, AssetCategory category, IEnumerable<StapledSecurityChild> childSecurities)
        {
            // Check that id is unique
            var stock = _StockRepository.Get(id);
            if (stock != null)
                throw new Exception("Id not unique");

            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_StockQuery.Find(effectivePeriod, y => y.ASXCode == asxCode).Any())
                throw new Exception(String.Format("Stock already exists with the code {0} at {1}", asxCode, listingDate));

            var stapledSecurity = new StapledSecurity(id);
            stapledSecurity.List(asxCode, name, category, childSecurities);
            _StockRepository.Add(stock);
            _StockCache.Add(stock);

            var stockPriceHistory = new StockPriceHistory(id);
            _StockPriceHistoryRepository.Add(stockPriceHistory);
            _StockPriceHistoryCache.Add(stockPriceHistory);
        }

        public void ChangeStock(Guid id, DateTime changeDate, string newAsxCode, string newName, AssetCategory newAssetCategory)
        {
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new Exception(String.Format("Stock not found"));

            stock.ChangeProperties(changeDate, newAsxCode, newName, newAssetCategory);
            _StockRepository.Update(stock);
        }

        public void DelistStock(Guid id, DateTime date)
        {
            // Check that stock exists
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new Exception("Stock not found");

            stock.DeList(date);
            _StockRepository.Update(stock);
        }

        public void UpdateClosingPrices(Guid id, IEnumerable<Tuple<DateTime, decimal>> closingPrices)
        {
            var stockPriceHistory = _StockPriceHistoryCache.Get(id);
            if (stockPriceHistory == null)
                throw new Exception("Stock not found");

            stockPriceHistory.UpdateClosingPrices(closingPrices);

            _StockPriceHistoryRepository.Update(stockPriceHistory);
        }

        public void ChangeDividendRules(Guid id, DateTime changeDate, decimal companyTaxRate, RoundingRule newDividendRoundingRule, bool drpActive, DRPMethod newDrpMethod)
        {
            // Check that stock exists
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new Exception("Stock not found");

            stock.ChangeDividendRules(changeDate, companyTaxRate, newDividendRoundingRule, drpActive, newDrpMethod);
            _StockRepository.Update(stock);
        }

        public void ChangeRelativeNTAs(Guid id, DateTime date, decimal[] percentages)
        {
            // Check that stock exists
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new Exception("Stock not found");

            var stapledSecurity = stock as StapledSecurity;
            if (stapledSecurity == null)
            {
                throw new Exception("Relative NTAs only apply stapled securities");
            }

            if (percentages.Length != stapledSecurity.ChildSecurities.Count)
            {
                throw new Exception(String.Format("The number of relative ntas provided ({0}) did not match the number of child securities ({1})", percentages.Length, stapledSecurity.ChildSecurities.Count));
            }

            stapledSecurity.SetRelativeNTAs(date, percentages);
            _StockRepository.Update(stock);
        }
    }
}
