using System;

namespace PortfolioManager.Data
{
    public class RecordNotFoundException : Exception
    {
        public Guid Id;

        public RecordNotFoundException(string message) : base(message)
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
}
