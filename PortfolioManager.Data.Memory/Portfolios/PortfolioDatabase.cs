using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.Memory.Portfolios
{
    public class MemoryPortfolioDatabase : IPortfolioDatabase 
    {
        internal List<ShareParcel> _Parcels { get; private set; }
        internal List<ITransaction> _Transactions { get; private set; }
        internal List<CGTEvent> _CGTEvents { get; private set; }
        internal List<IncomeReceived> _IncomeReceived { get; private set; }


        public IPortfolioUnitOfWork CreateUnitOfWork()
        {
            return new MemoryPortfolioUnitOfWork(this);
        }

        public IPortfolioQuery PortfolioQuery {get; private set;}

        public MemoryPortfolioDatabase()
        {
            PortfolioQuery = new MemoryPortfolioQuery(this);

            _Parcels = new List<ShareParcel>();
            _Transactions = new List<ITransaction>();
            _CGTEvents = new List<CGTEvent>();
            _IncomeReceived = new List<IncomeReceived>();

        }
    }
}
