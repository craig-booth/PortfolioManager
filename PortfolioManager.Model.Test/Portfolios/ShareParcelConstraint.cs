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
            base.actual = actual;
            bool found;

            if (actual is IEnumerable<ShareParcel>)
            {
                List<ShareParcel> expectedParcels = _Expected.ToList();
                IEnumerable<ShareParcel> actualParcels = actual as IEnumerable<ShareParcel>;

                foreach (ShareParcel actualParcel in actualParcels)
                {
                    found = false;
                    foreach (ShareParcel expectedParcel in expectedParcels)
                    {
                        if (_EntityComparer.Equals(expectedParcel, actualParcel))
                        {
                            if (expectedParcel.Parent != Guid.Empty)
                            {
                                // Check that parent are equivalent
                                var expectedParent = _Expected.First(x => x.Id == expectedParcel.Parent);
                                var actualParent = actualParcels.First(x => x.Id == actualParcel.Parent);

                                if (!_EntityComparer.Equals(expectedParent, actualParent))
                                    return false;
                            }

                            expectedParcels.Remove(expectedParcel);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        return false;
                }

                if (expectedParcels.Count > 0)
                    return false;

                return true;
            }

            return false;
        }

    }

}
