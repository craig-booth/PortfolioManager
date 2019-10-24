using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Stocks;
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
        public void Add(DateTime date, Stock stock, int units, decimal costBase, decimal amountReceived, decimal capitalGain, CGTMethod cgtMethod)
        {
            Add(new CgtEvent()
            {
                Id = Guid.NewGuid(),
                Date = date,
                Stock = stock,
                Units = units,
                CostBase = costBase,
                AmountReceived = amountReceived,
                CapitalGain = capitalGain,
                CgtMethod = cgtMethod
            });
        }
    }
}
