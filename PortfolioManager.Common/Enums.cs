using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Common
{
    public enum StockType { Ordinary, StapledSecurity, Trust }

    public enum DRPMethod { Round, RoundDown, RoundUp, RetainCashBalance }

    public enum AssetCategory { AustralianStocks, InternationalStocks, AustralianProperty, InternationalProperty, AustralianFixedInterest, InternationlFixedInterest, Cash }

    public enum CGTMethod { Other, Discount, Indexation }

    public enum CGTCalculationMethod { MinimizeGain, MaximizeGain, FirstInFirstOut, LastInFirstOut }

    public enum BankAccountTransactionType {Deposit, Withdrawl, Transfer, Fee, Interest }

    public enum TransactionType { Aquisition, Disposal, CostBaseAdjustment, OpeningBalance, ReturnOfCapital, Income, UnitCountAdjustment, CashTransaction }
}
