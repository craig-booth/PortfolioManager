using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Test.Portfolios.Transactions;

namespace PortfolioManager.Model.Test.Portfolios.ActualScenarios
{

    [TestFixture, Description("News corp demerger and sale")]
    public class NewsCorpDemergerAndSale : TransactionTestWithExpectedTests
    {

        protected override void AddStocks()
        {

            var NWSLV = _StockManager.Add("NWSLV", "News Corporation");
            var NNCLV = _StockManager.Add("NNCLV", "New News Corp");

            NWSLV.ChangeASXCode(new DateTime(2013, 07, 02), "FOXLV", "21st century fox");
            NNCLV.ChangeASXCode(new DateTime(2013, 09, 02), "NWSLV", "News Corporation post demerger");

        }

        public override void PerformTest()
        {
            var transactions = new ITransaction[]
            {
                new Aquisition()
                {
                    TransactionDate = new DateTime(2004, 11, 24),
                    ASXCode = "NWSLV",
                    Units = 235,
                    AveragePrice = 23.06m,
                    TransactionCosts = 19.95m,
                    Comment = "Parcel 1"
                },
                new Aquisition()
                {
                    TransactionDate = new DateTime(2007, 07, 27),
                    ASXCode = "NWSLV",
                    Units = 40,
                    AveragePrice = 21.76m,
                    TransactionCosts = 19.95m,
                    Comment = "Parcel 2"
                },
                new IncomeReceived()
                {
                    TransactionDate = new DateTime(2013, 06, 21),
                    RecordDate = new DateTime(2013, 06, 21),
                    ASXCode = "NWSLV",
                    FrankedAmount = 0.00m,
                    UnfrankedAmount = 0.00m,
                    Interest = 0.00m,
                    TaxDeferred = 12.20m,
                    FrankingCredits = 0.00m,
                    Comment = "Amount received for fractional shares"
                },
                new Aquisition()
                {
                    TransactionDate = new DateTime(2013, 06, 28),
                    ASXCode = "NNCLV",
                    Units = 68,
                    AveragePrice = 16.60m,
                    TransactionCosts = 0.00m,
                    Comment = "Received from demereger"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = new DateTime(2013, 06, 28),
                    ASXCode = "NWSLV",
                    Amount = 4.15m,
                    Comment = "Return of capital for NNCLV stocks"
                },
                new Disposal()
                {
                    TransactionDate = new DateTime(2013, 07, 22),
                    ASXCode = "NNCLV",
                    Units = 68,
                    AveragePrice = 17.32m,
                    TransactionCosts = 19.95m,
                    Comment = "Disposal of NNCLV"
                },
                new Disposal()
                {
                    TransactionDate = new DateTime(2015, 02, 10),
                    ASXCode = "FOXLV",
                    Units = 275,
                    AveragePrice = 35.79m,
                    TransactionCosts = 19.95m,
                    Comment = "Disposal of FOXLV"
                }
            };
            _Portfolio.ProcessTransactions(transactions);

            _TransactionDate = new DateTime(2015, 02, 10);

            //  _ExpectedParcels.Add(new ShareParcel(new DateTime(2013, 06, 28), _StockManager.GetStock("NNCLV", _TransactionDate).Id, 68, 16.60m, 68 * 16.60m, 68 * 16.60m, ParcelEvent.Aquisition));
            //  _ExpectedParcels.Add(new ShareParcel(new DateTime(2004, 11, 24), _StockManager.GetStock("FOXLV", _TransactionDate).Id, 235, 23.06m, 235 * 23.06m, 4453.37m, ParcelEvent.CostBaseReduction)
            //  {
            //      FromDate = new DateTime(2013, 06, 28)
            //  });
            //  _ExpectedParcels.Add(new ShareParcel(new DateTime(2007, 07, 27), _StockManager.GetStock("FOXLV", _TransactionDate).Id, 40, 21.76m, 40 * 21.76m, 722.58m, ParcelEvent.CostBaseReduction)
            //  {
            //      FromDate = new DateTime(2013, 06, 28)
            //  });

            _ExpectedIncome.Add(new IncomeReceived()
            {
                TransactionDate = new DateTime(2013, 06, 21),
                RecordDate = new DateTime(2013, 06, 21),
                ASXCode = "NWSLV",
                FrankedAmount = 0.00m,
                UnfrankedAmount = 0.00m,
                Interest = 0.00m,
                TaxDeferred = 12.20m,
                FrankingCredits = 0.00m,
                Comment = "Amount received for fractional shares"
            });

            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("NNCLV", new DateTime(2013, 07, 22)).Id, new DateTime(2013, 07, 22), 68, 68 * 16.60m, (68 * 17.32m) - 19.95m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("FOXLV", new DateTime(2015, 02, 10)).Id, new DateTime(2015, 02, 10), 235, 4453.37m, (235 * 35.79m) - 17.05m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("FOXLV", new DateTime(2015, 02, 10)).Id, new DateTime(2015, 02, 10), 40, 722.58m, (40 * 35.79m) - 2.90m));
        }
    }
}

