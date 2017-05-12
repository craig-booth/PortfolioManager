﻿using System;
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

        private ParcelService ParcelService { get; set; }       
        private CGTService CGTService { get; set; }
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

            ParcelService = new ParcelService(database.PortfolioQuery, StockService);
            TransactionService = new TransactionService(database, ParcelService, StockService, AttachmentService);
            ShareHoldingService = new ShareHoldingService(database.PortfolioQuery, ParcelService, StockService, TransactionService);
            AttachmentService = new AttachmentService(database);
            IncomeService = new IncomeService(database.PortfolioQuery, StockService, SettingsService);
            CGTService = new CGTService(database.PortfolioQuery);
            CorporateActionService = new CorporateActionService(corporateActionQuery, ParcelService, StockService, TransactionService, ShareHoldingService, IncomeService);

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


