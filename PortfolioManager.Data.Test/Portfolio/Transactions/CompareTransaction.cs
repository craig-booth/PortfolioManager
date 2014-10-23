using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.Test.Portfolio.Transactions
{
    public class TransactionComparer : IEqualityComparer<ITransaction>
    {
        public virtual bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            if ((transaction1.Id == transaction2.Id) &&
                (transaction1.Type == transaction2.Type) &&
                (transaction1.TransactionDate == transaction2.TransactionDate) &&
                (transaction1.ASXCode == transaction2.ASXCode) &&
                (transaction1.Description == transaction2.Description))
                return true;
            else
                return false;
        }

        public int GetHashCode(ITransaction transaction)
        {
            return transaction.Id.GetHashCode();
        }
    }

    public class AquisitionComparer : TransactionComparer
    {
        public bool Equals(Aquisition aquisition1, Aquisition aquisition2)
        {
            if (base.Equals(aquisition1, aquisition2) &&
                (aquisition1.AveragePrice == aquisition2.AveragePrice) &&
                (aquisition1.Units == aquisition2.Units) &&
                (aquisition1.TransactionCosts == aquisition2.TransactionCosts))
                return true;
            else
                return false;
        }
    }

    public class CostBaseAdjustmentComparer : TransactionComparer
    {
        public bool Equals(CostBaseAdjustment costbaseAdjustment1, CostBaseAdjustment aquisition2)
        {
            if (base.Equals(costbaseAdjustment1, aquisition2) &&
                (costbaseAdjustment1.Percentage == aquisition2.Percentage))
                return true;
            else
                return false;
        }
    }

    public class DisposalComparer : TransactionComparer
    {
        public bool Equals(Disposal disposal1, Disposal disposal2)
        {
            if (base.Equals(disposal1, disposal2) &&
                (disposal1.AveragePrice == disposal2.AveragePrice) &&
                (disposal1.Units == disposal2.Units) &&
                (disposal1.TransactionCosts == disposal2.TransactionCosts) &&
                (disposal1.CGTMethod == disposal2.CGTMethod))
                return true;
            else
                return false;
        }
    }

    public class IncomeReceivedComparer : TransactionComparer
    {
        public bool Equals(IncomeReceived incomeReceived1, IncomeReceived incomeReceived2)
        {
            if (base.Equals(incomeReceived1, incomeReceived2) &&
                (incomeReceived1.FrankedAmount == incomeReceived2.FrankedAmount) &&
                (incomeReceived1.UnfrankedAmount == incomeReceived2.UnfrankedAmount) &&
                (incomeReceived1.FrankingCredits == incomeReceived2.FrankingCredits) &&
                (incomeReceived1.Interest == incomeReceived2.Interest) &&
                (incomeReceived1.TaxDeferred == incomeReceived2.TaxDeferred))
                return true;
            else
                return false;
        }
    }

    public class OpeningBalanceComparer : TransactionComparer
    {
        public bool Equals(OpeningBalance openingBalance1, OpeningBalance openingBalance2)
        {
            if (base.Equals(openingBalance1, openingBalance2) &&
                (openingBalance1.CostBase == openingBalance2.CostBase) &&
                (openingBalance1.Units == openingBalance2.Units))
                return true;
            else
                return false;
        }
    }

    public class ReturnOfCapitalComparer : TransactionComparer
    {
        public bool Equals(ReturnOfCapital returnOfCapital1, ReturnOfCapital returnOfCapital2)
        {
            if (base.Equals(returnOfCapital1, returnOfCapital2) &&
                (returnOfCapital1.Amount == returnOfCapital2.Amount))
                return true;
            else
                return false;
        }
    }
}
