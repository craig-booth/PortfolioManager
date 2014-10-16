using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.Test
{
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
