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
            if (e.PropertyName == "Portfolio")
                RefreshView();
            else if ((e.PropertyName == "Stock") && Options.AllowStockSelection)
                RefreshView();
            else if ((e.PropertyName == "Date") && (Options.DateSelection == DateSelectionType.Single))
                RefreshView();
            else if ((e.PropertyName == "StartDate") && (Options.DateSelection == DateSelectionType.Range))
                RefreshView();
            else if ((e.PropertyName == "EndDate") && (Options.DateSelection == DateSelectionType.Range))
                RefreshView();
            else if ((e.PropertyName == "FinancialYear") && (Options.DateSelection == DateSelectionType.FinancialYear))
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

        public static string FormattedCompanyName(string asxCode, string companyName)
        {
            return string.Format("{0} ({1})", companyName, asxCode);
        }
    }
}
