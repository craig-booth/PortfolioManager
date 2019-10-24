using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booth.Common;

using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.RestApi.Stocks;
using PortfolioManager.Web.Mappers;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{

    public interface IStockService
    {
        StockResponse Get(Guid id, DateTime date);
        IEnumerable<StockResponse> Get(string query, DateTime date);
        IEnumerable<StockResponse> Get(string query, DateRange dateRange);
        StockHistoryResponse GetHistory(Guid id);
        StockPriceResponse GetClosingPrices(Guid id, DateRange dateRange);

        void ListStock(Guid id, string asxCode, string name, DateTime listingDate, bool trust, AssetCategory category);
        void ListStapledSecurity(Guid id, string asxCode, string name, DateTime listingDate, AssetCategory category, IEnumerable<StapledSecurityChild> childSecurities);
        void DelistStock(Guid id, DateTime date);

        void ChangeStock(Guid id, DateTime changeDate, string newAsxCode, string newName, AssetCategory newAssetCategory);
        void UpdateCurrentPrice(Guid id, decimal price);
        void UpdateClosingPrices(Guid id, IEnumerable<StockPrice> closingPrices);
        void ChangeDividendRules(Guid id, DateTime changeDate, decimal companyTaxRate, RoundingRule newDividendRoundingRule, bool drpActive, DRPMethod newDrpMethod);
        RelativeNTAResponse GetRelativeNTA(Guid id, DateRange dateRange);
        void ChangeRelativeNTAs(Guid id, DateTime date, IEnumerable<Tuple<string, decimal>> ntas);
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

        public StockResponse Get(Guid id, DateTime date)
        {
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new StockNotFoundException(id);

            return stock.ToResponse(date);
        }

        public IEnumerable<StockResponse> Get(string query, DateTime date)
        {
            IEnumerable<Stock> stocks;

            if (query == "")
                stocks = _StockQuery.All(date);
            else
                stocks = _StockQuery.Find(date, x => MatchesQuery(x, query));

            return stocks.Select(x => x.ToResponse(date));
        }

        public IEnumerable<StockResponse> Get(string query, DateRange dateRange)
        {
            IEnumerable<Stock> stocks;

            if (query == "")
                stocks = _StockQuery.All(dateRange);
            else
                stocks = stocks = _StockQuery.Find(dateRange, x => MatchesQuery(x, query));

            return stocks.Select(x => x.ToResponse(dateRange.ToDate));
        }

        public StockHistoryResponse GetHistory(Guid id)
        {
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new StockNotFoundException(id);

            return stock.ToHistoryResponse();
        }

        public StockPriceResponse GetClosingPrices(Guid id, DateRange dateRange)
        {
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new StockNotFoundException(id);

            return stock.ToPriceResponse(dateRange);
        }

        public void ListStock(Guid id, string asxCode, string name, DateTime listingDate, bool trust, AssetCategory category)
        {
            // Check that id is unique
            var stock = _StockRepository.Get(id);
            if (stock != null)
                throw new IdNotUniqueException();

            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_StockQuery.Find(effectivePeriod, y => y.ASXCode == asxCode).Any())
                throw new StockAlreadyExistsException(asxCode, listingDate);

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
                throw new IdNotUniqueException();

            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_StockQuery.Find(effectivePeriod, y => y.ASXCode == asxCode).Any())
                throw new StockAlreadyExistsException(asxCode, listingDate);

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
                throw new StockNotFoundException(id);

            stock.ChangeProperties(changeDate, newAsxCode, newName, newAssetCategory);
            _StockRepository.Update(stock);
        }

        public void DelistStock(Guid id, DateTime date)
        {
            // Check that stock exists
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new StockNotFoundException(id);

            stock.DeList(date);
            _StockRepository.Update(stock);
        }

        public void UpdateCurrentPrice(Guid id, decimal price)
        {
            // Check that stock exists
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new StockNotFoundException(id);

            if (stock.IsEffectiveAt(DateTime.Today))
                throw new Exception("Stock not active");

            var stockPriceHistory = _StockPriceHistoryCache.Get(id);
            if (stockPriceHistory == null)
                throw new StockNotFoundException(id);

            stockPriceHistory.UpdateCurrentPrice(price);
        }

        public void UpdateClosingPrices(Guid id, IEnumerable<StockPrice> closingPrices)
        {
            // Check that stock exists
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new StockNotFoundException(id);

            // Check that the date is within the effective period
            foreach (var closingPrice in closingPrices)
            {
                if (stock.IsEffectiveAt(closingPrice.Date))
                    throw new Exception(String.Format("Stock not active on {0}", closingPrice.Date));
            }

            var stockPriceHistory = _StockPriceHistoryCache.Get(id);
            if (stockPriceHistory == null)
                throw new StockNotFoundException(id);

            stockPriceHistory.UpdateClosingPrices(closingPrices);

            _StockPriceHistoryRepository.Update(stockPriceHistory);
        }

        public void ChangeDividendRules(Guid id, DateTime changeDate, decimal companyTaxRate, RoundingRule newDividendRoundingRule, bool drpActive, DRPMethod newDrpMethod)
        {
            // Check that stock exists
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new StockNotFoundException(id);

            stock.ChangeDividendRules(changeDate, companyTaxRate, newDividendRoundingRule, drpActive, newDrpMethod);
            _StockRepository.Update(stock);
        }

        public RelativeNTAResponse GetRelativeNTA(Guid id, DateRange dateRange)
        {
            // Check that stock exists
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new StockNotFoundException(id);

            if (stock is StapledSecurity stapledSecurity)
            {
                return stapledSecurity.ToRelativeNTAResponse(dateRange);
            }
            else
                throw new Exception("Relative NTAs only apply stapled securities");
        }

        public void ChangeRelativeNTAs(Guid id, DateTime date, IEnumerable<Tuple<string, decimal>> ntas)
        {
            // Check that stock exists
            var stock = _StockQuery.Get(id);
            if (stock == null)
                throw new StockNotFoundException(id);

            if (stock is StapledSecurity stapledSecurity)
            {
                if (ntas.Count() != stapledSecurity.ChildSecurities.Count)
                    throw new Exception(String.Format("The number of relative ntas provided ({0}) did not match the number of child securities ({1})", ntas.Count(), stapledSecurity.ChildSecurities.Count));


                var percentages = new decimal[stapledSecurity.ChildSecurities.Count];
                for (var i = 0; i < stapledSecurity.ChildSecurities.Count; i++)
                {
                    var nta = ntas.FirstOrDefault(x => x.Item1 == stapledSecurity.ChildSecurities[i].ASXCode);
                    if (nta == null)
                        throw new Exception(String.Format("Relative nta not provided for {0}", stapledSecurity.ChildSecurities[i].ASXCode));

                    percentages[i] = nta.Item2;
                }

                stapledSecurity.SetRelativeNTAs(date, percentages);
                _StockRepository.Update(stock);
            }
            else
            {
                throw new Exception("Relative NTAs can only apply stapled securities");
            }


        }

        private bool MatchesQuery(StockProperties stock, string query)
        {
            return ((stock.ASXCode.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) || (stock.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0));
        }
    }
}
