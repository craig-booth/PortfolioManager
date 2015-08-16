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
        IReadOnlyCollection<Stock> GetChildStocks(Guid parent, DateTime atDate);
        decimal PercentOfParentCost(Guid parent, Guid child, DateTime atDate);
        RelativeNTA GetRelativeNTA(Guid parent, Guid child, DateTime atDate);
        IReadOnlyCollection<RelativeNTA> GetRelativeNTAs(Guid parent, Guid child);
        string GetASXCode(Guid id, DateTime atDate);
    }


    public interface ICorporateActionRepository : IRepository<ICorporateAction>
    {
        
    }

    public interface ICorporateActionQuery
    {
        ICorporateAction Get(Guid id);
        IReadOnlyCollection<ICorporateAction> Find(Guid stock, DateTime fromDate, DateTime toDate);
    }

    public interface IRelativeNTARepository : IRepository<RelativeNTA>
    {
        void Delete(Guid parent, Guid child, DateTime atDate);
        void DeleteAll(Guid parent);
        void DeleteAll(Guid parent, Guid child);      
    }

}
