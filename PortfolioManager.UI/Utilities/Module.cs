using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;


namespace PortfolioManager.UI.Utilities
{


    class Module : NotifyClass
    {
        public string Label { get; private set; }
        public Geometry Image { get; private set; }

        public Visibility PageParameterAreaVisible { get; set; }

        public Visibility PageSelectionAreaVisible { get; set; }
        public List<IPageViewModel> Pages { get; private set; }

        public Module(string label, string image)
        {
            Label = label;
            Image = App.Current.FindResource(image) as Geometry;

            Pages = new List<IPageViewModel>();

            PageParameterAreaVisible = Visibility.Collapsed;
            PageSelectionAreaVisible = Visibility.Collapsed;
        }
    }
    
}
