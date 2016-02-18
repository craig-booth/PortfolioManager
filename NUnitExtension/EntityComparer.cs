using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework.Constraints;

namespace NUnitExtension
{
    public class EntityComparer
    {
        protected List<string> Ignore;

        public EntityComparer()
        {
            Ignore = new List<string>();
        }

        public EntityComparer(string propertiesToIgnore)
            : this()
        {
            Ignore.AddRange(propertiesToIgnore.Split(','));
        }

        public virtual bool Equal(object expected, object actual)
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
                    if ((property.PropertyType != typeof(string) && (property.PropertyType.GetInterface("IEnumerable") != null)))
                    {
                        var collectionConstraint = EntityConstraint.CollectionEqual(property.GetValue(expected) as IEnumerable);
                        if (!collectionConstraint.Matches(property.GetValue(actual)))
                            return false;
                    } 
                    else  
                    {
                        if (!property.GetValue(expected).Equals(property.GetValue(actual)))
                            return false;
                    }
                }
            }

            return true;
        }
    }
}
