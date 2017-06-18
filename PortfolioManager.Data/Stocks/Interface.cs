using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Data.Stocks
{

    public interface IStockDatabase
    {
        IStockUnitOfWork CreateUnitOfWork();
        IStockQuery StockQuery { get; }
        ICorporateActionQuery CorporateActionQuery { get; }
    }

    public interface IStockUnitOfWork : IDisposable
    {
        IStockRepository StockRepository { get; }
        ICorporateActionRepository CorporateActionRepository { get; }
        IRelativeNTARepository RelativeNTARepository { get; }
        IStockPriceRepository StockPriceRepository { get; }
        INonTradingDayRepository NonTradingDayRepository { get; }

        void Save();
    }

    public interface IStockRepository: IEffectiveDatedRepository<Stock>
    {
        
    }

    public interface IStockQuery
    {       
        Stock Get(Guid id, DateTime atDate);
        IEnumerable<Stock> GetAll();
        IEnumerable<Stock> GetAll(DateTime atDate);
        Stock GetByASXCode(string asxCode, DateTime atDate);
        bool TryGetByASXCode(string asxCode, DateTime atDate, out Stock stock);
        IEnumerable<Stock> GetChildStocks(Guid parent, DateTime atDate);
        decimal PercentOfParentCost(Guid parent, Guid child, DateTime atDate);
        RelativeNTA GetRelativeNTA(Guid parent, Guid child, DateTime atDate);
        IEnumerable<RelativeNTA> GetRelativeNTAs(Guid parent, Guid child);
        string GetASXCode(Guid id, DateTime atDate);

        decimal GetPrice(Guid stock, DateTime date);
        decimal GetPrice(Guid stock, DateTime date, bool exact);
        bool TryGetPrice(Guid stock, DateTime date, out decimal price);
        bool TryGetPrice(Guid stock, DateTime date, out decimal price, bool exact);
        Dictionary<DateTime, decimal> GetPrices(Guid stock, DateTime fromDate, DateTime toDate);
        DateTime GetLatestClosingPrice(Guid stock);

        bool TradingDay(DateTime date);
        IEnumerable<DateTime> NonTradingDays();
    }

    public interface ICorporateActionRepository : IRepository<CorporateAction>
    {
        
    }

    public interface ICorporateActionQuery
    {
        CorporateAction Get(Guid id);
        IEnumerable<CorporateAction> Find(Guid stock, DateTime fromDate, DateTime toDate);
    }

    public interface IRelativeNTARepository : IRepository<RelativeNTA>
    {
        void Delete(Guid parent, Guid child, DateTime atDate);
        void DeleteAll(Guid parent);
        void DeleteAll(Guid parent, Guid child);      
    }

    public interface IStockPriceRepository
    {
        bool Exists(Guid stockId, DateTime date);
        decimal Get(Guid stockId, DateTime date);
        void Add(Guid stockId, DateTime date, decimal price, bool Current);
        void Update(Guid stockId, DateTime date, decimal price, bool Current);
        void Delete(Guid stockId, DateTime date);
    }

    public interface INonTradingDayRepository
    {
        void Add(DateTime date);    
        void Delete(DateTime date);
    }

}
