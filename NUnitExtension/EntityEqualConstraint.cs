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
        protected readonly EntityWriter _EntityWriter;

        public EntityEqualConstraint(object expected)
            : this(expected, EntityConstraint.GetComparer(expected), EntityConstraint.GetWriter(expected))
        {
        }

        public EntityEqualConstraint(object expected, EntityComparer comparer, EntityWriter writer)
        {
            _Expected = expected;
            _EntityComparer = comparer;
            _EntityWriter = writer;
        }


        public override bool Matches(object actual)
        {
            base.actual = actual;

            return _EntityComparer.Equal(_Expected, actual);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            _EntityWriter.Write(writer, actual);            
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            _EntityWriter.Write(writer, _Expected);
        }

    }
}
