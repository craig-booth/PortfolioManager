using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.Transactions
{

    class OpeningBalanceSerializer : TransactionSerializer<OpeningBalance>, ITransactionSerializer
    {
        public OpeningBalanceSerializer(string prefix, string nameSpace)
            : base(prefix, nameSpace, "openingbalance")
        {

        }

        protected override void SerializeProperties(OpeningBalance transaction, XmlWriter xmlWriter)
        {
            WriteProperty("units", transaction.Units, xmlWriter);
            WriteProperty("costbase", transaction.CostBase, xmlWriter);
            WriteProperty("aquisitiondate", transaction.AquisitionDate, xmlWriter);
            WriteProperty("purchaseid", transaction.PurchaseId, xmlWriter);
        }

        protected override void SetProperty(OpeningBalance transaction, string propertyName, string propertyValue)
        {
            if (propertyName == "units")
                transaction.Units = PropertyAsInteger(propertyValue);
            else if (propertyName == "costbase")
                transaction.CostBase = PropertyAsDecimal(propertyValue);
            else if (propertyName == "aquisitiondate")
                transaction.AquisitionDate = PropertyAsDateTime(propertyValue);
            else if (propertyName == "purchaseid")
                transaction.PurchaseId = PropertyAsGuid(propertyValue);
        }
    }
}
