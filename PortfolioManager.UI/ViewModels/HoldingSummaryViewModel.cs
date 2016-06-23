﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class HoldingSummaryViewModel : ViewModel
    {

        public string ASXCode { get; private set; }

        private string _Heading;
        new public string Heading
        {
            get
            {
                return _Heading;
            }
            private set
            {
                _Heading = value;
                OnPropertyChanged();
            }
        }

        public HoldingSummaryViewModel(string label)
            : base(label)
        {
        }


        public override void SetData(object data)
        {
            var holding = data as ShareHolding;

            ASXCode = holding.Stock.ASXCode;

            Heading = "Holdings for " + ASXCode;
        }

    }
}
