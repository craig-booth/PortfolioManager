using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Data
{

    public interface ILiveStockPriceDatabase
    {
        ILiveStockPriceUnitOfWork CreateUnitOfWork();
        ILiveStockPriceQuery LivePriceQuery { get; }
    }

    public interface ILiveStockPriceUnitOfWork : IDisposable
    {
        ILiveStockPriceRepository LivePriceRepository { get; }

        void Save();
    }

    public interface ILiveStockPriceRepository 
    {
        decimal Get(Guid stockId);
        void Update(Guid stockId, decimal price);
        void Delete(Guid stockId);
    }

    public interface ILiveStockPriceQuery
    {
        decimal GetPrice(Guid stock);
        IDictionary<Guid, decimal> GetPrice(IEnumerable<Guid> stocks);
    }
}
