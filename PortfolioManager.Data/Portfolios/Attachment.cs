using System;
using System.IO;

namespace PortfolioManager.Data.Portfolios
{
    public class Attachment : Entity
    {
        public string Extension { get; set; }
        public MemoryStream Data { get; private set; }

        public Attachment()
            : this(Guid.NewGuid())
        {
        }

        public Attachment(Guid id)
            :base(id)
        {
            Data = new MemoryStream();
        }

    }
}
