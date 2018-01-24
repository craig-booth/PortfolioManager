using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.Transactions
{

    class IncomeReceivedSerializer : TransactionSerializer<IncomeReceived>, ITransactionSerializer
    {
        public IncomeReceivedSerializer(string prefix, string nameSpace)
            : base(prefix, nameSpace, "incomereceived")
        {

        }

        protected override async Task SerializeProperties(IncomeReceived transaction, XmlWriter xmlWriter)
        {
            await WriteProperty("franked", transaction.FrankedAmount, xmlWriter);
            await WriteProperty("unfranked", transaction.UnfrankedAmount, xmlWriter);
            await WriteProperty("frankingcredits", transaction.FrankingCredits, xmlWriter);
            await WriteProperty("interest", transaction.Interest, xmlWriter);
            await WriteProperty("taxdeferred", transaction.TaxDeferred, xmlWriter);
            await WriteProperty("createcashtransaction", transaction.CreateCashTransaction, xmlWriter);
            await WriteProperty("drpcashbalance", transaction.DRPCashBalance, xmlWriter);
        }

        protected override void SetProperty(IncomeReceived transaction, string propertyName, string propertyValue)
        {
            if (propertyName == "franked")
                transaction.FrankedAmount = PropertyAsDecimal(propertyValue);
            else if (propertyName == "unfranked")
                transaction.UnfrankedAmount = PropertyAsDecimal(propertyValue);
            else if (propertyName == "frankingcredits")
                transaction.FrankingCredits = PropertyAsDecimal(propertyValue);
            else if (propertyName == "interest")
                transaction.Interest = PropertyAsDecimal(propertyValue);
            else if (propertyName == "taxdeferred")
                transaction.TaxDeferred = PropertyAsDecimal(propertyValue);
            else if (propertyName == "createcashtransaction")
                transaction.CreateCashTransaction = PropertyAsBoolean(propertyValue);
            else if (propertyName == "drpcashbalance")
                transaction.DRPCashBalance = PropertyAsDecimal(propertyValue);
        }
    }
}
