using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Service.CorporateActions;

namespace PortfolioManager.Service
{
    public class CorporateActionService
    {
        private readonly ICorporateActionQuery _CorporateActionQuery;
        private readonly ParcelService _ParcelService;
        private readonly StockService _StockService;
        private readonly TransactionService _TransactionService;
        private readonly ShareHoldingService _ShareHoldingService;

        private readonly Dictionary<CorporateActionType, ICorporateActionHandler> _CorporateActionHandlers;

        internal CorporateActionService(ICorporateActionQuery corporateActionQuery, ParcelService parcelService, StockService stockService, TransactionService transactionService, ShareHoldingService shareHoldingService, StockSettingService stockSettingService)
        {
            _CorporateActionQuery = corporateActionQuery;
            _ParcelService = parcelService;
            _StockService = stockService;
            _TransactionService = transactionService;
            _ShareHoldingService = shareHoldingService;

            _CorporateActionHandlers = new Dictionary<CorporateActionType, ICorporateActionHandler>();

            /* Add corporate action handlers */
            _CorporateActionHandlers.Add(CorporateActionType.CapitalReturn, new CapitalReturnHandler(_StockService, _ParcelService));
            _CorporateActionHandlers.Add(CorporateActionType.Dividend, new DividendHandler(_StockService, _ParcelService, stockSettingService));
            _CorporateActionHandlers.Add(CorporateActionType.Transformation, new TransformationHandler(_StockService, _ParcelService));
            _CorporateActionHandlers.Add(CorporateActionType.SplitConsolidation, new SplitConsolidationHandler(_StockService, _ParcelService));
            _CorporateActionHandlers.Add(CorporateActionType.Composite, new CompositeActionHandler(_StockService, _ParcelService, this));
        }

        public IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction)
        {
            var handler = _CorporateActionHandlers[corporateAction.Type];
            if (handler == null)
                throw new NotSupportedException("Transaction type not supported");

            return handler.CreateTransactionList(corporateAction);
        }

        public IReadOnlyCollection<CorporateAction> GetUnappliedCorporateActions(Stock stock, DateTime fromDate, DateTime toDate)
        {    
            var allCorporateActions = new List<CorporateAction>();

            var corporateActions = _CorporateActionQuery.Find(stock.Id, fromDate, toDate);
            AddUnappliedCorporateActions(allCorporateActions, corporateActions);

            // Sort by Action Date 
            allCorporateActions.Sort(new CorporateActionComparer());

            return allCorporateActions.AsReadOnly(); 
        }

        public IReadOnlyCollection<CorporateAction> GetUnappliedCorporateActions()
        {
            // Get a list of all stocks held
            var allOwnedStocks = _ShareHoldingService.GetOwnedStockIds(DateTime.Today);

            var allCorporateActions = new List<CorporateAction>();
            foreach (var ownedStock in allOwnedStocks)
            {
                var corporateActions = _CorporateActionQuery.Find(ownedStock.Id, ownedStock.FromDate, ownedStock.ToDate);
                AddUnappliedCorporateActions(allCorporateActions, corporateActions);
            }

            // Sort by Action Date 
            allCorporateActions.Sort(new CorporateActionComparer());

            return allCorporateActions.AsReadOnly(); 
        }

        public bool HasBeenApplied(CorporateAction corporateAction)
        {
            var handler = _CorporateActionHandlers[corporateAction.Type];
            if (handler == null)
                return false;

            return (handler.HasBeenApplied(corporateAction, _TransactionService));
        }

        internal void AddUnappliedCorporateActions(IList<CorporateAction> toList, IEnumerable<CorporateAction> fromList)
        {

            foreach (CorporateAction corporateAction in fromList)
            {
                if (!HasBeenApplied(corporateAction))
                    toList.Add(corporateAction);
            }
        }
    }
}
