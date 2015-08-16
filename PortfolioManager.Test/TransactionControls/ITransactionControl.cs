using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Test.TransactionControls
{
    public interface ITransactionControl
    {
        ITransaction CreateTransaction();
        void UpdateTransaction(ITransaction transaction);
        void DisplayTransaction(ITransaction transaction);
    }
}
