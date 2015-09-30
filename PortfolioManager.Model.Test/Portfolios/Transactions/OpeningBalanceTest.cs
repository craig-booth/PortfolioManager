using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios.Transactions
{

    [TestFixture, Description("Opening balance for ordinary share")]
    public class OpeningBalanceOrdinaryShare : TransactionTest
    {
        public override void Setup()
        {
            _TransactionDate = new DateTime(2000, 01, 01);

            var openingbalance = new OpeningBalance()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "AAA",
                Units = 1000,
                CostBase = 1500.00m,
                Comment = "Test Opening Balance"
            };
            _Portfolio.Transactions.Add(openingbalance);

            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockManager.GetStock("AAA", _TransactionDate).Id, openingbalance.Units, 1.50m, openingbalance.CostBase, openingbalance.CostBase, ParcelEvent.OpeningBalance));
        }
    }

    [TestFixture, Description("Opening balance for stapled security")]
    public class OpeningBalanceStapledSecurity : TransactionTest
    {
        public override void Setup()
        {
            _TransactionDate = new DateTime(2000, 01, 01);

            var openingbalance = new OpeningBalance()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "SSS",
                Units = 1000,
                CostBase = 15000.00m,
                Comment = "Test Opening Balance"
            };
            _Portfolio.Transactions.Add(openingbalance);

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var mainParcel = new ShareParcel(openingbalance.TransactionDate, _StockManager.GetStock("SSS", _TransactionDate).Id, openingbalance.Units, 15.00m, 15000.00m, 15000.00m, ParcelEvent.OpeningBalance);
            _ExpectedParcels.Add(mainParcel);
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockManager.GetStock("SSS1", _TransactionDate).Id, openingbalance.Units, 1.50m, 1500.00m, 1500.00m, mainParcel.Id, ParcelEvent.OpeningBalance));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockManager.GetStock("SSS2", _TransactionDate).Id, openingbalance.Units, 4.50m, 4500.00m, 4500.00m, mainParcel.Id, ParcelEvent.OpeningBalance));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockManager.GetStock("SSS3", _TransactionDate).Id, openingbalance.Units, 9.00m, 9000.00m, 9000.00m, mainParcel.Id, ParcelEvent.OpeningBalance));
        }
     }
}
