using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Service.Utils;

using StockManager.Service;

namespace PortfolioManager.Service.Obsolete
{

    class PortfolioChangedEventArgs : EventArgs
    {

    }

    delegate void PortfolioChangedEventHandler(PortfolioChangedEventArgs e);

    class Portfolio
    {
        private PortfolioSettingsService SettingsService { get; set; }
   
        private CorporateActionService CorporateActionService { get; set; }
        private AttachmentService AttachmentService { get; set; }
        private TransactionService TransactionService { get; set; }
        private ShareHoldingService ShareHoldingService { get; set; }
        private IncomeService IncomeService { get; set; }
        private StockService StockService { get; set; }

        public event PortfolioChangedEventHandler PortfolioChanged;

        protected void OnPortfolioChanged()
        {

            PortfolioChanged?.Invoke(new PortfolioChangedEventArgs());
        }

        public Portfolio(IPortfolioDatabase database, StockServiceRepository stockServiceRepository, IStockQuery stockQuery, ICorporateActionQuery corporateActionQuery)
        {
            SettingsService = new PortfolioSettingsService();

            StockService = new StockService(stockServiceRepository);

            TransactionService = new TransactionService(database, StockService, AttachmentService);
            ShareHoldingService = new ShareHoldingService(database.PortfolioQuery, StockService, TransactionService);
            AttachmentService = new AttachmentService(database);
            IncomeService = new IncomeService(database.PortfolioQuery, StockService, SettingsService);
            CorporateActionService = new CorporateActionService(database.PortfolioQuery, corporateActionQuery, StockService, TransactionService, ShareHoldingService, IncomeService);

            /* Load transactions */
            var allTransactions = TransactionService.GetTransactions(DateTime.MinValue, DateTime.MaxValue);
            using (IPortfolioUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                foreach (var transaction in allTransactions)
                    TransactionService.ApplyTransaction(unitOfWork, transaction);
                unitOfWork.Save();
            }

            TransactionService.PortfolioChanged += TransactionService_PortfolioChanged;
        }

        private void TransactionService_PortfolioChanged(PortfolioChangedEventArgs e)
        {
            OnPortfolioChanged();
        }
    }
}


