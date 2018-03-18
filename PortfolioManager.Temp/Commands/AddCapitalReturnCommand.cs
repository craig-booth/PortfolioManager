using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Stocks.Commands
{
    public class AddCapitalReturnCommand : ICommand
    {
        public string ASXCode { get; }
        public DateTime RecordDate { get; }
        public string Description { get; }
        public DateTime PaymentDate { get; }
        public decimal Amount { get; }

        public AddCapitalReturnCommand(string asxCode, DateTime recordDate, string description, DateTime paymentDate, decimal amount)
        {
            ASXCode = asxCode;
            RecordDate = recordDate;
            Description = description;
            PaymentDate = paymentDate;
            Amount = amount;
        }
    }
}
