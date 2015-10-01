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

    [TestFixture, Description("Purchase stock")]
    public class AquisitionOrdinaryShare : TransactionTestWithExpectedTests 
    {
        public override void Setup()
        {
            _TransactionDate = new DateTime(2000, 01, 01);

            var aquisition = new Aquisition()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "AAA",
                Units = 1000,
                AveragePrice = 10.00m,
                TransactionCosts = 19.95m,
                Comment = "Test aquisition"
            };
            _Portfolio.Transactions.Add(aquisition);

            decimal costBase = 10019.95m; // (1000 * 10) + 19.95
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockManager.GetStock("AAA", _TransactionDate).Id, aquisition.Units, aquisition.AveragePrice, costBase, costBase, ParcelEvent.Aquisition));
        }
    }

    [TestFixture, Description("Purchase stapled security")]
    public class AquisitionStapledSecurity : TransactionTestWithExpectedTests
    {
        public override void Setup()
        {
            _TransactionDate = new DateTime(2000, 01, 01);

            var aquisition = new Aquisition()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "SSS",
                Units = 1000,
                AveragePrice = 10.00m,
                TransactionCosts = 19.95m,
                Comment = "Test aquisition"
            };
            _Portfolio.Transactions.Add(aquisition);

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var mainParcel = new ShareParcel(_TransactionDate, _StockManager.GetStock("SSS", _TransactionDate).Id, aquisition.Units, 10.00m, 10019.95m, 10019.95m, ParcelEvent.Aquisition)
            {
                IncludeInHoldings = true,
                IncludeInParcels = false
            };            
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockManager.GetStock("SSS1", _TransactionDate).Id, aquisition.Units, 1.00m, 1002.00m, 1002.00m, mainParcel.Id, ParcelEvent.Aquisition));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockManager.GetStock("SSS2", _TransactionDate).Id, aquisition.Units, 3.00m, 3005.98m, 3005.98m, mainParcel.Id, ParcelEvent.Aquisition));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockManager.GetStock("SSS3", _TransactionDate).Id, aquisition.Units, 6.00m, 6011.97m, 6011.97m, mainParcel.Id, ParcelEvent.Aquisition));
            _ExpectedParcels.Add(mainParcel);
        }
    }
}
