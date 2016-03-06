using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public class Attachment : IEntity
    {
        public Guid Id { get; private set; }
        public Stream Data { get; private set; }

        public Attachment()
            :this(Guid.NewGuid())
        {
            
        }

        public Attachment(Guid id)
        {
            Id = id;

            Data = new MemoryStream();
        }

    }
}
