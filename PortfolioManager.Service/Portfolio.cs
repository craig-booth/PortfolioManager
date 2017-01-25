﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Service.Utils;

using StockManager.Service;

namespace PortfolioManager.Service
{

    public class PortfolioChangedEventArgs : EventArgs
    {

    }

    public delegate void PortfolioChangedEventHandler(PortfolioChangedEventArgs e);

    public class Portfolio
    {
        public PortfolioSettingsService SettingsService { get; private set; }

        public ParcelService ParcelService { get; private set; }
        public ShareHoldingService ShareHoldingService { get; private set; }
        public TransactionService TransactionService { get; private set; }
        public IncomeService IncomeService { get; private set; }
        public CGTService CGTService { get; private set; }
        public CorporateActionService CorporateActionService { get; private set; }
        public AttachmentService AttachmentService { get; private set; }
        public CashAccountService CashAccountService { get; private set; }

        public StockService StockService { get; private set; }

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
            CashAccountService = new CashAccountService(database);
            ShareHoldingService = new ShareHoldingService(ParcelService, StockService, TransactionService, CashAccountService);
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


