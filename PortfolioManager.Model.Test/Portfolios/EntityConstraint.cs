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
    public interface IEntityWriter<T>
                where T : IEntity
    {
        void Write(MessageWriter writer, T entity);
    }

    public class EntityEqualConstraint<T> : Constraint
        where T : IEntity
    {
        protected readonly T _Expected;
        protected readonly IEntityWriter<T> _EntityWriter;
        protected List<PropertyDifference> _Differences;

        public EntityEqualConstraint(T expected, IEntityWriter<T> writer)
        {
            _Expected = expected;
            _EntityWriter = writer;
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;

            if (actual is T)
            {
                _Differences = EntityComparer.ListDifferences(_Expected, (T)actual);

                return (_Differences.Count == 0);
            }
            
            return false;
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            if (actual is T)
                _EntityWriter.Write(writer, (T)actual);
            else
                writer.WriteActualValue(actual);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            _EntityWriter.Write(writer, (T)_Expected);
        }

        public override void WriteMessageTo(MessageWriter writer)
        {
            writer.DisplayDifferences(this);

            writer.Write("  Differences:\n");
            foreach (var difference in _Differences)
            {
                writer.Write("      Property {0} : Expected \"{1}\" But was \"{2}\" \n", new object[] { difference.Name, difference.Expected, difference.Actual });
            }
        } 

    }

    public class EntityCollectionEqualConstraint<T> : Constraint
        where T : IEntity
    {
        protected readonly ICollection<T> _Expected;
        protected readonly IEntityWriter<T> _EntityWriter;
        protected List<List<PropertyDifference>> _Differences;

        public EntityCollectionEqualConstraint(ICollection<T> expected, IEntityWriter<T> writer)
        {
            _Expected = expected;
            _EntityWriter = writer;
        }

        public override bool Matches(object actual)
        {
            // Change to make order important - this will make the comparison simple
            // Loop through expected.
            //    compare to entry in corresponding entry actual
            //    if different then add differences
            //    if entry not in actual (ie actual.count < expected.count
            //         note difference as missing entry *
            //    if extra entries in expected note as extra entry *
            //         
            //  * create new class to represent a entry in a collection then make differences a list of these


            base.actual = actual;
            bool found;

            _Differences = new List<List<PropertyDifference>>();

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

                _EntityWriter.Write(writer, entity);

                count++;
            }
            writer.Write(">");
        }
    } 
}
