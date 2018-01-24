using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.Transactions
{

    class DisposalSerializer : TransactionSerializer<Disposal>, ITransactionSerializer
    {
        public DisposalSerializer(string prefix, string nameSpace)
            : base(prefix, nameSpace, "disposal")
        {

        }

        protected override async Task SerializeProperties(Disposal transaction, XmlWriter xmlWriter)
        {
            await WriteProperty("units", transaction.Units, xmlWriter);
            await WriteProperty("averageprice", transaction.AveragePrice, xmlWriter);
            await WriteProperty("transactioncosts", transaction.TransactionCosts, xmlWriter);
            await WriteProperty("cgtmethod", (int)transaction.CGTMethod, xmlWriter);
            await WriteProperty("createcashtransaction", transaction.CreateCashTransaction, xmlWriter);
        }

        protected override void SetProperty(Disposal transaction, string propertyName, string propertyValue)
        {
            if (propertyName == "units")
                transaction.Units = PropertyAsInteger(propertyValue);
            else if (propertyName == "averageprice")
                transaction.AveragePrice = PropertyAsDecimal(propertyValue);
            else if (propertyName == "transactioncosts")
                transaction.TransactionCosts = PropertyAsDecimal(propertyValue);
            else if (propertyName == "cgtmethod")
                transaction.CGTMethod = (CGTCalculationMethod)PropertyAsInteger(propertyValue);
            else if (propertyName == "createcashtransaction")
                transaction.CreateCashTransaction = PropertyAsBoolean(propertyValue);
        }
    }
}
