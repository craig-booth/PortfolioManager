using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{
    public interface IStockRepository : IRepository<Stock>
    {
        Stock Get(string asxCode, DateTime date);
        IEnumerable<Stock> All();
        IEnumerable<Stock> All(DateTime date);
        IEnumerable<Stock> All(DateRange dateRange);
        IEnumerable<Stock> Find(DateTime date, Func<StockProperties, bool> predicate);
        IEnumerable<Stock> Find(DateRange dateRange, Func<StockProperties, bool> predicate);

        void ListStock(Guid id, string asxCode, string name, DateTime listingDate, bool trust, AssetCategory category);
        void ListStapledSecurity(Guid id, string asxCode, string name, DateTime listingDate, AssetCategory category, IEnumerable<StapledSecurityChild> childSecurities);
        void DelistStock(Guid id, DateTime date);
    }

    public class StockRepository :
        Repository<Stock>,
        IRepository<Stock>,
        IStockRepository,
        ILoadableRepository<Stock>
    {
        private IEventStream<StapledSecurity> _StapledSecurityEventStream;

        public StockRepository(IEventStream<Stock> stockEventStream, IEventStream<StapledSecurity> stapledSecurityEventStream, IEntityCache<Stock> cache)
            : base(stockEventStream, cache)
        {
            _StapledSecurityEventStream = stapledSecurityEventStream;
        }

        public void LoadFromEventStream()
        {
            var entityIds = _EventStream.GetStoredEntityIds();
            foreach (var id in entityIds)
                Get(id);

            entityIds = _StapledSecurityEventStream.GetStoredEntityIds();
            foreach (var id in entityIds)
                Get(id);
        } 

        public Stock Get(string asxCode, DateTime date)
        {
            return _Cache.All().FirstOrDefault(x => x.IsEffectiveAt(date) && x.Properties.Matches(date, y => y.ASXCode == asxCode));
        } 

        public IEnumerable<Stock> All()
        {
            return _Cache.All();
        } 

        public IEnumerable<Stock> All(DateTime date)
        {
            return _Cache.All().Where(x => x.IsEffectiveAt(date));
        }
         
        public IEnumerable<Stock> All(DateRange dateRange)
        {
            return _Cache.All().Where(x => x.IsEffectiveDuring(dateRange));
        } 

        public IEnumerable<Stock> Find(DateTime date, Func<StockProperties, bool> predicate)
        {
            return All(date).Where(x => x.Properties.Matches(date, predicate));
        } 

        public IEnumerable<Stock> Find(DateRange dateRange, Func<StockProperties, bool> predicate)
        {
            return All(dateRange).Where(x => x.Properties.Matches(dateRange, predicate));
        } 

        public void ListStock(Guid id, string asxCode, string name, DateTime listingDate, bool trust, AssetCategory category)
        {
            // Check that id is unique
            var stock = _Cache.Get(id);
            if (stock != null)
                throw new Exception("Id not unique");

            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_Cache.All().Any(x => x.Properties.Matches(effectivePeriod, y => y.ASXCode == asxCode)))
                throw new Exception(String.Format("Stock already exists with the code {0} at {1}", asxCode, listingDate));

            stock = new Stock();
            stock.List(asxCode, name, trust, category);

            _Cache.Add(stock);
        } 

        public void ListStapledSecurity(Guid id, string asxCode, string name, DateTime listingDate, AssetCategory category, IEnumerable<StapledSecurityChild> childSecurities)
        {
            // Check that id is unique
            var stock = _Cache.Get(id);
            if (stock != null)
                throw new Exception("Id not unique");

            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_Cache.All().Any(x => x.Properties.Matches(effectivePeriod, y => y.ASXCode == asxCode)))
                throw new Exception(String.Format("Stock already exists with the code {0} at {1}", asxCode, listingDate));

            var stapledSecurity = new StapledSecurity();
            stapledSecurity.List(asxCode, name, category, childSecurities);

            _Cache.Add(stapledSecurity);
        } 

        public void DelistStock(Guid id, DateTime date)
        {
            // Check that stock exists
            var stock = Get(id);
            if (stock == null)
                throw new Exception("Stock not found");

            stock.DeList(date);
        }
    } 
}
