using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using NUnit.Framework.Constraints;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios
{

    public static class PortfolioConstraint
    {
        public static EntityEqualConstraint<ShareParcel> Equals(ShareParcel expected)
        {
            return new EntityEqualConstraint<ShareParcel>(expected, new ShareParcelWriter());
        }
        
   /*     public static ShareParcelCollectionEqualConstraint Equals(ICollection<ShareParcel> expected)
        {
            return new ShareParcelCollectionEqualConstraint(expected, new ShareParcelComparer(), new ShareParcelWriter());
        } */

        public static EntityCollectionEqualConstraint<ShareParcel> Equals(ICollection<ShareParcel> expected)
        {
            return new EntityCollectionEqualConstraint<ShareParcel>(expected, new ShareParcelWriter());
        }

        public static EntityEqualConstraint<IncomeReceived> Equals(IncomeReceived expected)
        {
            return new EntityEqualConstraint<IncomeReceived>(expected, new IncomeReceviedWriter());
        }

        public static EntityCollectionEqualConstraint<IncomeReceived> Equals(ICollection<IncomeReceived> expected)
        {
            return new EntityCollectionEqualConstraint<IncomeReceived>(expected, new IncomeReceviedWriter());
        }

        public static EntityEqualConstraint<CGTEvent> Equals(CGTEvent expected)
        {
            return new EntityEqualConstraint<CGTEvent>(expected, new CGTEventWriter());
        }

        public static EntityCollectionEqualConstraint<CGTEvent> Equals(ICollection<CGTEvent> expected)
        {
            return new EntityCollectionEqualConstraint<CGTEvent>(expected, new CGTEventWriter());
        }
    }


}
