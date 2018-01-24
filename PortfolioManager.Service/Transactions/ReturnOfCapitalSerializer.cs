using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.Transactions
{

    class ReturnOfCapitalSerializer : TransactionSerializer<ReturnOfCapital>, ITransactionSerializer
    {
        public ReturnOfCapitalSerializer(string prefix, string nameSpace)
            : base(prefix, nameSpace, "returnofcapital")
        {

        }

        protected override async Task SerializeProperties(ReturnOfCapital transaction, XmlWriter xmlWriter)
        {
            await WriteProperty("amount", transaction.Amount, xmlWriter);
            await WriteProperty("createcashtransaction", transaction.CreateCashTransaction, xmlWriter);
        }

        protected override void SetProperty(ReturnOfCapital transaction, string propertyName, string propertyValue)
        {
            if (propertyName == "amount")
                transaction.Amount = PropertyAsInteger(propertyValue);
            else if (propertyName == "createcashtransaction")
                transaction.CreateCashTransaction = PropertyAsBoolean(propertyValue);
        }
    }
}
