using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitExtension
{
    public class EntityConstraint
    {
        private static Dictionary<Type, EntityComparer> Comparers = new Dictionary<Type, EntityComparer>();

        public static void RegisterComparer(Type type, EntityComparer comparer)
        {
            if (Comparers.ContainsKey(type))
                Comparers[type] = comparer;
            else
                Comparers.Add(type, comparer);
        }

        public static void RegisterComparer(Type type, string propertiesToIgnore)
        {
            RegisterComparer(type, new EntityComparer(propertiesToIgnore));
        }

        public static EntityComparer GetComparer(object obj)
        {
            var objectType = obj.GetType();

            if (Comparers.ContainsKey(objectType))
                return Comparers[objectType];
            else
                throw new Exception(String.Format("Comparer not found for type {0}", objectType.Name));
        }

        public static EntityEqualConstraint Equal(object expected)
        {
            return new EntityEqualConstraint(expected);
        }

        public static EntityCollectionEqualConstraint CollectionEquals(IEnumerable expected)
        {
            return new EntityCollectionEqualConstraint(expected);
        }

        public static EntityCollectionEquivalantConstraint CollectionEquivalant(IEnumerable expected)
        {
            return new EntityCollectionEquivalantConstraint(expected);
        }
    }





}
