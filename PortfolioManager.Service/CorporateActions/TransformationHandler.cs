using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Service.CorporateActions
{
    class TransformationHandler : ICorporateActionHandler  
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly IStockQuery _StockQuery;

        public TransformationHandler(IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
        {
            _PortfolioQuery = portfolioQuery;
            _StockQuery = stockQuery;
        }

        public IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction)
        {
            var transformation = corporateAction as Transformation;

            var transactions = new List<Transaction>();

            /* locate parcels that the transformation applies to */
            var transfomationStock = _StockQuery.Get(transformation.Stock, transformation.ActionDate);
            var ownedParcels = _PortfolioQuery.GetParcelsForStock(transfomationStock.Id, transformation.ActionDate, transformation.ActionDate);
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
                        int units = (int)Math.Ceiling(parcel.Units * ((decimal)resultingStock.NewUnits / (decimal)resultingStock.OriginalUnits));
                        decimal costBase = parcel.CostBase * resultingStock.CostBase;
                        var stock = _StockQuery.Get(resultingStock.Stock, transformation.ImplementationDate);

                        transactions.Add(new OpeningBalance()
                        {
                            TransactionDate = transformation.ImplementationDate,
                            ASXCode = stock.ASXCode,
                            Units = units,
                            CostBase = costBase,
                            AquisitionDate = parcel.AquisitionDate,
                            RecordDate = transformation.ImplementationDate,
                            PurchaseId = parcel.PurchaseId,
                            Comment = transformation.Description
                        });
                    }
                }

                /* Reduce the costbase of the original parcels */
                if (transformation.ResultingStocks.Any())
                {
                    decimal originalCostBasePercentage = 1 - transformation.ResultingStocks.Sum(x => x.CostBase);
                    var stock = _StockQuery.Get(transformation.Stock, transformation.ImplementationDate);

                    transactions.Add(new CostBaseAdjustment()
                    {
                        TransactionDate = transformation.ActionDate,
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
                    int units = (int)Math.Ceiling(totalUnits * ((decimal)resultingStock.NewUnits / (decimal)resultingStock.OriginalUnits));
                    decimal costBase = totalCostBase * resultingStock.CostBase;
                    capitalReturn += units * resultingStock.CostBase;
                    var stock = _StockQuery.Get(resultingStock.Stock, transformation.ImplementationDate);

                    transactions.Add(new OpeningBalance()
                    {
                        TransactionDate = transformation.ImplementationDate,
                        ASXCode = stock.ASXCode,
                        Units = units,
                        CostBase = costBase,
                        AquisitionDate = resultingStock.AquisitionDate,
                        RecordDate = transformation.ImplementationDate,
                        PurchaseId = transformation.Id,
                        Comment = transformation.Description
                    });
                }

                /* Reduce the costbase of the original parcels */
                if (capitalReturn != 0.00m)
                {
                    var stock = _StockQuery.Get(transformation.Stock, transformation.ImplementationDate);

                    transactions.Add(new ReturnOfCapital()
                    {
                        TransactionDate = transformation.ImplementationDate,
                        ASXCode = stock.ASXCode,
                        RecordDate = transformation.ActionDate,
                        Amount = capitalReturn,
                        CreateCashTransaction = false,
                        Comment = transformation.Description
                    });
                }

            }

            /* Handle disposal of original parcels */
            if (transformation.CashComponent > 0)
            {
                var stock = _StockQuery.Get(transformation.Stock, transformation.ImplementationDate);

                transactions.Add(new Disposal()
                {
                    TransactionDate = transformation.ImplementationDate,
                    ASXCode = stock.ASXCode,
                    Units = ownedParcels.Sum(x => x.Units),
                    AveragePrice = transformation.CashComponent,
                    TransactionCosts = 0.00m,
                    CGTMethod = CGTCalculationMethod.FirstInFirstOut,
                    RecordDate = transformation.ImplementationDate,
                    CreateCashTransaction = true,
                    Comment = transformation.Description
                });
            }

            return transactions.AsReadOnly();
        }

        public bool HasBeenApplied(CorporateAction corporateAction)
        {
            Transformation transformation = corporateAction as Transformation;

            IReadOnlyCollection<Transaction> transactions;
            string asxCode;

            if (transformation.ResultingStocks.Any())
            {
                asxCode = _StockQuery.GetASXCode(transformation.ResultingStocks.First().Stock, transformation.ImplementationDate);
                transactions = _PortfolioQuery.GetTransactions(asxCode, TransactionType.OpeningBalance, transformation.ImplementationDate, transformation.ImplementationDate);
            }
            else
            { 
                asxCode = _StockQuery.GetASXCode(transformation.Stock, transformation.ImplementationDate);

                transactions = _PortfolioQuery.GetTransactions(asxCode, TransactionType.Disposal, transformation.ImplementationDate, transformation.ImplementationDate);
            }
            return (transactions.Count() > 0);
        }

    }


}
