using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Data
{

    public abstract class Entity
    {
        public Guid Id { get; private set; }

        private Entity()
        {

        }

        public Entity(Guid id)
        {
            Id = id;
        }
    }

    public abstract class EffectiveDatedEntity : Entity
    {
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        public EffectiveDatedEntity(Guid id, DateTime fromDate, DateTime toDate)
            : base(id)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }

        public void EndEntity(DateTime atDate)
        {
            ToDate = atDate;
        }

        public bool IsEffectiveAt(DateTime atDate)
        {
            return (atDate >= FromDate) && (atDate <= ToDate);
        }

        public bool IsWithinRange(DateTime startDate, DateTime endDate)
        {
            return (FromDate <= startDate && ToDate >= startDate) ||
                   (FromDate > startDate && FromDate <= endDate);
        }

    }

    public interface IEditableEffectiveDatedEntity<T> where T : EffectiveDatedEntity
    {
        void EndEntity(DateTime atDate);
        T CreateNewEffectiveEntity(DateTime atDate);
    }

    public interface IRepository<T> where  T: Entity
    {
        T Get(Guid id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Guid id);
    }

    public interface IEffectiveDatedRepository<T> : IRepository<T> where T: EffectiveDatedEntity
    {
        T Get(Guid id, DateTime atDate);
        void Delete(Guid id, DateTime atDate);
    }
}
