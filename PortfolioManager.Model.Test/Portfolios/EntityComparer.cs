using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios
{
    public class PropertyDifference
    {
        public string Name;
        public string Expected;
        public string Actual;

        public PropertyDifference(string name, string expected, string actual)
        {
            Name = name;
            Expected = expected;
            Actual = actual;
        }
    }

    public static class EntityComparer
    {
        public static List<PropertyDifference> ListDifferences(IEntity expected, IEntity actual)
        {
            var differences = new List<PropertyDifference>();
            
            var entityType = expected.GetType();            

            PropertyInfo[] propertyInfos;
            propertyInfos = entityType.GetProperties();
        
            foreach (var propertyInfo in propertyInfos)
            {
                var expectedValue = entityType.GetProperty(propertyInfo.Name).GetValue(expected);
                var actualValue = entityType.GetProperty(propertyInfo.Name).GetValue(actual);

                if (!actualValue.Equals(expectedValue))
                    differences.Add(new PropertyDifference(propertyInfo.Name, expectedValue.ToString(), actualValue.ToString()));
            }

            return differences;
        }

        public static bool Equals(IEntity expected, IEntity actual)
        {
            var entityType = expected.GetType();

            PropertyInfo[] propertyInfos;
            propertyInfos = entityType.GetProperties();

            foreach (var propertyInfo in propertyInfos)
            {
                var expectedValue = entityType.GetProperty(propertyInfo.Name).GetValue(expected);
                var actualValue = entityType.GetProperty(propertyInfo.Name).GetValue(actual);

                if (!actualValue.Equals(expectedValue))
                    return false;
            }

            return true;
        }

    }
}
