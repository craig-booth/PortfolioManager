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
 
    public interface IViewModel
    {
        string Label { get; }
        string Heading { get; }
        ViewOptions Options { get; }

        void SetData(object data);
    }


    public enum DateSelectionType { None, Single, Range, FinancialYear }

    public class ViewOptions
    {
        public bool AllowStockSelection { get; set; }
        public DateSelectionType DateSelection { get; set; }
    }

    public abstract class ViewModel : NotifyClass, IViewModel 
    {
        public string Label { get; protected set; }
        public string Heading { get; protected set; }
        public ViewOptions Options { get; set; }

        public abstract void SetData(object data);

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

    public abstract class PortfolioViewModel : ViewModel, IViewModel
    {
        public Portfolio Portfolio { get; private set; }

        public PortfolioViewModel(string label, Portfolio portfolio)
            : base(label)
        {
            Portfolio = portfolio;
        }
    }


}
