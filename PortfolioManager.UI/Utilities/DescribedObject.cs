using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.UI.Utilities
{

    class DescribedObject<T>
    {
        public T Value { get; set; }
        public string Description { get; set; }

        public DescribedObject(T data, string description)
        {
            Value = data;
            Description = description;
        }
    }
}
