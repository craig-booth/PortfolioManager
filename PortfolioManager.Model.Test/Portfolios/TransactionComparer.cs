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
            return (transaction1.Type == transaction2.Type) 
                    && (transaction1.TransactionDate == transaction2.TransactionDate)
                    && (transaction1.ASXCode == transaction2.ASXCode)
                    && (transaction1.Description == transaction2.Description)
                    && _TransactionComparers[transaction1.Type].Equals(transaction1, transaction2);
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
                && (returnOfCapital1.Comment == returnOfCapital2.Comment)
                && (returnOfCapital1.RecordDate == returnOfCapital2.RecordDate);
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var returnOfCapital = transaction as ReturnOfCapital;

            writer.Write("<ReturnOfCapital:- TransactionDate: {0:d}, ASXCode: {1}, Description: {2}, Amount: {3}, RecordDate: {4:d}, Comment: {5}>",
                new object[] { returnOfCapital.TransactionDate, returnOfCapital.ASXCode, returnOfCapital.Description, returnOfCapital.Amount, returnOfCapital.RecordDate, returnOfCapital.Comment });
        }
    }

    public class OpeningBalanceComparer : IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            var openingBalance1 = transaction1 as OpeningBalance;
            var openingBalance2 = transaction2 as OpeningBalance;

            return (openingBalance1.AquisitionDate == openingBalance2.AquisitionDate)
                && (openingBalance1.Comment == openingBalance2.Comment)
                && (openingBalance1.CostBase == openingBalance2.CostBase)
                && (openingBalance1.Units == openingBalance2.Units);
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var openingBalance = transaction as OpeningBalance;

            writer.Write("<OpeningBalance:- Transactionate: {0:d}, ASXCode: {1}, Description: {2}, AquisitionDate: {3:d}, CostBase: {4}, Units: {5}, Comment: {6}>",
                new object[] { openingBalance.TransactionDate, openingBalance.ASXCode, openingBalance.Description, openingBalance.AquisitionDate, openingBalance.CostBase, openingBalance.Units, openingBalance.Comment });
        }
    }

    public class AquisitionComparer : IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            var aquistion1 = transaction1 as Aquisition;
            var aquistion2 = transaction2 as Aquisition;

            return (aquistion1.AveragePrice == aquistion2.AveragePrice)
                && (aquistion1.Comment == aquistion2.Comment)
                && (aquistion1.TransactionCosts == aquistion2.TransactionCosts)
                && (aquistion1.Units == aquistion2.Units);
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var aquistion = transaction as Aquisition;

            writer.Write("<Aquisition:- Transactionate: {0:d}, ASXCode: {1}, Description: {2}, AveragePrice: {3}, TransactionCosts: {4}, Units: {5}, Comment: {6}>",
                new object[] { aquistion.TransactionDate, aquistion.ASXCode, aquistion.Description, aquistion.AveragePrice, aquistion.TransactionCosts, aquistion.Units, aquistion.Comment});
        }
    }

    public class CostBaseAdjustmentComparer : IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            var costBaseAdjustment1 = transaction1 as CostBaseAdjustment;
            var costBaseAdjustment2 = transaction2 as CostBaseAdjustment;

            return (costBaseAdjustment1.Comment == costBaseAdjustment2.Comment)
                && (costBaseAdjustment1.Percentage == costBaseAdjustment1.Percentage)
                && (costBaseAdjustment1.RecordDate == costBaseAdjustment2.RecordDate);
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var costBaseAdjustment = transaction as CostBaseAdjustment;

            writer.Write("<CostBaseAdjustment:- Transactionate: {0:d}, ASXCode: {1}, Description: {2}, Percentage: {3}, RecordDate: {4:d}, Comment: {5}>",
                new object[] { costBaseAdjustment.TransactionDate, costBaseAdjustment.ASXCode, costBaseAdjustment.Description, costBaseAdjustment.Percentage, costBaseAdjustment.RecordDate, costBaseAdjustment.Comment });
        }
    }

    public class DisposalComparer : IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            var disposal1 = transaction1 as Disposal;
            var disposal2 = transaction2 as Disposal;

            return (disposal1.AveragePrice == disposal2.AveragePrice)
                && (disposal1.CGTMethod == disposal2.CGTMethod)
                && (disposal1.Comment == disposal2.Comment)
                && (disposal1.TransactionCosts == disposal2.TransactionCosts)
                && (disposal1.Units == disposal2.Units);
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var disposal = transaction as Disposal;

            writer.Write("<Disposal:- Transactionate: {0:d}, ASXCode: {1}, Description: {2}, AveragePrice: {3}, CGTMethod: {4}, TransactionCosts: {5}, Units: {6}, Comment: {7}>",
                new object[] { disposal.TransactionDate, disposal.ASXCode, disposal.Description, disposal.AveragePrice, disposal.CGTMethod, disposal.TransactionCosts, disposal.Units, disposal.Comment });
        }
    }

    public class IncomeReceivedComparer : IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            var incomeReceived1 = transaction1 as IncomeReceived;
            var incomeReceived2 = transaction2 as IncomeReceived;

            return (incomeReceived1.Comment == incomeReceived2.Comment)
                && (incomeReceived1.FrankedAmount == incomeReceived2.FrankedAmount)
                && (incomeReceived1.FrankingCredits == incomeReceived2.FrankingCredits)
                && (incomeReceived1.Interest == incomeReceived2.Interest)
                && (incomeReceived1.RecordDate == incomeReceived2.RecordDate)
                && (incomeReceived1.TaxDeferred == incomeReceived2.TaxDeferred)
                && (incomeReceived1.UnfrankedAmount == incomeReceived2.UnfrankedAmount);
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var incomeReceived = transaction as IncomeReceived;

            writer.Write("<IncomeReceived:- Transactionate: {0:d}, ASXCode: {1}, Description: {2}, FrankedAmount: {3}, UnfrankedAmount: {4}, FrankingCredits: {5}, TaxDeferred: {6}, Interest: {7}, RecordDate: {8:d}, Comment: {9}>",
                new object[] { incomeReceived.TransactionDate, incomeReceived.ASXCode, incomeReceived.Description, incomeReceived.FrankedAmount, incomeReceived.UnfrankedAmount, incomeReceived.FrankingCredits, incomeReceived.TaxDeferred, incomeReceived.Interest, incomeReceived.RecordDate, incomeReceived.Comment });
        }
    }

    public class UnitCountAdjustmentComparer : IEntityComparer<ITransaction>
    {
        public bool Equals(ITransaction transaction1, ITransaction transaction2)
        {
            var unitCountAdjustment1 = transaction1 as UnitCountAdjustment;
            var unitCountAdjustment2 = transaction2 as UnitCountAdjustment;

            return (unitCountAdjustment1.Comment == unitCountAdjustment2.Comment)
                && (unitCountAdjustment1.NewUnits == unitCountAdjustment2.NewUnits)
                && (unitCountAdjustment1.OriginalUnits == unitCountAdjustment2.OriginalUnits);
        }

        public void Write(MessageWriter writer, ITransaction transaction)
        {
            var unitCountAdjustment = transaction as UnitCountAdjustment;

            writer.Write("<UnitCountAdjustment:- Transactionate: {0:d}, ASXCode: {1}, Description: {2}, NewUnits: {3}, OriginalUnits: {4}, Comment: {5}>",
                new object[] { unitCountAdjustment.TransactionDate, unitCountAdjustment.ASXCode, unitCountAdjustment.Description, unitCountAdjustment.NewUnits, unitCountAdjustment.OriginalUnits, unitCountAdjustment.Comment });
        }
    }

}
