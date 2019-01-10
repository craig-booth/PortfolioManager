using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Utils;

namespace PortfolioManager.Domain.Portfolios
{

    public interface ICgtEventCollection
    : ITransactionList<CgtEvent>
    {

    }

    public class CgtEventCollection
        : TransactionList<CgtEvent>,
        ICgtEventCollection,
        ITransactionList<CgtEvent>
    {
    }
}
