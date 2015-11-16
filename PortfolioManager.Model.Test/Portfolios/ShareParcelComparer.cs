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
    public class ShareParcelComparer : IEntityComparer<ShareParcel>
    {
        public bool Equals(ShareParcel parcel1, ShareParcel parcel2)
        {
            if ((parcel1.FromDate == parcel2.FromDate) &&
                (parcel1.ToDate == parcel2.ToDate) &&
                (parcel1.Stock == parcel2.Stock) &&
                (parcel1.AquisitionDate == parcel2.AquisitionDate) &&
                (parcel1.Units == parcel2.Units) &&
                (parcel1.UnitPrice == parcel2.UnitPrice) &&
                (parcel1.CostBase == parcel2.CostBase) &&
                (parcel1.Event == parcel2.Event))
            {
                if ((parcel1.PurchaseId == Guid.Empty) && (parcel2.PurchaseId != Guid.Empty))
                    return false;
                else if ((parcel1.PurchaseId != Guid.Empty) && (parcel2.PurchaseId == Guid.Empty))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        public void Write(MessageWriter writer, ShareParcel parcel)
        {
            writer.Write("<ShareParcel:- FromDate: {0:d}, ToDate: {1:d}, Stock: {2}, AquisitionDate {3:d}, Units: {4}, UnitPrice: {5}, CostBase: {6}, Event: {7}, PurchaseId: {8}>",
                new object[] { parcel.FromDate, parcel.ToDate, parcel.Stock, parcel.AquisitionDate, parcel.Units, parcel.UnitPrice, parcel.CostBase, parcel.Event, parcel.PurchaseId });
        }
    }

}
