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
            return _Transactions.SkipWhile(x => x.Date < dateRange.FromDate).TakeWhile(x => x.Date <= dateRange.ToDate);
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
                // No transactions
                if (_Transactions.Count == 0)
                    return 0.00m;

                // On or after last transaction
                if ((_LastTransaction != null) && (date >= _LastTransaction.Date))
                    return _LastTransaction.Balance;

                // Otherwise search for transaction
                var dummyTransaction = new Transaction(date, "", 0.00m, BankAccountTransactionType.Deposit, 0.00m);
                var index = _Transactions.BinarySearch(dummyTransaction);
                if (index < 0)
                {
                    index = ~index;

                    while (_Transactions[index].Date > date)
                    {
                        index--;

                        if (index < 0)
                            return 0.00m;
                    }
                }
                else
                {
                    while (_Transactions[index + 1].Date == date)
                        index++;
                }

                return _Transactions[index].Balance;
            }
        }

        public IEnumerable<EffectiveBalance> EffectiveBalances(DateRange dateRange)
        {
            var transactions = Transactions(dateRange);

            var fromDate = dateRange.FromDate;
            var toDate = DateUtils.NoEndDate;
            var balance = 0.00m;

            foreach (var transaction in transactions)
            {
                if (fromDate == transaction.Date)
                {
                    balance = transaction.Balance;
                }
                else
                {
                    toDate = transaction.Date.AddDays(-1);

                    yield return new EffectiveBalance(fromDate, toDate, balance);

                    fromDate = transaction.Date;
                    toDate = transaction.Date;
                    balance = transaction.Balance;
                }
            }

            yield return new EffectiveBalance(fromDate, dateRange.ToDate, balance);
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
            AddTransaction(date, amount, description, BankAccountTransactionType.Transfer);
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
            if ((type == BankAccountTransactionType.Interest) && (description == ""))
                description = "Interest";

            if ((_LastTransaction == null) || (date >= _LastTransaction.Date))
            {
                _LastTransaction = new Transaction(date, description, amount, type, Balance + amount);
                _Transactions.Add(_LastTransaction);
            }
            else
            {
                var newTransaction = new Transaction(date, description, amount, type, 0.00m);

                var index = _Transactions.BinarySearch(newTransaction);
                if (index < 0)
                {
                    index = ~index;
                }
                else
                {
                    while (_Transactions[index].Date == date)
                        index++;
                }

                newTransaction.Balance = (_Transactions[index].Balance - _Transactions[index].Amount) + amount;
                _Transactions.Insert(index, newTransaction);

                for (var i = index + 1; i < _Transactions.Count; i++)
                    _Transactions[i].Balance += amount;
            }
        }

        public class Transaction : IComparable<Transaction>
        {
            public readonly DateTime Date;
            public readonly string Description;
            public readonly decimal Amount;
            public readonly BankAccountTransactionType Type;
            public decimal Balance;

            public Transaction(DateTime date, string description, decimal amount, BankAccountTransactionType type, decimal balance)
            {
                Date = date;
                Description = description;
                Amount = amount;
                Type = type;
                Balance = balance;
            }

            public int CompareTo(Transaction other)
            {
                return Date.CompareTo(other.Date);
            }
        }


        public class EffectiveBalance
        {
            public DateRange EffectivePeriod { get; set; }
            public decimal Balance { get; set; }

            public EffectiveBalance(DateTime fromDate, DateTime toDate, decimal balance)
            {
                EffectivePeriod = new DateRange(fromDate, toDate);
                Balance = balance;
            }
        }
    }
}