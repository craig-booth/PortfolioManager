﻿using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework.Constraints;

namespace NUnitExtension
{

    public class EntityCollectionEqualConstraint : Constraint
    {
        protected readonly IEnumerable _Expected;

        public EntityCollectionEqualConstraint(IEnumerable expected)
        {
            _Expected = expected;
        }

        public override bool Matches(object actual)
        {

            /* Enumerable.SequenceEqual(list1.OrderBy(t => t), list2.OrderBy(t => t))

    */

            base.actual = actual;

            var actualCollection = actual as IEnumerable;

            if (actualCollection == null)
                return false;

            var expectedEnumerator = _Expected.GetEnumerator();
            var actualEnumerator = actualCollection.GetEnumerator();

            while (expectedEnumerator.MoveNext())
            {
                if (actualEnumerator.MoveNext())
                {

                    var comparer = EntityConstraint.GetComparer(expectedEnumerator.Current);

                    if (!comparer.Equal(expectedEnumerator.Current, actualEnumerator.Current))
                        return false;
                }
                else
                    return false;
            }

            if (actualEnumerator.MoveNext())
                return false;
            else
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
            var collection = entity as IEnumerable;

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