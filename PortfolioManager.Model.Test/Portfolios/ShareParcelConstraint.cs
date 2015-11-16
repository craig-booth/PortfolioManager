using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework.Constraints;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios
{

    public class ShareParcelCollectionEqualConstraint : EntityCollectionEqualConstraint<ShareParcel>
    {

        public ShareParcelCollectionEqualConstraint(ICollection<ShareParcel> expected)
            : base(expected, new ShareParcelComparer())
        {
        }

        public override bool Matches(object actual)
        {
            if (!base.Matches(actual))
                return false;

            return true;
        }
    }

}
