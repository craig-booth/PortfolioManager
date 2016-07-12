using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.UI.Utilities
{


    class Module : NotifyClass
    {
        public string Label { get; private set; }
        public Geometry Image { get; private set; }

        public Visibility ViewParameterAreaVisible { get; set; }
        public ViewParameter ViewParameter { get; set; }

        public Visibility ViewSelectionAreaVisible { get; set; }
        public List<IViewModel> Views { get; private set; }

        private IViewModel _SelectedView;
        public IViewModel SelectedView
        {
            get
            {
                return _SelectedView;
            }
            set
            {
                _SelectedView = value;
                if (_SelectedView != null)
                    _SelectedView.SetData(ViewParameter);
                OnPropertyChanged();
            }
        }

        public Module(string label, string image)
        {
            Label = label;
            Image = App.Current.FindResource(image) as Geometry;

            Views = new List<IViewModel>();

            ViewParameterAreaVisible = Visibility.Collapsed;
            ViewSelectionAreaVisible = Visibility.Collapsed;
        }
    }
    
}
