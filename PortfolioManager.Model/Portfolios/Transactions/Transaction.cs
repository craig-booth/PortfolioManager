using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public enum TransactionType { Aquisition, Disposal, CostBaseAdjustment, OpeningBalance, ReturnOfCapital, Income, Deposit, Withdrawl, Interest, Fee }

    public interface ITransaction: IEntity 
    {
        TransactionType Type { get; }
        DateTime TransactionDate { get; }
        int Sequence { get;  }
        string ASXCode { get; }
        string Description { get; }
    }

}
