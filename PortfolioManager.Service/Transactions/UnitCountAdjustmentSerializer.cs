using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.Transactions
{
    class UnitCountAdjustmentSerializer : TransactionSerializer<UnitCountAdjustment>, ITransactionSerializer
    {
        public UnitCountAdjustmentSerializer(string prefix, string nameSpace)
            : base(prefix, nameSpace, "unitcountadjustment")
        {

        }

        protected override async Task SerializeProperties(UnitCountAdjustment transaction, XmlWriter xmlWriter)
        {
            await WriteProperty("originalunits", transaction.OriginalUnits, xmlWriter);
            await WriteProperty("newunits", transaction.NewUnits, xmlWriter);
        }

        protected override void SetProperty(UnitCountAdjustment transaction, string propertyName, string propertyValue)
        {
            if (propertyName == "originalunits")
                transaction.OriginalUnits = PropertyAsInteger(propertyValue);
            else if (propertyName == "newunits")
                transaction.NewUnits = PropertyAsInteger(propertyValue);
        }
    }
}
