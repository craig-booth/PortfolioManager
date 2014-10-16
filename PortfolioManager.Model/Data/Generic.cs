using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Data
{
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
    }

    public class DuplicateRecordException : Exception
    {
        public Guid Id;

        public DuplicateRecordException(Guid id)
            : base("A record with the id " + id + " already exists")
        {
            Id = id;
        }
    }

    public interface IEntity
    {
        Guid Id { get; }
    }

    public interface IRepository<T> where  T: IEntity
    {
        T Get(Guid id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Guid id);
    }

    public interface IEffectiveDatedEntity: IEntity 
    {
        DateTime FromDate { get; }
        DateTime ToDate { get; }
    }

    public interface IEffectiveDatedRepository<T> : IRepository<T> where T: IEffectiveDatedEntity
    {
        T Get(Guid id, DateTime atDate);
        void Delete(Guid id, DateTime atDate);
    }
}
