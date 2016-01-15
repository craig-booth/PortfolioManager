using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Test.Utils
{
    [TestFixture]
    class MathUtilsTest
    {
        [Test, Description("ApportionAmmount into thirds")]
        public void ApportionAmountThirds()
        {
            ApportionedCurrencyValue[] values = new ApportionedCurrencyValue[3];

            values[0].Units = 50;
            values[1].Units = 50;
            values[2].Units = 50;

            MathUtils.ApportionAmount(100.00M, values);

            Assert.That(values[0].Amount, Is.EqualTo(33.33M));
            Assert.That(values[1].Amount, Is.EqualTo(33.34M));
            Assert.That(values[2].Amount, Is.EqualTo(33.33M));
        }

        [Test, Description("ApportionAmmount single value")]
        public void ApportionAmountSingle()
        {
            ApportionedCurrencyValue[] values = new ApportionedCurrencyValue[1];

            values[0].Units = 50;

            MathUtils.ApportionAmount(100.00M, values);

            Assert.That(values[0].Amount, Is.EqualTo(100.00M));
        }

        [Test, Description("ApportionAmmount different values")]
        public void ApportionAmountComplex()
        {
            ApportionedCurrencyValue[] values = new ApportionedCurrencyValue[5];

            values[0].Units = 11;
            values[1].Units = 42;
            values[2].Units = 24;
            values[3].Units = 31;
            values[4].Units = 57;

            MathUtils.ApportionAmount(276.21M, values);

            Assert.That(values[0].Amount, Is.EqualTo(18.41M));
            Assert.That(values[1].Amount, Is.EqualTo(70.31M));
            Assert.That(values[2].Amount, Is.EqualTo(40.18M));
            Assert.That(values[3].Amount, Is.EqualTo(51.89M));
            Assert.That(values[4].Amount, Is.EqualTo(95.42M));
        }
    }
}
