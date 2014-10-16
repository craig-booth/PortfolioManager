using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Stocks
{
    public class CapitalReturn : ICorporateAction
    {
        private IStockDatabase _Database;

        public Guid Id { get; private set; }
        public Guid Stock { get; private set; }
        public DateTime ActionDate { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }

        public CapitalReturn(IStockDatabase stockDatabase, Guid stock, DateTime actionDate, DateTime paymentDate, decimal amount, string description)
            : this(stockDatabase, Guid.NewGuid(), stock, actionDate, paymentDate, amount, description)
        {
        }

        public CapitalReturn(IStockDatabase stockDatabase, Guid id, Guid stock, DateTime actionDate, DateTime paymentDate, decimal amount, string description)
        {
            _Database = stockDatabase;
            Id = id;
            ActionDate = actionDate;
            Stock = stock;
            PaymentDate = paymentDate;
            Amount = amount;
            if (description != "")
                Description = description;
            else
                Description = "Capital Return " + Amount.ToString("c");
        }

        public void Change(DateTime newActionDate, DateTime newPaymentDate, decimal newAmount, string newDescription)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                ActionDate = newActionDate;
                PaymentDate = newPaymentDate;
                Amount = newAmount;
                Description = newDescription;

                unitOfWork.CorporateActionRepository.Update(this);

                unitOfWork.Save();
            }
        }
    }
}
