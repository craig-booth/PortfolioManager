using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework.Constraints;

namespace NUnitExtension
{
    public class EntityComparer
    {
        private List<string> Ignore;

        public EntityComparer()
        {
            Ignore = new List<string>();
        }

        public EntityComparer(string propertiesToIgnore)
            : this()
        {
            Ignore.AddRange(propertiesToIgnore.Split(','));
        }

        public new bool Equals(object expected, object actual)
        {
            if (actual == null)
                return false;

            if (expected.GetType() != actual.GetType())
                return false;

            var expectedType = expected.GetType();
            foreach (var property in expectedType.GetProperties())
            {
                if (!Ignore.Contains(property.Name))
                {
                    if (!property.GetValue(expected).Equals(property.GetValue(actual)))
                        return false;
                }
            }

            return true;
        }

        public void Write(MessageWriter writer, object entity)
        {
            if (entity == null)
                return;

            var entityType = entity.GetType();

            writer.Write("<{0}:- ", entityType.Name);

            foreach (var property in entityType.GetProperties())
            {
                if (!Ignore.Contains(property.Name))
                {
                    writer.Write("{0}: {1}, ", property.Name, property.GetValue(entity));
                }
            }

            writer.Write(">");
            writer.Write(writer.NewLine);
        }
    }
}
