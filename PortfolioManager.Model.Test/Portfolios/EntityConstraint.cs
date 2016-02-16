using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework.Constraints;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios
{

    public interface IEntityComparer<T>
    {
        bool Equals(T expected, T actual);
        void Write(MessageWriter writer, T entity);
    }

    public class EntityEqualConstraint<T> : Constraint
    {
        protected readonly T _Expected;
        protected readonly IEntityComparer<T> _EntityComparer;

        public EntityEqualConstraint(T expected, IEntityComparer<T> comparer)
        {
            _Expected = expected;
            _EntityComparer = comparer;
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;

            if (actual is T)
                return _EntityComparer.Equals(_Expected, (T)actual);
            else
                return false;
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            if (actual is T)
                _EntityComparer.Write(writer, (T)actual);
            else
                writer.WriteActualValue(actual);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            _EntityComparer.Write(writer, (T)_Expected);
        }

    }

    public abstract class EntityCollectionConstraint<T> : Constraint
    {
        protected readonly ICollection<T> _Expected;
        protected readonly IEntityComparer<T> _EntityComparer;

        public EntityCollectionConstraint(ICollection<T> expected, IEntityComparer<T> comparer)
        {
            _Expected = expected;
            _EntityComparer = comparer;
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            WriteValue(writer, _Expected);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            if (actual is IEnumerable<T>)
                WriteValue(writer, actual as IEnumerable<T>);
            else
                writer.WriteActualValue(actual);
        }

        private void WriteValue(MessageWriter writer, IEnumerable<T> entities)
        {
            int count = 0;

            writer.Write("<");
            foreach (T entity in entities)
            {
                if (count > 0)
                    writer.Write(",\n              ");

                _EntityComparer.Write(writer, entity);

                count++;
            }
            writer.Write(">");
        }
    }


    public class EntityCollectionEqualConstraint<T> : EntityCollectionConstraint<T>
    {

        public EntityCollectionEqualConstraint(ICollection<T> expected, IEntityComparer<T> comparer)
            : base(expected, comparer)
        {
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;

            if (actual is IEnumerable<T>)
            {
                IEnumerable<T> actualEntities = actual as IEnumerable<T>;
                var expectedEnumerator = _Expected.GetEnumerator();


                foreach (T actualEntity in actualEntities)
                {
                    if (expectedEnumerator.MoveNext())
                    {
                        if (!_EntityComparer.Equals(expectedEnumerator.Current, actualEntity))
                            return false;
                    }
                    else
                        return false;
                }
                return true;
            }

            return false;
        }
    }


    public class EntityCollectionEquivalentConstraint<T> : EntityCollectionConstraint<T>
    {
     
        public EntityCollectionEquivalentConstraint(ICollection<T> expected, IEntityComparer<T> comparer)
            : base(expected, comparer)
        {
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;
            bool found;

            if (actual is IEnumerable<T>)
            {
                List<T> expectedEntities = _Expected.ToList();
                IEnumerable<T> actualEntities = actual as IEnumerable<T>;

                foreach (T actualEntity in actualEntities)
                {
                    found = false;
                    foreach (T expectedEntity in expectedEntities)
                    {
                        if (_EntityComparer.Equals(expectedEntity, actualEntity))
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

            return false;
        }

    }


}
