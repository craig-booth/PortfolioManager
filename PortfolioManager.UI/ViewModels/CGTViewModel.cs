﻿using System;
using System.Collections.ObjectModel;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.ViewModels
{
    class CGTViewModel : PortfolioViewModel
    {
  
        public ObservableCollection<CGTEventViewModel> CGTEvents { get; private set; }

        public decimal CurrentYearCapitalGainsOther { get; private set; }
        public decimal CurrentYearCapitalGainsDiscounted { get; private set; }
        public decimal CurrentYearCapitalGainsTotal { get; private set; }
        public decimal CurrentYearCapitalLossesOther { get; private set; }
        public decimal CurrentYearCapitalLossesDiscounted { get; private set; }
        public decimal CurrentYearCapitalLossesTotal { get; private set; }
        public decimal GrossCapitalGainOther { get; private set; }
        public decimal GrossCapitalGainDiscounted { get; private set; }
        public decimal GrossCapitalGainTotal { get; private set; }
        public decimal Discount { get; private set; }
        public decimal NetCapitalGainOther { get; private set; }
        public decimal NetCapitalGainDiscounted { get; private set; }
        public decimal NetCapitalGainTotal { get; private set; }

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

        public CGTViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.FinancialYear;

            CGTEvents = new ObservableCollection<CGTEventViewModel>();
        }

        public async override void RefreshView()
        {
            Heading = string.Format("CGT Report for {0}/{1} Financial Year", _Parameter.FinancialYear - 1, _Parameter.FinancialYear);

            var responce = await _Parameter.RestClient.Portfolio.GetCGTLiability(DateUtils.FinancialYear(_Parameter.FinancialYear));
            if (responce == null)
                return;

            CurrentYearCapitalGainsOther = responce.CurrentYearCapitalGainsDiscounted;
            CurrentYearCapitalGainsDiscounted = responce.CurrentYearCapitalGainsDiscounted;
            CurrentYearCapitalGainsTotal = responce.CurrentYearCapitalGainsTotal;
            CurrentYearCapitalLossesOther = responce.CurrentYearCapitalLossesOther;
            CurrentYearCapitalLossesDiscounted = responce.CurrentYearCapitalLossesDiscounted;
            CurrentYearCapitalLossesTotal = responce.CurrentYearCapitalLossesTotal;
            GrossCapitalGainOther = responce.GrossCapitalGainOther;
            GrossCapitalGainDiscounted = responce.GrossCapitalGainDiscounted;
            GrossCapitalGainTotal = responce.GrossCapitalGainTotal;
            Discount = responce.Discount;
            NetCapitalGainOther = responce.NetCapitalGainOther;
            NetCapitalGainDiscounted = responce.NetCapitalGainDiscounted;
            NetCapitalGainTotal = responce.NetCapitalGainTotal;

            CGTEvents.Clear();
            foreach (var cgtEvent in responce.Events)
                CGTEvents.Add(new CGTEventViewModel(cgtEvent));

            OnPropertyChanged("");
        }
    }

    class CGTEventViewModel
    {
        public string CompanyName { get; private set; }
        public DateTime EventDate { get; private set; }
        public decimal CostBase { get; private set; }
        public decimal AmountReceived { get; private set; }
        public decimal CapitalGain { get; private set; }
        public string Method { get; private set; }

        public CGTEventViewModel(CgtLiabilityResponse.CgtLiabilityEvent cgtEvent)
        {
            CompanyName = cgtEvent.Stock.FormattedCompanyName();
            EventDate = cgtEvent.EventDate;
            CostBase = cgtEvent.CostBase;
            AmountReceived = cgtEvent.AmountReceived;
            CapitalGain = cgtEvent.CapitalGain;
            if (cgtEvent.Method == CGTMethod.Discount)
                Method = "Discount";
            else if (cgtEvent.Method == CGTMethod.Indexation)
                Method = "Indexation";
            else
                Method = "Other";

        }
    }

}
