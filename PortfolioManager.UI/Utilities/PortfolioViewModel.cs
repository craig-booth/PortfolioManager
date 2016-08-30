using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace PortfolioManager.UI.Utilities
{
    abstract class PortfolioViewModel : PageViewModel, IPageViewModel
    {
        protected ViewParameter _Parameter;

        public PortfolioViewModel(string label, ViewParameter parameter)
            : base(label)
        {
            _Parameter = parameter;
        }

        public void ParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshView();
        }

        public override void Activate()
        {
            if (_Parameter != null)
                _Parameter.PropertyChanged += ParameterChanged;

            RefreshView();
        }

        public override void Deactivate()
        {
            if (_Parameter != null)
                _Parameter.PropertyChanged -= ParameterChanged;
        }

        public virtual void RefreshView()
        {

        }
    }
}
