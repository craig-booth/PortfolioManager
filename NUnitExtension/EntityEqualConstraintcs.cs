using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework.Constraints;

namespace NUnitExtension
{
    public class EntityEqualConstraint : Constraint
    {
        protected readonly object _Expected;
        protected readonly EntityComparer _EntityComparer;

        public EntityEqualConstraint(object expected)
            : this(expected, EntityConstraint.GetComparer(expected))
        {
        }

        public EntityEqualConstraint(object expected, EntityComparer comparer)
        {
            _Expected = expected;
            _EntityComparer = comparer;
        }


        public override bool Matches(object actual)
        {
            base.actual = actual;

            return _EntityComparer.Equals(_Expected, actual);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            _EntityComparer.Write(writer, actual);
           
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            _EntityComparer.Write(writer, _Expected);
        } 

    }
}
