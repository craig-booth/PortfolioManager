using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnitExtension;

using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios
{
    public class ShareParcelComparer : EntityComparer
    {

        public ShareParcelComparer()
            : base("Id,PurchaseId")
        {

        }

        public override bool Equal(object expected, object actual)
        {
            if (base.Equal(expected, actual))
            {
                var expectedParcel = expected as ShareParcel;
                var actualParcel = actual as ShareParcel;

                if ((expectedParcel.PurchaseId == Guid.Empty) && (actualParcel.PurchaseId != Guid.Empty))
                    return false;
                else if ((expectedParcel.PurchaseId != Guid.Empty) && (actualParcel.PurchaseId == Guid.Empty))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
    }
}
