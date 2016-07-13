﻿using System;
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

        public Visibility ViewSelectionAreaVisible { get; set; }
        public List<IViewModel> Views { get; private set; }

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
