﻿using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.Portfolios.Events;
using PortfolioManager.Domain.Transactions;
using PortfolioManager.Domain.Transactions.Events;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.Portfolios
{

    public class Portfolio : TrackedEntity
    {
        private ServiceFactory<ITransactionHandler> _TransactionHandlers = new ServiceFactory<ITransactionHandler>();

        public string Name { get; private set; }
        public Guid Owner { get; private set; }

        private IStockResolver _StockResolver;

        private HoldingCollection _Holdings = new HoldingCollection();
        public IHoldingCollection Holdings => _Holdings;

        private TransactionCollection _Transactions = new TransactionCollection();
        public ITransactionCollection Transactions => _Transactions;

        private CashAccount _CashAccount = new CashAccount();
        public ICashAccount CashAccount => _CashAccount;

        private CgtEventCollection _CgtEvents = new CgtEventCollection();
        public ICgtEventCollection CgtEvents => _CgtEvents;

        public DateTime StartDate
        {
            get { return DateUtils.Earlist(_Transactions.Earliest, _CashAccount.Transactions.Earliest); }
        }

        public DateTime EndDate
        {
            get { return DateUtils.NoEndDate; }
        }

        public Portfolio(Guid id, IStockResolver stockResolver)
            : base(id)
        {
            _StockResolver = stockResolver;

            _TransactionHandlers.Register<Aquisition>(() => new AquisitionHandler(_Holdings, _CashAccount));
            _TransactionHandlers.Register<Disposal>(() => new DisposalHandler(_Holdings, _CashAccount, _CgtEvents));
            _TransactionHandlers.Register<CashTransaction>(() => new CashTransactionHandler(_CashAccount));
            _TransactionHandlers.Register<OpeningBalance>(() => new OpeningBalanceHandler(_Holdings, _CashAccount));
            _TransactionHandlers.Register<IncomeReceived>(() => new IncomeReceivedHandler(_Holdings, _CashAccount));
            _TransactionHandlers.Register<ReturnOfCapital>(() => new ReturnOfCapitalHandler(_Holdings, _CashAccount));
        }

        public void Create(string name, Guid owner)
        {
            var @event = new PortfolioCreatedEvent(Id, Version, name, owner);
            Apply(@event);

            PublishEvent(@event);
        }

        public void Apply(PortfolioCreatedEvent @event)
        {
            Version++;

            Name = @event.Name;
            Owner = @event.Owner;
        }

        public void ChangeDrpParticipation(Guid holding, bool participateInDrp)
        {
            var @event = new DrpParticipationChangedEvent(Id, Version, holding, participateInDrp);
            Apply(@event);

            PublishEvent(@event);
        }

        public void Apply(DrpParticipationChangedEvent @event)
        {
            Version++;

            var holding = _Holdings.Get(@event.Holding);
            if (holding != null)
                holding.ChangeDrpParticipation(@event.ParticipateInDrp);
        }

        public void MakeCashTransaction(DateTime transactionDate, BankAccountTransactionType type, decimal amount, string comment, Guid transactionId)
        {
            var @event = new CashTransactionOccurredEvent(Id, Version, transactionId, transactionDate, Guid.Empty, comment)
            {
                CashTransactionType = type,
                Amount = amount
            };
            Apply(@event);

            PublishEvent(@event);
        }

        public void Apply(CashTransactionOccurredEvent @event)
        {
            var cashTransaction = new CashTransaction();
            MapFieldsFromEvent(cashTransaction, @event);
            cashTransaction.CashTransactionType = @event.CashTransactionType;
            cashTransaction.Amount = @event.Amount;

            var handler = _TransactionHandlers.GetService<CashTransaction>();
            handler.ApplyTransaction(cashTransaction);
            _Transactions.Add(cashTransaction);
        }

        public void AquireShares(DateTime aquisitionDate, Stock stock, int units, decimal averagePrice, decimal transactionCosts, bool createCashTransaction, string comment, Guid transactionId)
        {
            var @event = new AquisitionOccurredEvent(Id, Version, transactionId, aquisitionDate, stock.Id, comment)
            {
                Units = units,
                AveragePrice = averagePrice,
                TransactionCosts = transactionCosts,
                CreateCashTransaction = createCashTransaction
            };
            Apply(@event);

            PublishEvent(@event);
        }

        public void Apply(AquisitionOccurredEvent @event)
        {
            var aquisition = new Aquisition();
            MapFieldsFromEvent(aquisition, @event);
            aquisition.Units = @event.Units;
            aquisition.AveragePrice = @event.AveragePrice;
            aquisition.TransactionCosts = @event.TransactionCosts;
            aquisition.CreateCashTransaction = @event.CreateCashTransaction;

            var handler = _TransactionHandlers.GetService<Aquisition>();
            handler.ApplyTransaction(aquisition);
            _Transactions.Add(aquisition);
        }

        public void DisposeOfShares(DateTime disposalDate, Stock stock, int units, decimal averagePrice, decimal transactionCosts, CGTCalculationMethod cgtMethod, bool createCashTransaction, string comment, Guid transactionId)
        {
            var @event = new DisposalOccurredEvent(Id, Version, transactionId, disposalDate, stock.Id, comment)
            {
                Units = units,
                AveragePrice = averagePrice,
                TransactionCosts = transactionCosts,
                CGTMethod = cgtMethod,
                CreateCashTransaction = createCashTransaction
            };
            Apply(@event);

            PublishEvent(@event);
        }

        public void Apply(DisposalOccurredEvent @event)
        {
            var disposal = new Disposal();
            MapFieldsFromEvent(disposal, @event);
            disposal.Units = @event.Units;
            disposal.AveragePrice = @event.AveragePrice;
            disposal.TransactionCosts = @event.TransactionCosts;
            disposal.CreateCashTransaction = @event.CreateCashTransaction;

            var handler = _TransactionHandlers.GetService<Disposal>();
            handler.ApplyTransaction(disposal);
            _Transactions.Add(disposal);
        }

        public void IncomeReceived(DateTime recordDate, DateTime paymentDate, Stock stock, decimal frankedAmount, decimal unfrankedAmount, decimal frankingCredits, decimal interest, decimal taxDeferred, decimal drpCashBalance, bool createCashTransaction, string comment, Guid transactionId)
        {
            var @event = new IncomeOccurredEvent(Id, Version, transactionId, paymentDate, stock.Id, comment)
            {
                RecordDate = recordDate,
                FrankedAmount = frankedAmount,
                UnfrankedAmount = unfrankedAmount,
                FrankingCredits = frankingCredits,
                Interest = interest,
                TaxDeferred = taxDeferred,
                CreateCashTransaction = createCashTransaction,
                DRPCashBalance = drpCashBalance
            };
            Apply(@event);

            PublishEvent(@event);
        }

        public void Apply(IncomeOccurredEvent @event)
        {
            var incomeReceived = new IncomeReceived();
            MapFieldsFromEvent(incomeReceived, @event);
            incomeReceived.RecordDate = @event.RecordDate;
            incomeReceived.FrankedAmount = @event.FrankedAmount;
            incomeReceived.UnfrankedAmount = @event.UnfrankedAmount;
            incomeReceived.FrankingCredits = @event.FrankingCredits;
            incomeReceived.Interest = @event.Interest;
            incomeReceived.TaxDeferred = @event.TaxDeferred;
            incomeReceived.CreateCashTransaction = @event.CreateCashTransaction;
            incomeReceived.DRPCashBalance = @event.DRPCashBalance;

            var handler = _TransactionHandlers.GetService<IncomeReceived>();
            handler.ApplyTransaction(incomeReceived);
            _Transactions.Add(incomeReceived);
        }

        public void AddOpeningBalance(DateTime transactionDate, DateTime aquisitionDate, Stock stock, int units, decimal costBase, string comment, Guid transactionId)
        {
            var @event = new OpeningBalanceOccurredEvent(Id, Version, transactionId, transactionDate, stock.Id, comment)
            {
                AquisitionDate = aquisitionDate,
                Units = units,
                CostBase = costBase
            };
            Apply(@event);

            PublishEvent(@event);
        }

        public void Apply(OpeningBalanceOccurredEvent @event)
        {
            var openingBalance = new OpeningBalance();
            MapFieldsFromEvent(openingBalance, @event);
            openingBalance.AquisitionDate = @event.AquisitionDate;
            openingBalance.Units = @event.Units;
            openingBalance.CostBase = @event.CostBase;

            var handler = _TransactionHandlers.GetService<OpeningBalance>();
            handler.ApplyTransaction(openingBalance);
            _Transactions.Add(openingBalance);
        }

        public void ReturnOfCapitalReceived(DateTime paymentDate, DateTime recordDate, Stock stock, decimal amount, bool createCashTransaction, string comment, Guid transactionId)
        {

        }

        public void Apply(ReturnOfCapitalOccurredEvent @event)
        {

        }

        private void MapFieldsFromEvent(Transaction transaction, TransactionOccurredEvent @event)
        {
            transaction.Id = @event.TransactionId;
            transaction.Date = @event.Date;
            transaction.Stock = _StockResolver.GetStock(@event.Stock);
            transaction.Comment = @event.Comment;
        }
    }
}
