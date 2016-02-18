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
            /* // lists should have same count of items, and set difference must be empty
var areEquivalent = (list1.Count == list2.Count) && !list1.Except(list2).Any(); 

    */

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

                    if (comparer.Equal(expectedEntity, actualEntity))
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
                var entityWriter = EntityConstraint.GetWriter(obj);

                entityWriter.Write(writer, obj);
            }

            writer.Write(">");
            writer.Write(writer.NewLine);
        }

    }
}