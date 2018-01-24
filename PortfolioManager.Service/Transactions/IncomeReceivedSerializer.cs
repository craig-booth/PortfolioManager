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

        protected override void SerializeProperties(IncomeReceived transaction, XmlWriter xmlWriter)
        {
            WriteProperty("franked", transaction.FrankedAmount, xmlWriter);
            WriteProperty("unfranked", transaction.UnfrankedAmount, xmlWriter);
            WriteProperty("frankingcredits", transaction.FrankingCredits, xmlWriter);
            WriteProperty("interest", transaction.Interest, xmlWriter);
            WriteProperty("taxdeferred", transaction.TaxDeferred, xmlWriter);
            WriteProperty("createcashtransaction", transaction.CreateCashTransaction, xmlWriter);
            WriteProperty("drpcashbalance", transaction.DRPCashBalance, xmlWriter);
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
