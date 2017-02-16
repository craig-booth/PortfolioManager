using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Service.Utils;

using StockManager.Service;

namespace PortfolioManager.Service
{
    interface IServiceHandler
    {
        object HandleRequest(object request);
    }

    public class PortfolioService
    {
        private readonly Dictionary<Type, IServiceHandler> _ServiceHandlers;

        private readonly PortfolioSettingsService _SettingsService;
        private readonly StockService _StockService;
        private readonly ParcelService _ParcelService;
        private readonly TransactionService _TransactionService;
        private readonly CashAccountService _CashAccountService;
        private readonly ShareHoldingService _ShareHoldingService;
        private readonly AttachmentService _AttachmentService;
        private readonly IncomeService _IncomeService;
        private readonly CGTService _CGTService;

        public PortfolioService(IPortfolioDatabase database, StockServiceRepository stockServiceRepository, IStockQuery stockQuery, ICorporateActionQuery corporateActionQuery)
        {
            _SettingsService = new PortfolioSettingsService();

            _StockService = new StockService(stockServiceRepository);
            _ParcelService = new ParcelService(database.PortfolioQuery, _StockService);
            _TransactionService = new TransactionService(database, _ParcelService, _StockService, _AttachmentService);
            _CashAccountService = new CashAccountService(database);
            _ShareHoldingService = new ShareHoldingService(_ParcelService, _StockService, _TransactionService, _CashAccountService);
            _AttachmentService = new AttachmentService(database);
            _IncomeService = new IncomeService(database.PortfolioQuery, _StockService, _SettingsService);
            _CGTService = new CGTService(database.PortfolioQuery);


            _ServiceHandlers = new Dictionary<Type, IServiceHandler>();
            _ServiceHandlers.Add(typeof(PortfolioSummaryRequest), new PortfolioSummaryHandler(_ShareHoldingService, _CashAccountService));
            _ServiceHandlers.Add(typeof(PortfolioPerformanceRequest), new PortfolioPerformanceHandler(_ShareHoldingService, _CashAccountService, _TransactionService, _StockService, _IncomeService));
        }

        public Responce HandleRequest<Request, Responce>(Request request)
        {
            IServiceHandler handler;
            if (_ServiceHandlers.TryGetValue(typeof(Request), out handler))
                return (Responce)handler.HandleRequest(request);
            else
                throw new NotSupportedException();
        }
    }
}
