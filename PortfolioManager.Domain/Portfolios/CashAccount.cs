using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Portfolios
{
    public class CashAccount
    {
        private Transaction _LastTransaction;

        private List<Transaction> _Transactions = new List<Transaction>();

        public IEnumerable<Transaction> Transactions(DateRange dateRange)
        {
            return _Transactions.Where(x => dateRange.Contains(x.Date));
        }

        public decimal Balance
        {
            get
            {
                if (_LastTransaction != null)
                    return _LastTransaction.Balance;
                else
                    return 0.00m;
            }
        }

        public decimal this[DateTime date]
        {
            get
            {
                var transaction = _Transactions.LastOrDefault(x => x.Date <= date);
                if (transaction != null)
                    return transaction.Balance;
                else
                    return 0.00m;
            }
        }

        public void Deposit(DateTime date, decimal amount, string description)
        {
            AddTransaction(date, amount, description, BankAccountTransactionType.Deposit);
        }

        public void Withdraw(DateTime date, decimal amount, string description)
        {
            AddTransaction(date, -amount, description, BankAccountTransactionType.Deposit);
        }

        public void Transfer(DateTime date, decimal amount, string description)
        {
            AddTransaction(date, -amount, description, BankAccountTransactionType.Transfer);
        }

        public void FeeDeducted(DateTime date, decimal amount, string description)
        {
            AddTransaction(date, -amount, description, BankAccountTransactionType.Fee);
        }

        public void InterestPaid(DateTime date, decimal amount, string description)
        { 
            AddTransaction(date, amount, description, BankAccountTransactionType.Interest);
        }

        public void AddTransaction(DateTime date, decimal amount, string description, BankAccountTransactionType type)
        {
            if ((_LastTransaction != null) && (_LastTransaction.Date > date))
                throw new Exception("Transactions already after this date");

            if ((type == BankAccountTransactionType.Interest) && (description == ""))
                description = "Interest";

            _LastTransaction = new Transaction(date, description, amount, type, Balance + amount);
            _Transactions.Add(_LastTransaction);
        }

        public class Transaction
        {
            public readonly DateTime Date;
            public readonly string Description;
            public readonly decimal Amount;
            public readonly BankAccountTransactionType Type;
            public readonly decimal Balance;

            public Transaction(DateTime date, string description, decimal amount, BankAccountTransactionType type, decimal balance)
            {
                Date = date;
                Description = description;
                Amount = amount;
                Type = type;
                Balance = balance;
            }
        }

    }
}
