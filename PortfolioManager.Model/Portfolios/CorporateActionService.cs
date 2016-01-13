using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{
    public class CorporateActionService
    {
        private readonly ICorporateActionQuery _CorporateActionQuery;
        private readonly ParcelService _ParcelService;
        private readonly StockService _StockService;
        private readonly TransactionService _TransactionService;

        private readonly Dictionary<CorporateActionType, ICorporateActionHandler> _CorporateActionHandlers;

        internal CorporateActionService(ICorporateActionQuery corporateActionQuery, ParcelService parcelService, StockService stockService, TransactionService transactionService)
        {
            _CorporateActionQuery = corporateActionQuery;
            _ParcelService = parcelService;
            _StockService = stockService;
            _TransactionService = transactionService;


            _CorporateActionHandlers = new Dictionary<CorporateActionType, ICorporateActionHandler>();

            /* Add corporate action handlers */
            _CorporateActionHandlers.Add(CorporateActionType.CapitalReturn, new CapitalReturnHandler(_StockService, _ParcelService));
            _CorporateActionHandlers.Add(CorporateActionType.Dividend, new DividendHandler(_StockService, _ParcelService));
            _CorporateActionHandlers.Add(CorporateActionType.Transformation, new TransformationHandler(_StockService, _ParcelService));
            _CorporateActionHandlers.Add(CorporateActionType.SplitConsolidation, new SplitConsolidationHandler(_StockService));
        }

        public IReadOnlyCollection<ITransaction> CreateTransactionList(ICorporateAction corporateAction)
        {
            var handler = _CorporateActionHandlers[corporateAction.Type];
            if (handler == null)
                throw new NotSupportedException("Transaction type not supported");

            return handler.CreateTransactionList(corporateAction);
        }

        public IReadOnlyCollection<ICorporateAction> GetUnappliedCorparateActions(Stock stock, DateTime fromDate, DateTime toDate)
        {    
            var allCorporateActions = new List<ICorporateAction>();

            var corporateActions = _CorporateActionQuery.Find(stock.Id, fromDate, toDate);
            AddUnappliedCorporateActions(allCorporateActions, corporateActions);

            // Sort by Action Date 
            allCorporateActions.Sort(new CorporateActionComparer());

            return allCorporateActions.AsReadOnly(); 
        }

        public IReadOnlyCollection<ICorporateAction> GetUnappliedCorparateActions()
        {
            // Get a list of all stocks held
            var allOwnedStocks = GetStocksInPortfolio(DateTime.Today);

            var allCorporateActions = new List<ICorporateAction>();
            foreach (OwnedStock ownedStock in allOwnedStocks)
            {
                var corporateActions = _CorporateActionQuery.Find(ownedStock.Id, ownedStock.FromDate, ownedStock.ToDate);
                AddUnappliedCorporateActions(allCorporateActions, corporateActions);
            }

            // Sort by Action Date 
            allCorporateActions.Sort(new CorporateActionComparer());

            return allCorporateActions.AsReadOnly(); 
        }

        private IReadOnlyCollection<OwnedStock> GetStocksInPortfolio(DateTime date)
        {
            List<OwnedStock> ownedStocks = new List<OwnedStock>();

            var parcels = _ParcelService.GetParcels(date).OrderBy(x => x.Stock).ThenBy(x => x.FromDate);

            OwnedStock currentStock = null;
            foreach (var shareParcel in parcels)
            {
                if ((currentStock != null) && (shareParcel.Stock == currentStock.Id) && (shareParcel.FromDate < currentStock.ToDate))
                {
                    if (shareParcel.ToDate > currentStock.ToDate)
                        currentStock.ToDate = shareParcel.ToDate;
                }
                else
                {
                    currentStock = new OwnedStock()
                    {
                        Id = shareParcel.Stock,
                        FromDate = shareParcel.FromDate,
                        ToDate = shareParcel.ToDate
                    };
                    ownedStocks.Add(currentStock);
                }
            }

            return ownedStocks.AsReadOnly();
        }

        internal void AddUnappliedCorporateActions(IList<ICorporateAction> toList, IEnumerable<ICorporateAction> fromList)
        {

            foreach (ICorporateAction corporateAction in fromList)
            {
                IReadOnlyCollection<ITransaction> transactions;
                TransactionType type;
                DateTime date;
                string asxCode;

                if (corporateAction.Type == CorporateActionType.Dividend)
                {
                    Dividend dividend = corporateAction as Dividend;
                    date = dividend.PaymentDate;
                    type = TransactionType.Income;
                    asxCode = _StockService.Get(dividend.Stock, date).ASXCode;
                }
                else if (corporateAction.Type == CorporateActionType.CapitalReturn)
                {
                    CapitalReturn capitalReturn = corporateAction as CapitalReturn;
                    date = capitalReturn.PaymentDate;
                    type = TransactionType.ReturnOfCapital;
                    asxCode = _StockService.Get(capitalReturn.Stock, date).ASXCode;
                }
                else if (corporateAction.Type == CorporateActionType.Transformation)
                {
                    Transformation transformation = corporateAction as Transformation;
                    date = transformation.ImplementationDate;

                    if (transformation.ResultingStocks.Any())
                    {
                        type = TransactionType.OpeningBalance;
                        asxCode = _StockService.Get(transformation.ResultingStocks.First().Stock, date).ASXCode;
                    }
                    else
                    {
                        type = TransactionType.Disposal;
                        asxCode = _StockService.Get(transformation.Stock, date).ASXCode;
                    }

                }
                else
                    continue;

                transactions = _TransactionService.GetTransactions(asxCode, type, date, date);
                if (transactions.Count() == 0)
                    toList.Add(corporateAction);
            }
        }
    }

    class OwnedStock
    {
        public Guid Id;
        public DateTime FromDate;
        public DateTime ToDate;
    } 
}
