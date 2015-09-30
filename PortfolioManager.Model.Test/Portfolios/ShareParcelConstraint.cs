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

 /*   public class ShareParcelCollectionEqualConstraint : EntityCollectionEqualConstraint<ShareParcel>
    {

        public ShareParcelCollectionEqualConstraint(ICollection<ShareParcel> expected, IEqualityComparer<ShareParcel> comparer, IEntityWriter<ShareParcel> writer)
            : base(expected, comparer, writer)
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

    } */

    public interface IEntityComparer<T>
    {
        List<PropertyDifference> FindDifferences(T entity1, T entity2);
    }

    public class ShareParcelComparer : IEqualityComparer<ShareParcel>
    {
        public bool Equals(ShareParcel parcel1, ShareParcel parcel2)
        {
            return parcel1.FromDate == parcel2.FromDate &&
                   parcel1.ToDate == parcel2.ToDate &&
                   parcel1.Stock == parcel2.Stock &&
                   parcel1.AquisitionDate == parcel2.AquisitionDate &&
                   parcel1.Units == parcel2.Units &&
                   parcel1.UnitPrice == parcel2.UnitPrice &&
                   parcel1.CostBase == parcel2.CostBase &&
                   parcel1.Event == parcel2.Event;
        }

        public int GetHashCode(ShareParcel parcel)
        {
            return parcel.Id.GetHashCode();
        }
    }

    public class ShareParcelWriter : IEntityWriter<ShareParcel>
    {
        public void Write(MessageWriter writer, ShareParcel parcel)
        {
            writer.Write("<ShareParcel:- FromDate: {0:d}, ToDate: {1:d}, Stock: {2}, AquisitionDate {3:d}, Units: {4}, UnitPrice: {5}, CostBase: {6}, Event: {7}, Parent: {8}>",
                new object[] { parcel.FromDate, parcel.ToDate, parcel.Stock, parcel.AquisitionDate, parcel.Units, parcel.UnitPrice, parcel.CostBase, parcel.Event, parcel.Parent });
        }
    }

}
