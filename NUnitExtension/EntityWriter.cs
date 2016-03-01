using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework.Constraints;

namespace NUnitExtension
{
    public class EntityWriter
    {
        protected List<string> Ignore;

        public EntityWriter()
        {
            Ignore = new List<string>();
        }

        public EntityWriter(string propertiesToIgnore)
            : this()
        {
            Ignore.AddRange(propertiesToIgnore.Split(','));
        }

        public EntityWriter(IEnumerable<string> propertiesToIgnore)
            : this()
        {
            Ignore.AddRange(propertiesToIgnore);
        }

        public virtual void Write(MessageWriter writer, object entity)
        {
            if (entity == null)
                return;

            var entityType = entity.GetType();

            writer.Write("<{0}:- ", entityType.Name);

            foreach (var property in entityType.GetProperties())
            {
                if (!Ignore.Contains(property.Name))
                {
                    if (property.PropertyType == typeof(DateTime))
                        writer.Write("{0}: {1:d}, ", property.Name, property.GetValue(entity));
                    else if ((property.PropertyType != typeof(string) && (property.PropertyType.GetInterface("IEnumerable") != null)))
                    {
                        writer.Write("{0}: ", property.Name);
                        var collectionConstraint = EntityConstraint.CollectionEqual(property.GetValue(entity) as IEnumerable);
                        collectionConstraint.WriteDescriptionTo(writer);
                    }
                    else
                        writer.Write("{0}: {1}, ", property.Name, property.GetValue(entity));
                }
            }

            writer.Write(">");
            writer.Write(writer.NewLine);
        }
    }
}
