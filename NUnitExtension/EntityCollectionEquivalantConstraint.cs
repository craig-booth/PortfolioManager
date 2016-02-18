using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework.Constraints;

namespace NUnitExtension
{

    public class EntityCollectionEquivalantConstraint : Constraint
    {
        protected readonly IEnumerable _Expected;

        public EntityCollectionEquivalantConstraint(IEnumerable expected)
        {
            _Expected = expected;
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;

            var actualEntities = actual as IEnumerable;

            if (actualEntities == null)
                return false;
            base.actual = actual;
            bool found;

            var expectedEntities = _Expected.Cast<object>().ToList();

            foreach (var actualEntity in actualEntities)
            {
                found = false;
                foreach (var expectedEntity in _Expected)
                {
                    var comparer = EntityConstraint.GetComparer(expectedEntity);

                    if (comparer.Equals(expectedEntity, actualEntity))
                    {
                        expectedEntities.Remove(expectedEntity);
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return false;
            }

            if (expectedEntities.Count > 0)
                return false;

            return true;

        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            WriteValue(writer, actual);

        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            WriteValue(writer, _Expected);
        }

        private void WriteValue(MessageWriter writer, object entity)
        {
            var collection = entity as ICollection;

            if (collection == null)
                return;

            writer.Write("<");

            foreach (var obj in collection)
            {
                var comparer = EntityConstraint.GetComparer(obj);

                comparer.Write(writer, obj);
            }

            writer.Write(">");
            writer.Write(writer.NewLine);
        }

    }
}