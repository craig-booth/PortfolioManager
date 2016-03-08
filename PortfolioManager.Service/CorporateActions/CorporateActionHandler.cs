using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Service.CorporateActions
{
    interface ICorporateActionHandler
    {
        IReadOnlyCollection<Transaction> CreateTransactionList(ICorporateAction corporateAction);
        bool HasBeenApplied(ICorporateAction corporateAction, TransactionService transactionService);
    }
}
