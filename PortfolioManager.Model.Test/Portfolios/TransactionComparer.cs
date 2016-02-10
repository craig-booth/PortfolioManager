using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios
{
    public class TransactionComparer : IEntityComparer<ITransaction>
    {
        private Dictionary<TransactionType, IEntityComparer<ITransaction>> _TransactionComparers;

        public TransactionComparer()
        {
            _TransactionComparers = new Dictionary<TransactionType, IEntityComparer<ITransaction>>();
            _TransactionComparers.Add(TransactionType.Aquisition, new AquisitionComparer());
            _TransactionComparers.Add(TransactionType.CostBaseAdjustment, new CostBaseAdjustmentComparer());
            _TransactionComparers.Add(TransactionType.Disposal, new DisposalComparer());
            _TransactionComparers.Add(TransactionType.Income, new IncomeReceivedComparer());
            _TransactionComparers.Add(TransactionType.OpeningBalance, new OpeningBalanceComparer());
            _TransactionComparers.Add(TransactionType.ReturnOfCapital, new ReturnOfCapitalComparer());
            _TransactionComparers.Add(TransactionType.UnitCountAdjustment, new UnitCountAdjustmentComparer());
        }

        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            if (transaction1.Type != transaction2.Type)
                return false;

            return _TransactionComparers[transaction1.Type].Equals(transaction1, transaction2);
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            _TransactionComparers[transaction.Type].Write(writer, transaction);
        }
    }

    public class ReturnOfCapitalComparer: IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            var returnOfCapital1 = transaction1 as ReturnOfCapital;
            var returnOfCapital2 = transaction2 as ReturnOfCapital;

            return (returnOfCapital1.Amount == returnOfCapital2.Amount)
                && (returnOfCapital1.ASXCode == returnOfCapital2.ASXCode)
                && (returnOfCapital1.Comment == returnOfCapital2.Comment)
                && (returnOfCapital1.RecordDate == returnOfCapital2.RecordDate);
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var returnOfCapital = transaction as ReturnOfCapital;

            writer.Write("<ReturnOfCapital:- Transactionate: {0:d}>",
                new object[] { returnOfCapital.TransactionDate });
        }
    }

    public class OpeningBalanceComparer : IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            return false;
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var openingBalance = transaction as OpeningBalance;

            writer.Write("<OpeningBalance:- Transactionate: {0:d}>",
                new object[] { openingBalance.TransactionDate });
        }
    }

    public class AquisitionComparer : IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            return false;
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var aquistion = transaction as Aquisition;

            writer.Write("<Aquisition:- Transactionate: {0:d}>",
                new object[] { aquistion.TransactionDate });
        }
    }

    public class CostBaseAdjustmentComparer : IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            return false;
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var costBaseAdjustment = transaction as Aquisition;

            writer.Write("<CostBaseAdjustment:- Transactionate: {0:d}>",
                new object[] { costBaseAdjustment.TransactionDate });
        }
    }

    public class DisposalComparer : IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            return false;
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var disposal = transaction as Disposal;

            writer.Write("<Disposal:- Transactionate: {0:d}>",
                new object[] { disposal.TransactionDate });
        }
    }

    public class IncomeReceivedComparer : IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            return false;
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var incomeReceived = transaction as Aquisition;

            writer.Write("<IncomeReceived:- Transactionate: {0:d}>",
                new object[] { incomeReceived.TransactionDate });
        }
    }

    public class UnitCountAdjustmentComparer : IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            return false;
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var unitCountAdjustment = transaction as Aquisition;

            writer.Write("<UnitCountAdjustment:- Transactionate: {0:d}>",
                new object[] { unitCountAdjustment.TransactionDate });
        }
    }

}
