using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.UI.Utilities
{
    class Module
    {
        public string Label { get; private set; }
        public List<IViewModel> Views { get; private set; }

        public Module(string label)
        {
            Views = new List<IViewModel>();

            Label = label;
        }
    }

    class DummyView : IViewModel
    {
        public string Heading { get; private set; }

        public DummyView(string heading)
        {
            Heading = heading;
        }
    }
    
}
