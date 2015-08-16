using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using NUnit.Framework.Constraints;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios
{

    public static class PortfolioConstraint
    {
        public static ShareParcelEqual Equals(ShareParcel expected)
        {
            return new ShareParcelEqual(expected);
        }

        public static ShareParcelCollectionEqual Equals(ICollection<ShareParcel> expected)
        {
            return new ShareParcelCollectionEqual(expected);
        }

    }

    public class ShareParcelEqual : Constraint
    {
        private readonly ShareParcel _Expected;
        private readonly ShareParcelComparer _Comparer;

        public ShareParcelEqual(ShareParcel expected)
        {
            _Expected = expected;
            _Comparer = new ShareParcelComparer();
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;
        
            if (actual is ShareParcel)
                return _Comparer.Equals(_Expected, (ShareParcel)actual);
            else
                return false;
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            if (actual is ShareParcel)
                ShareParcelWriter.Write(writer, (ShareParcel)actual);
            else
                writer.WriteActualValue(actual);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            ShareParcelWriter.Write(writer, (ShareParcel)_Expected);
        }

    }

    public class ShareParcelCollectionEqual : Constraint 
    {
        private readonly ICollection<ShareParcel> _Expected;
        private readonly ShareParcelComparer _Comparer;

        public ShareParcelCollectionEqual(ICollection<ShareParcel> expected)
            :  base(expected)
        {
            _Expected = expected;
            _Comparer = new ShareParcelComparer();
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
                        if (_Comparer.Equals(expectedParcel, actualParcel))
                        {
                            if (expectedParcel.Parent != Guid.Empty)
                            {
                                // Check that parent are equivalent
                                var expectedParent = _Expected.First(x => x.Id == expectedParcel.Parent);
                                var actualParent = actualParcels.First(x => x.Id == actualParcel.Parent);

                                if (! _Comparer.Equals(expectedParent, actualParent))
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


        public override void WriteDescriptionTo(MessageWriter writer)
        {
            WriteValue(writer, _Expected);            
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            if (actual is IEnumerable<ShareParcel>)
                WriteValue(writer, actual as IEnumerable<ShareParcel>);
            else
                writer.WriteActualValue(actual);
        }

        private void WriteValue(MessageWriter writer, IEnumerable<ShareParcel> parcels)
        {
            int count = 0;

            writer.Write("<");
            foreach (ShareParcel parcel in parcels)
            {
                if (count > 0)
                    writer.Write(",\n              ");

                ShareParcelWriter.Write(writer, parcel);

                count++;
            }
            writer.Write(">");
        }
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

    public static class ShareParcelWriter
    {
        public static void Write(MessageWriter writer, ShareParcel parcel)
        {
            writer.Write("<ShareParcel:- FromDate: {0:d}, ToDate: {1:d}, Stock: {2}, AquisitionDate {3:d}, Units: {4}, UnitPrice: {5}, CostBase: {6}, Event: {7}, Parent: {8}>", 
                new object[] {parcel.FromDate, parcel.ToDate, parcel.Stock, parcel.AquisitionDate, parcel.Units, parcel.UnitPrice, parcel.CostBase, parcel.Event, parcel.Parent}); 
        }
    }

}
