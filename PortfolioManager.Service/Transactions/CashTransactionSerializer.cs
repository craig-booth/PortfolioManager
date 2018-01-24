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

        protected override void SerializeProperties(CashTransaction transaction, XmlWriter xmlWriter)
        {
            WriteProperty("cashtransactiontype", (int)transaction.CashTransactionType, xmlWriter);
            WriteProperty("amount", transaction.Amount, xmlWriter);
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
