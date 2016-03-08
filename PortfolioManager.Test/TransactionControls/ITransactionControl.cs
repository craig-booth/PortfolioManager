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
        Transaction CreateTransaction();
        void UpdateTransaction(Transaction transaction);
        void DisplayTransaction(Transaction transaction);
    }
}
