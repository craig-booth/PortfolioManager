using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Data.Memory.Stocks;
using PortfolioManager.Data.SQLite.Stocks;

namespace PortfolioManager.Data.Test.Stocks
{
    public class StockTest
    {
        public IStockDatabase CreateDatabase()
        {
             return new SQLiteStockDatabase("Data Source=:memory:;Version=3;");
        }

    }

    public class EntityComparer : IEqualityComparer<IEntity>
    {
        public bool Equals(IEntity entity1, IEntity entity2)
        {
            if (entity1.Id == entity2.Id)
                return true;
            else
                return false;
        }

        public int GetHashCode(IEntity entity)
        {
            return entity.Id.GetHashCode();
        }
    }

    public class EffectiveDatedEntityComparer : IEqualityComparer<IEffectiveDatedEntity>
    {
        public bool Equals(IEffectiveDatedEntity entity1, IEffectiveDatedEntity entity2)
        {
            if ((entity1.Id == entity2.Id) && (entity1.FromDate == entity2.FromDate) && (entity1.ToDate == entity2.ToDate))
                return true;
            else
                return false;
        }

        public int GetHashCode(IEffectiveDatedEntity entity)
        {
            return entity.Id.GetHashCode();
        }
    }

}
