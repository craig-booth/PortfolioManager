using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service;

using PortfolioManager.UI.ViewModels;

namespace PortfolioManager.UI.Utilities
{
    public class ViewNavigator : NotifyClass
    {
        private Dictionary<string, IViewModel> _ViewModels;
        private Stack<ViewWithData> _History;
        private ViewWithData _CurrentView;
        private Portfolio _CurrentPortfolio;

        public RelayCommand<ViewWithData> NavigateToCommand { get; private set; }
        public RelayCommand NavigateBackCommand { get; private set; }

        public ViewNavigator()
        {
            _ViewModels = new Dictionary<string, IViewModel>();
            _History = new Stack<ViewWithData>();

            NavigateToCommand = new RelayCommand<ViewWithData>(NavigateTo);
            NavigateBackCommand = new RelayCommand(NavigateBack, () => { return _History.Count > 1; });
        }

        public void SetPortfolio(Portfolio portfolio)
        {
            _CurrentPortfolio = portfolio;
        }

        public void RegisterViewModel(string name, IViewModel viewModel)
        {
            _ViewModels.Add(name, viewModel);
        }

        private IViewModel _CurrentViewModel;
        public IViewModel CurrentViewModel
        {
            get { return _CurrentViewModel; }
            set
            {
                _CurrentViewModel = value;
                OnPropertyChanged();
            }
        }
       
        public void NavigateTo(ViewWithData view)
        {
            _History.Push(_CurrentView);
            SetCurrentView(view);
        }

        public void NavigateBack()
        {
            var view = _History.Pop();
            SetCurrentView(view);
        }   
        
        private void SetCurrentView(ViewWithData view)
        {
            _CurrentView = view;
            CurrentViewModel = _ViewModels[_CurrentView.ViewName];

            (CurrentViewModel as IPortfolioViewModel)?.SetPortfolio(_CurrentPortfolio);
            (CurrentViewModel as IViewModelWithData)?.SetData(view.Data);
        }   
    }

    public class ViewWithData
    {
        public string ViewName { get; private set; }
        public object Data { get; private set; }
       
        public ViewWithData(string viewName, object data)
        {
            ViewName = viewName;
            Data = data;
        }
    }
}
