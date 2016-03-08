﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Test.Transactions
{

    [TestFixture, Description("Return of Capital of Ordinary share - single parcel")]
    public class ReturnOfCapitalOrdinaryShareSingleParcel : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new Transaction[]
            {           
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate,
                    Comment = "Return of Capital test"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    RecordDate = _TransactionDate,
                    Amount = 0.70m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);


            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 800.00m, ParcelEvent.CostBaseReduction)
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
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate,
                    Comment = "Return of Capital test"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    RecordDate = _TransactionDate,
                    Amount = 2.00m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 0.00m, ParcelEvent.CostBaseReduction)
            {
                FromDate = _TransactionDate
            });

            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 1000, 1500.00m, 500.00m));
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
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate1,
                    Comment = "Return of Capital test"
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    CostBase = 1200.00m,
                    AquisitionDate = aquisitionDate2,
                    Comment = "Return of Capital test"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    RecordDate = _TransactionDate,
                    Amount = 0.10m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            _ExpectedParcels.Add(new ShareParcel(aquisitionDate1, _StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 1400.00m, ParcelEvent.CostBaseReduction)
            {
                FromDate = _TransactionDate
            });
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate2, _StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, 500, 2.40m, 1200.00m, 1150.00m, ParcelEvent.CostBaseReduction)
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
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate1,
                    Comment = "Return of Capital test"
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    CostBase = 1200.00m,
                    AquisitionDate = aquisitionDate2,
                    Comment = "Return of Capital test"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    RecordDate = _TransactionDate,
                    Amount = 1.80m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            _ExpectedParcels.Add(new ShareParcel(aquisitionDate1, _StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 0.00m, ParcelEvent.CostBaseReduction)
            {
                FromDate = _TransactionDate
            });
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate2, _StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, 500, 2.40m, 1200.00m, 300.00m, ParcelEvent.CostBaseReduction)
            {
                FromDate = _TransactionDate
            });

            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 1000, 1500.00m, 300.00m));
        }
    }

    [TestFixture, Description("Return of Capital of  Child security - single parcels")]
    public class ReturnOfCapitalChildSecuritySingleParcels : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new Transaction[]
            {           
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 15000.00m,
                    AquisitionDate = aquisitionDate,
                    Comment = ""
                },
                new ReturnOfCapital()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "SSS3",
                    RecordDate = _TransactionDate,
                    Amount = 0.30m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var purchaseId = Guid.NewGuid();
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockServiceRepository.StockService.GetStock("SSS1", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 1500.00m, purchaseId, ParcelEvent.OpeningBalance));
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockServiceRepository.StockService.GetStock("SSS2", _TransactionDate).Id, 1000, 4.50m, 4500.00m, 4500.00m, purchaseId, ParcelEvent.OpeningBalance));
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockServiceRepository.StockService.GetStock("SSS3", _TransactionDate).Id, 1000, 9.00m, 9000.00m, 8700.00m, purchaseId, ParcelEvent.CostBaseReduction)
            {
                FromDate = _TransactionDate
            });

        }
    }

    [TestFixture, Description("Return of Capital validation tests")]
    public class ReturnOfCapitalValidationTests : PortfolioTest
    {
        [Test, Description("Return of Capital of Ordinary share - no parcels")]
        [ExpectedException(typeof(NoParcelsForTransaction))]
        public void NoParcelsForTransaction()
        {
            var transactions = new Transaction[]
            {           
                new ReturnOfCapital()
                {
                    TransactionDate = new DateTime(2002, 01, 01),
                    ASXCode = "AAA",
                    RecordDate = new DateTime(2002, 01, 01),
                    Amount = 0.10m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);
        }

        [Test, Description("Return of Capital of  of Stapled Security")]
        [ExpectedException(typeof(TransctionNotSupportedForStapledSecurity))]
        public void TransctionNotSupportedForStapledSecurity()
        {
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = new DateTime(2000, 01, 01),
                    Comment = "Return of Capital test"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = new DateTime(2002, 01, 01),
                    ASXCode = "SSS",
                    RecordDate = new DateTime(2002, 01, 01),
                    Amount = 0.10m,
                    Comment = "Return of Capital test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);
        }
    }

}