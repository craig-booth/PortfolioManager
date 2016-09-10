using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Data
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

        void Save();
    }

    public interface IStockRepository: IEffectiveDatedRepository<Stock>
    {
        
    }

    public interface IStockQuery
    {       
        Stock Get(Guid id, DateTime atDate);
        IReadOnlyCollection<Stock> GetAll();
        IReadOnlyCollection<Stock> GetAll(DateTime atDate);
        Stock GetByASXCode(string asxCode, DateTime atDate);
        bool TryGetByASXCode(string asxCode, DateTime atDate, out Stock stock);
        IReadOnlyCollection<Stock> GetChildStocks(Guid parent, DateTime atDate);
        decimal PercentOfParentCost(Guid parent, Guid child, DateTime atDate);
        RelativeNTA GetRelativeNTA(Guid parent, Guid child, DateTime atDate);
        IReadOnlyCollection<RelativeNTA> GetRelativeNTAs(Guid parent, Guid child);
        string GetASXCode(Guid id, DateTime atDate);

        decimal GetClosingPrice(Guid stock, DateTime date);
        decimal GetClosingPrice(Guid stock, DateTime date, bool exact);
        bool TryGetClosingPrice(Guid stock, DateTime date, out decimal price);
        bool TryGetClosingPrice(Guid stock, DateTime date, out decimal price, bool exact);
        Dictionary<DateTime, decimal> GetClosingPrices(Guid stock, DateTime fromDate, DateTime toDate);
    }

    public interface ICorporateActionRepository : IRepository<CorporateAction>
    {
        
    }

    public interface ICorporateActionQuery
    {
        CorporateAction Get(Guid id);
        IReadOnlyCollection<CorporateAction> Find(Guid stock, DateTime fromDate, DateTime toDate);
    }

    public interface IRelativeNTARepository : IRepository<RelativeNTA>
    {
        void Delete(Guid parent, Guid child, DateTime atDate);
        void DeleteAll(Guid parent);
        void DeleteAll(Guid parent, Guid child);      
    }

    public interface IStockPriceRepository
    {
        decimal Get(Guid stockId, DateTime date);
        void Add(Guid stockId, DateTime date, decimal price);
        void Update(Guid stockId, DateTime date, decimal price);
        void Delete(Guid stockId, DateTime date);
    }
}
