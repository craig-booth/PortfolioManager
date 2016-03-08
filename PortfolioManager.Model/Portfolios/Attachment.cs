using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public class Attachment : Entity
    {
        public Stream Data { get; private set; }

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
