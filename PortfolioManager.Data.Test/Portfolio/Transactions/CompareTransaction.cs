using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.Test.Portfolio.Transactions
{
    public class TransactionComparer : IEqualityComparer<Transaction> 
    {
        public virtual bool Equals(Transaction transaction1, Transaction transaction2)
        {
            if ((transaction1.Id == transaction2.Id) &&
                (transaction1.Type == transaction2.Type) &&
                (transaction1.TransactionDate == transaction2.TransactionDate) &&
                (transaction1.ASXCode == transaction2.ASXCode) &&
                (transaction1.Description == transaction2.Description) &&
                (transaction1.RecordDate == transaction2.RecordDate) &&
                (transaction1.Comment == transaction2.Comment))
                return true;
            else
                return false;
        }

        public int GetHashCode(Transaction transaction)
        {
            return transaction.Id.GetHashCode();
        }
    }

    public class AquisitionComparer : TransactionComparer
    {
        public override bool Equals(Transaction transaction1, Transaction transaction2)
        {
            var aquisition1 = transaction1 as Aquisition;
            var aquisition2 = transaction2 as Aquisition;

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
        public override bool Equals(Transaction transaction1, Transaction transaction2)
        {
            var costbaseAdjustment1 = transaction1 as CostBaseAdjustment;
            var costbaseAdjustment2 = transaction2 as CostBaseAdjustment;

            if (base.Equals(costbaseAdjustment1, costbaseAdjustment2) &&
                (costbaseAdjustment1.Percentage == costbaseAdjustment2.Percentage))
                return true;
            else
                return false;
        }
    }

    public class DisposalComparer : TransactionComparer
    {
        public override bool Equals(Transaction transaction1, Transaction transaction2)
        {
            var disposal1 = transaction1 as Disposal;
            var disposal2 = transaction2 as Disposal;

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
        public override bool Equals(Transaction transaction1, Transaction transaction2)
        {
            var incomeReceived1 = transaction1 as IncomeReceived;
            var incomeReceived2 = transaction2 as IncomeReceived;

            if (base.Equals(transaction1, transaction2) &&
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
        public override bool Equals(Transaction transaction1, Transaction transaction2)
        {
            var openingBalance1 = transaction1 as OpeningBalance;
            var openingBalance2 = transaction2 as OpeningBalance;

            if (base.Equals(openingBalance1, openingBalance2) &&
                (openingBalance1.CostBase == openingBalance2.CostBase) &&
                (openingBalance1.Units == openingBalance2.Units) &&
                (openingBalance1.AquisitionDate == openingBalance2.AquisitionDate))
                return true;
            else
                return false;
        }
    }

    public class ReturnOfCapitalComparer : TransactionComparer
    {
        public override bool Equals(Transaction transaction1, Transaction transaction2)
        {
            var returnOfCapital1 = transaction1 as ReturnOfCapital;
            var returnOfCapital2 = transaction2 as ReturnOfCapital;

            if (base.Equals(returnOfCapital1, returnOfCapital2) &&
                (returnOfCapital1.Amount == returnOfCapital2.Amount))
                return true;
            else
                return false;
        }
    }
}
