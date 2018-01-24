using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.Transactions
{
    class CostBaseAdjustmentSerializer : TransactionSerializer<CostBaseAdjustment>, ITransactionSerializer
    {
        public CostBaseAdjustmentSerializer(string prefix, string nameSpace)
            : base(prefix, nameSpace, "costbaseadjustment")
        {

        }

        protected override void SerializeProperties(CostBaseAdjustment transaction, XmlWriter xmlWriter)
        {
            WriteProperty("percentage", transaction.Percentage, xmlWriter);
        }

        protected override void SetProperty(CostBaseAdjustment transaction, string propertyName, string propertyValue)
        {
            if (propertyName == "percentage")
                transaction.Percentage = PropertyAsDecimal(propertyValue);
        }
    }
}
