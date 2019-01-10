using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Utils;
using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Domain.Transactions
{

    public interface ITransactionHandler
    {
        void ApplyTransaction(Transaction transaction);
    }

    public interface ITransactionService
        : ITransactionList<Transaction>
    {
        void Apply(Transaction transaction);
        void Apply(IEnumerable<Transaction> transactions);
    }

    public class TransactionService :
        TransactionList<Transaction>,
        ITransactionService
    {
        private Dictionary<Type, ITransactionHandler> _Handlers = new Dictionary<Type, ITransactionHandler>();

        public TransactionService(HoldingCollection holdings, CashAccount cashAccount, CgtEventCollection cgtEvents)
        {
            _Handlers.Add(typeof(Aquisition), new AquisitionHandler(holdings, cashAccount));
            _Handlers.Add(typeof(Disposal), new DisposalHandler(holdings, cashAccount, cgtEvents));
            _Handlers.Add(typeof(CashTransaction), new CashTransactionHandler(cashAccount));
            _Handlers.Add(typeof(OpeningBalance), new OpeningBalanceHandler(holdings, cashAccount));
            _Handlers.Add(typeof(IncomeReceived), new IncomeReceivedHandler(holdings, cashAccount));
        }
        
        public void Apply(Transaction transaction)
        {
            if (_Handlers.TryGetValue(transaction.GetType(), out var handler))
                handler.ApplyTransaction(transaction);

            Add(transaction);
        }

        public void Apply(IEnumerable<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                if (_Handlers.TryGetValue(transaction.GetType(), out var handler))
                    handler.ApplyTransaction(transaction);

                Add(transaction);
            }
        }
    }
}
