using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;


namespace PortfolioManager.Service.Transactions
{
    class CashTransactionSerializer : TransactionSerializer<CashTransaction>, ITransactionSerializer
    {
        public CashTransactionSerializer(string prefix, string nameSpace)
            : base(prefix, nameSpace, "cashtransaction")
        {

        }

        protected override async Task SerializeProperties(CashTransaction transaction, XmlWriter xmlWriter)
        {
            await WriteProperty("cashtransactiontype", (int)transaction.CashTransactionType, xmlWriter);
            await WriteProperty("amount", transaction.Amount, xmlWriter);
        }

        protected override void SetProperty(CashTransaction transaction, string propertyName, string propertyValue)
        {
            if (propertyName == "cashtransactiontype")
                transaction.CashTransactionType = (BankAccountTransactionType)PropertyAsInteger(propertyValue);
            else if (propertyName == "amount")
                transaction.Amount = PropertyAsDecimal(propertyValue);
        }
    }
}
