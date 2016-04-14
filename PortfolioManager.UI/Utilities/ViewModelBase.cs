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
    public interface IDataViewModel
    {
        string Heading { get; }
        void SetData(Portfolio portfolio, object data);
    }

    public interface IViewModel
    {
        string Heading { get; }
    }

    public interface IPortfolioViewModel : IViewModel
    {
        Portfolio Portfolio { get; }
        void SetPortfolio(Portfolio portfolio);
    }

    public interface IViewModelWithData
    {
        void SetData(object data);
    }

    public abstract class ViewModel : NotifyClass, IViewModel 
    {
        public string Heading { get; protected set; }
    }

    public abstract class PortfolioViewModel : ViewModel, IPortfolioViewModel
    {
        protected Portfolio _Portfolio;
        public Portfolio Portfolio { get; }

        public virtual void SetPortfolio(Portfolio portfolio)
        {
            _Portfolio = portfolio;
        }
    }

}
