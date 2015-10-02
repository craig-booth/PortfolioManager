using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios.Transactions
{

    [TestFixture, Description("Disposal of Ordinary Share - single parcel, sell all")]
    public class DisposalOrdinaryShareSingleParcelSellAll : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Disposal of Ordinary Share - single parcel, sell part")]
    public class DisposalOrdinaryShareSingleParcelSellPart : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Disposal of Ordinary Share - multiple parcels, sell all")]
    public class DisposalOrdinaryShareMultipleParcelsSellAll : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Disposal of Ordinary Share - multiple parcels, sell part")]
    public class DisposalOrdinaryShareMultipleParcelsSellPart : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Disposal of Stapled Security - single parcel, sell all")]
    public class DisposalStapledSecuritySingleParcelSellAll : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Disposal of Stapled Security - single parcel, sell part")]
    public class DisposalStapledSecuritySingleParcelSellPart : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Disposal of Stapled Security - multiple parcels, sell all")]
    public class DisposalStapledSecurityMultipleParcelsSellAll : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Disposal of Stapled Security - multiple parcels, sell part")]
    public class DisposalStapledSecurityMultipleParcelsSellPart : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Disposal validation tests")]
    public class DisposalValidationTests : TransactionTest
    {
        [Test, Description("Disposal of Ordinary Share - no parcels")]
        [ExpectedException(typeof(NoParcelsForTransaction))]
        public void NoParcelsForTransaction()
        {
            throw new NotSupportedException();
        }

        [Test, Description("Disposal of Ordinary Share - single parcel, not enough shares")]
        public void SingleParcelNotEnoughShares()
        {
            throw new NotSupportedException();
        }

        [Test, Description("Disposal of Ordinary Share - multiple parcels, not enough shares")]
        public void MultipleParcelsNotEnoughShares()
        {
            throw new NotSupportedException();
        }
    }
}
