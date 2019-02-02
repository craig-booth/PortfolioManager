using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Web.Services
{
    public class StockService
    {
        private IStockQuery _StockQuery;
        private IRepository<Stock> _StockRepository;     

        public StockService(IStockQuery stockQuery, IRepository<Stock> stockRepository)
        {
            _StockQuery = stockQuery;
            _StockRepository = stockRepository;
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

            stock = new Stock();
            stock.List(asxCode, name, trust, category);
            _StockRepository.Add(stock);
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

            var stapledSecurity = new StapledSecurity();
            stapledSecurity.List(asxCode, name, category, childSecurities);
            _StockRepository.Add(stock);
        }

        public void DelistStock(Guid id, DateTime date)
        {
            // Check that stock exists
            var stock = _StockRepository.Get(id);
            if (stock == null)
                throw new Exception("Stock not found");

            stock.DeList(date);
            _StockRepository.Update(stock);
        }
    }
}
