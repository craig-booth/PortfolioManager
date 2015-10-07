using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public class CashAccount
    {      
        private List<CashAccountTransaction> _Transactions;

        public IEnumerable<CashAccountTransaction> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            return _Transactions.Where(t => (t.Date >= fromDate) && (t.Date <= toDate)).AsEnumerable();
        }

        public decimal GetBalance(DateTime atDate)
        {
            return _Transactions.Where(t => t.Date >= atDate).Sum(x => x.Amount);
        }

        public CashAccount()
        {
            _Transactions = new List<CashAccountTransaction>();
        }


        public void AddTransaction(CashAccountTransactionType type, DateTime date, string description, decimal amount)
        {
            CashAccountTransaction transaction = new CashAccountTransaction(type, date, description, amount);

            _Transactions.Add(transaction);
        }

        public void DeleteTranasction()
        {

        }

        public void ChangeTransaction()
        {

        }
    }

    public enum CashAccountTransactionType
    {
        Deposit,
        Withdrawl,
        Transfer,
        Fee,
        Interest
    }

    public class CashAccountTransaction
    {
        public Guid Id { get; set; }
        public CashAccountTransactionType Type { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public CashAccountTransaction(CashAccountTransactionType type, DateTime date, string description, decimal amount)
        {
            Type = type;
            Date = date;
            Description = description;
            Amount = amount;
        }
    }
}