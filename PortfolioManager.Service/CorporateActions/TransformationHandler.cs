using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.CorporateActions
{
    class TransformationHandler : ICorporateActionHandler  
    {
        private readonly StockService _StockService;
        private readonly ParcelService _ParcelService;

        public TransformationHandler(StockService stockService, ParcelService parcelService)
        {
            _StockService = stockService;
            _ParcelService = parcelService;
        }

        public IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction)
        {
            var transformation = corporateAction as Transformation;

            var transactions = new List<Transaction>();

            /* locate parcels that the transformation applies to */
            var transfomationStock = _StockService.Get(transformation.Stock, transformation.ActionDate);
            var ownedParcels = _ParcelService.GetParcels(transfomationStock, transformation.ActionDate);
            if (ownedParcels.Count == 0)
                return transactions;

            decimal totalCostBase = ownedParcels.Sum(x => x.CostBase);

            if (transformation.RolloverRefliefApplies)
            {
                foreach (var parcel in ownedParcels)
                {
                    /* create parcels for resulting stock */
                    foreach (ResultingStock resultingStock in transformation.ResultingStocks)
                    {
                        int units = (int)Math.Round(parcel.Units * ((decimal)resultingStock.NewUnits / (decimal)resultingStock.OriginalUnits));
                        decimal costBase = parcel.CostBase * resultingStock.CostBase;
                        var stock = _StockService.Get(resultingStock.Stock, transformation.ImplementationDate);

                        transactions.Add(new OpeningBalance()
                        {
                            TransactionDate = transformation.ImplementationDate,
                            ASXCode = stock.ASXCode,
                            Units = units,
                            CostBase = costBase,
                            AquisitionDate = parcel.AquisitionDate,
                            RecordDate = transformation.ImplementationDate,
                            Comment = transformation.Description
                        });
                    }
                }

                /* Reduce the costbase of the original parcels */
                if (transformation.ResultingStocks.Any())
                {
                    decimal originalCostBasePercentage = 1 - transformation.ResultingStocks.Sum(x => x.CostBase);
                    var stock = _StockService.Get(transformation.Stock, transformation.ImplementationDate);

                    transactions.Add(new CostBaseAdjustment()
                    {
                        TransactionDate = transformation.ImplementationDate,
                        ASXCode = stock.ASXCode,
                        RecordDate = transformation.ActionDate,
                        Percentage = originalCostBasePercentage,
                        Comment = transformation.Description
                    });
                }
            }
            else
            {
                int totalUnits = ownedParcels.Sum(x => x.Units);
                decimal capitalReturn = 0.00m;

                /* create parcels for resulting stock */
                foreach (ResultingStock resultingStock in transformation.ResultingStocks)
                {
                    int units = (int)Math.Round(totalUnits * ((decimal)resultingStock.NewUnits / (decimal)resultingStock.OriginalUnits));
                    decimal costBase = totalCostBase * resultingStock.CostBase;
                    capitalReturn += units * resultingStock.CostBase;
                    var stock = _StockService.Get(resultingStock.Stock, transformation.ImplementationDate);

                    transactions.Add(new Aquisition()
                    {
                        TransactionDate = resultingStock.AquisitionDate,
                        ASXCode = stock.ASXCode,
                        Units = units,
                        AveragePrice = resultingStock.CostBase,
                        TransactionCosts = 0.00m,
                        RecordDate = resultingStock.AquisitionDate,
                        Comment = transformation.Description
                    });
                }

                /* Reduce the costbase of the original parcels */
                if (capitalReturn != 0.00m)
                {
                    var stock = _StockService.Get(transformation.Stock, transformation.ImplementationDate);

                    transactions.Add(new ReturnOfCapital()
                    {
                        TransactionDate = transformation.ImplementationDate,
                        ASXCode = stock.ASXCode,
                        RecordDate = transformation.ActionDate,
                        Amount = capitalReturn,
                        Comment = transformation.Description
                    });
                }

            }

            /* Handle disposal of original parcels */
            if (transformation.CashComponent > 0)
            {
                var stock = _StockService.Get(transformation.Stock, transformation.ImplementationDate);

                transactions.Add(new Disposal()
                {
                    TransactionDate = transformation.ImplementationDate,
                    ASXCode = stock.ASXCode,
                    Units = ownedParcels.Sum(x => x.Units),
                    AveragePrice = transformation.CashComponent,
                    TransactionCosts = 0.00m,
                    CGTMethod = CGTCalculationMethod.FirstInFirstOut,
                    RecordDate = transformation.ImplementationDate,
                    Comment = transformation.Description
                });
            }

            return transactions.AsReadOnly();
        }

        public bool HasBeenApplied(CorporateAction corporateAction, TransactionService transactionService)
        {
            Transformation transformation = corporateAction as Transformation;

            IReadOnlyCollection<Transaction> transactions;
            string asxCode;

            if (transformation.ResultingStocks.Any())
            {
                asxCode = _StockService.Get(transformation.ResultingStocks.First().Stock, transformation.ImplementationDate).ASXCode;
                transactions = transactionService.GetTransactions(asxCode, TransactionType.OpeningBalance, transformation.ImplementationDate, transformation.ImplementationDate);
            }
            else
            { 
                asxCode = _StockService.Get(transformation.Stock, transformation.ImplementationDate).ASXCode;

                transactions = transactionService.GetTransactions(asxCode, TransactionType.Disposal, transformation.ImplementationDate, transformation.ImplementationDate);
            }
            return (transactions.Count() > 0);
        }

    }


}
