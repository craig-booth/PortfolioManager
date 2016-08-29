using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using PortfolioManager.Service;

namespace PortfolioManager.UI.Utilities
{
 
    interface IViewModel
    {
        string Label { get; }
        string Heading { get; }
        ViewOptions Options { get; }

        void Activate();
        void Deactivate();
    }


    enum DateSelectionType { None, Single, Range, FinancialYear }

    class ViewOptions
    {
        public bool AllowStockSelection { get; set; }
        public DateSelectionType DateSelection { get; set; }
    }

    abstract class ViewModel : NotifyClass, IViewModel 
    {
        public string Label { get; protected set; }
        public string Heading { get; protected set; }
        public ViewOptions Options { get; set; }

        public virtual void Activate()
        {
        }

        public virtual void Deactivate()
        {
        }

        public ViewModel(string label)
        {
            Label = label;
            Heading = label;

            Options = new ViewOptions()
            {
                AllowStockSelection = false,
                DateSelection = DateSelectionType.None
            };
        }
    }

    abstract class PortfolioViewModel : ViewModel, IViewModel
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
