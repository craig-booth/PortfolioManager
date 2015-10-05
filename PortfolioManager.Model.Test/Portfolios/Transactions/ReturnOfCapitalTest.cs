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

    [TestFixture, Description("Return of Capital of Ordinary share - single parcel")]
    public class ReturnOfCapitalOrdinaryShareSingleParcel : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new ITransaction[]
            {           
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    Comment = "Return of Capital test"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    Amount = 0.70m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.Transactions.Add(transactions);


            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockManager.GetStock("AAA", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 800.00m, ParcelEvent.CostBaseReduction)
                {
                    FromDate = _TransactionDate
                });
        }
    }


    [TestFixture, Description("Return of Capital of Ordinary share - greater than cost base")]
    public class ReturnOfCapitalOrdinaryShareGreaterThanCostBase : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    Comment = "Return of Capital test"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    Amount = 2.00m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.Transactions.Add(transactions);

            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockManager.GetStock("AAA", _TransactionDate).Id, 1000, 1.50m, 1000.00m, 0.00m, ParcelEvent.CostBaseReduction)
            {
                FromDate = _TransactionDate
            });

            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 1000, 1500.00m, 500.00m));
        }
    }


    [TestFixture, Description("Return of Capital of Ordinary share - multiple parcels")]
    public class ReturnOfCapitalOrdinaryShareMultipleParcels : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            DateTime aquisitionDate1 = new DateTime(2000, 01, 01);
            DateTime aquisitionDate2 = new DateTime(2001, 01, 01);
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    Comment = "Return of Capital test"
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    CostBase = 1200.00m,
                    Comment = "Return of Capital test"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    Amount = 0.10m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.Transactions.Add(transactions);

            _ExpectedParcels.Add(new ShareParcel(aquisitionDate1, _StockManager.GetStock("AAA", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 1400.00m, ParcelEvent.CostBaseReduction)
            {
                FromDate = _TransactionDate
            });
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate2, _StockManager.GetStock("AAA", _TransactionDate).Id, 500, 2.40m, 1200.00m, 1150.00m, ParcelEvent.CostBaseReduction)
            {
                FromDate = _TransactionDate
            });
        }
    }


    [TestFixture, Description("Return of Capital of Ordinary share - multiple parcels greater than cost base")]
    public class ReturnOfCapitalOrdinaryShareMultipleParcelsGreaterThanCostBase : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            DateTime aquisitionDate1 = new DateTime(2000, 01, 01);
            DateTime aquisitionDate2 = new DateTime(2001, 01, 01);
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    Comment = "Return of Capital test"
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    CostBase = 1200.00m,
                    Comment = "Return of Capital test"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    Amount = 1.80m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.Transactions.Add(transactions);

            _ExpectedParcels.Add(new ShareParcel(aquisitionDate1, _StockManager.GetStock("AAA", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 0.00m, ParcelEvent.CostBaseReduction)
            {
                FromDate = _TransactionDate
            });
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate2, _StockManager.GetStock("AAA", _TransactionDate).Id, 500, 2.40m, 1200.00m, 300.00m, ParcelEvent.CostBaseReduction)
            {
                FromDate = _TransactionDate
            });

            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 1000, 1500.00m, 300.00m));
        }
    }


    [TestFixture, Description("Return of Capital validation tests")]
    public class ReturnOfCapitalValidationTests : TransactionTest
    {
        [Test, Description("Return of Capital of Ordinary share - no parcels")]
        [ExpectedException(typeof(NoParcelsForTransaction))]
        public void NoParcelsForTransaction()
        {
            var transactions = new ITransaction[]
            {           
                new ReturnOfCapital()
                {
                    TransactionDate = new DateTime(2002, 01, 01),
                    ASXCode = "AAA",
                    Amount = 0.10m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.Transactions.Add(transactions);
        }

        [Test, Description("Return of Capital of  of Stapled Security")]
        [ExpectedException(typeof(TransctionNotSupportedForStapledSecurity))]
        public void TransctionNotSupportedForStapledSecurity()
        {
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 1500.00m,
                    Comment = "Return of Capital test"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = new DateTime(2002, 01, 01),
                    ASXCode = "SSS",
                    Amount = 0.10m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.Transactions.Add(transactions);
        }
    }

}
