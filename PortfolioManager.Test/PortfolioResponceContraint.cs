using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

using NUnit.Framework;
using NUnit.Framework.Constraints;
using FluentAssertions;
using FluentAssertions.Equivalency;


namespace PortfolioManager.Test
{ 
    public class PortfolioResponceContraint : Constraint
    {
        public object Expected;

        public PortfolioResponceContraint(object expected)
        {
            Expected = expected;
        }

        public PortfolioResponceContraint(Type type, string fileName)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var serializer = new XmlSerializer(type);
                Expected = serializer.Deserialize(streamReader);
            }
        }

        public PortfolioResponceContraint(Type type, string fileName, Type[] extraTypes)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var serializer = new XmlSerializer(type, extraTypes);
                Expected = serializer.Deserialize(streamReader);
            }
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var result = new ConstraintResult(this, actual);

            actual.Should().BeEquivalentTo(Expected, options => options
                       .RespectingRuntimeTypes()
                       );

            return new ConstraintResult(this, actual, true);
        }

    }

    public class Is : NUnit.Framework.Is
    {
        public static PortfolioResponceContraint EquivalentTo(object expected)
        {
            return new PortfolioResponceContraint(expected);
        }

        public static PortfolioResponceContraint EquivalentTo(Type type, string fileName)
        {
            return new PortfolioResponceContraint(type, fileName);
        }

        public static PortfolioResponceContraint EquivalentTo(Type type, string fileName, Type[] extraTypes)
        {
            return new PortfolioResponceContraint(type, fileName, extraTypes);
        }
    }

    public static class PortfolioResponceContraintExtensions
    {

        public static PortfolioResponceContraint EquivalentTo(this ConstraintExpression expression, Type type, string fileName)
        {
            var constraint = new PortfolioResponceContraint(type, fileName);
            expression.Append(constraint);
            return constraint;
        }
        public static PortfolioResponceContraint EquivalentTo(this ConstraintExpression expression, Type type, string fileName, Type[] extraTypes)
        {
            var constraint = new PortfolioResponceContraint(type, fileName, extraTypes);
            expression.Append(constraint);
            return constraint;
        }
    }   



}
