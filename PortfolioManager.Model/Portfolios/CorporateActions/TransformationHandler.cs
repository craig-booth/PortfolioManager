using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
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

        public IReadOnlyCollection<ITransaction> CreateTransactionList(ICorporateAction corporateAction)
        {
            var transformation = corporateAction as Transformation;

            var transactions = new List<ITransaction>();

            /* locate parcels that the transformation applies to */
            var transfomationStock = _StockService.Get(transformation.Stock, transformation.ActionDate);
            var ownedParcels = _ParcelService.GetParcels(transfomationStock, transformation.ActionDate);
            if (ownedParcels.Count == 0)
                return transactions;

            int totalUnits = ownedParcels.Sum(x => x.Units);
            decimal totalCostBase = ownedParcels.Sum(x => x.CostBase);

            /* create parcels for resulting stock */
            foreach (ResultingStock resultingStock in transformation.ResultingStocks)
            {
                int units = (int)Math.Round(totalUnits * ((decimal)resultingStock.NewUnits / (decimal)resultingStock.OriginalUnits));
                decimal costBase = totalCostBase * resultingStock.CostBasePercentage;
                var stock = _StockService.Get(resultingStock.Stock, transformation.ImplementationDate);
                transactions.Add(new OpeningBalance()
                {
                    TransactionDate = transformation.ImplementationDate,
                    ASXCode = stock.ASXCode,
                    Units = units,
                    CostBase = costBase,
                    AquisitionDate = transformation.ImplementationDate,
                    Comment = transformation.Description
                });
            }

            /* Reduce the costbase of the original parcels */
            if (transformation.ResultingStocks.Any())
            {
                decimal originalCostBasePercentage = 1 - transformation.ResultingStocks.Sum(x => x.CostBasePercentage);

                var stock = _StockService.Get(transformation.Stock, transformation.ImplementationDate);
                transactions.Add(new CostBaseAdjustment()
                {
                    TransactionDate = transformation.ImplementationDate,
                    ASXCode = stock.ASXCode,
                    Percentage = originalCostBasePercentage,
                    Comment = transformation.Description
                });
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
                    TransactionCosts = 0.00M,
                    CGTMethod = CGTCalculationMethod.FirstInFirstOut,
                    Comment = transformation.Description
                });
            }

            return transactions.AsReadOnly();
        }

        public bool HasBeenApplied(ICorporateAction corporateAction, TransactionService transactionService)
        {
            Transformation transformation = corporateAction as Transformation;

            IReadOnlyCollection<ITransaction> transactions;
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
