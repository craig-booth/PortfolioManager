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

using PortfolioManager.Service.Interface;

namespace PortfolioManager.Test
{ 
    public class PortfolioResponceContraint : Constraint
    {
        private ServiceResponce Expected;

        public PortfolioResponceContraint(ServiceResponce expected)
        {
            Expected = expected;
        }

        public PortfolioResponceContraint(Type type, string fileName)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var serializer = new XmlSerializer(type, new Type[]
                        {
                            typeof(AquisitionTransactionItem),
                            typeof(CashTransactionItem),
                            typeof(CostBaseAdjustmentTransactionItem),
                            typeof(DisposalTransactionItem),
                            typeof(IncomeTransactionItem),
                            typeof(OpeningBalanceTransactionItem),
                            typeof(ReturnOfCapitalTransactionItem),
                            typeof(UnitCountAdjustmentTransactionItem)
                        });

                Expected = (ServiceResponce)serializer.Deserialize(streamReader);
            }
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var result = new ConstraintResult(this, actual);

            var actualResponce = actual as ServiceResponce;

            actualResponce.ShouldBeEquivalentTo(Expected, options => options
                       .RespectingRuntimeTypes()
                       .Excluding(x => x.ResponceTime)
                       ); 
                         
            return new ConstraintResult(this, actual, true);
        }

    }

    public class Is : NUnit.Framework.Is
    {
        public static PortfolioResponceContraint EquivalentTo(ServiceResponce expected)
        {
            return new PortfolioResponceContraint(expected);
        }

        public static PortfolioResponceContraint EquivalentTo(Type type, string fileName)
        {
            return new PortfolioResponceContraint(type, fileName);
        }
    }

    public static class PortfolioResponceContraintExtensions
    {
        public static PortfolioResponceContraint EquivalentTo(this ConstraintExpression expression, ServiceResponce expected)
        {
            var constraint = new PortfolioResponceContraint(expected);
            expression.Append(constraint);
            return constraint;
        }

        public static PortfolioResponceContraint EquivalentTo(this ConstraintExpression expression, Type type, string fileName)
        {
            var constraint = new PortfolioResponceContraint(type, fileName);
            expression.Append(constraint);
            return constraint;
        }
    }   



}
