using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Data
{
    [Serializable]
    public class RecordNotFoundException : Exception
    {
        public Guid Id;

        public RecordNotFoundException(string message): base(message)
        {
            Id = Guid.Empty;
        }

        public RecordNotFoundException(Guid id)
            : base("A record with the id " + id + " does not exist")
        {
            Id = id;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Id", Id);
        }
    }

    [Serializable]
    public class DuplicateRecordException : Exception
    {
        public Guid Id;

        public DuplicateRecordException(Guid id)
            : base("A record with the id " + id + " already exists")
        {
            Id = id;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Id", Id);
        }
    }

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
