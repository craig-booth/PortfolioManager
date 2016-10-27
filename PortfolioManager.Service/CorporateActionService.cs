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
        private readonly ShareHoldingService _ShareHoldingService;
        private readonly TransactionService _TransactionService;

        private readonly Dictionary<CorporateActionType, ICorporateActionHandler> _CorporateActionHandlers;

        internal CorporateActionService(ICorporateActionQuery corporateActionQuery, ParcelService parcelService, StockService stockService, TransactionService transactionService, ShareHoldingService shareHoldingService, PortfolioSettingsService portfolioSettingService)
        {
            _CorporateActionQuery = corporateActionQuery;
            _ShareHoldingService = shareHoldingService;
            _TransactionService = transactionService;

            _CorporateActionHandlers = new Dictionary<CorporateActionType, ICorporateActionHandler>();

            /* Add corporate action handlers */
            _CorporateActionHandlers.Add(CorporateActionType.CapitalReturn, new CapitalReturnHandler(stockService, parcelService));
            _CorporateActionHandlers.Add(CorporateActionType.Dividend, new DividendHandler(stockService, parcelService, portfolioSettingService));
            _CorporateActionHandlers.Add(CorporateActionType.Transformation, new TransformationHandler(stockService, parcelService));
            _CorporateActionHandlers.Add(CorporateActionType.SplitConsolidation, new SplitConsolidationHandler(stockService, parcelService));
            _CorporateActionHandlers.Add(CorporateActionType.Composite, new CompositeActionHandler(stockService, parcelService, this));
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
