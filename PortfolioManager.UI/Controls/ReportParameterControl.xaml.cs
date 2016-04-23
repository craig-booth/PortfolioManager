using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.Controls
{
    /// <summary>
    /// Interaction logic for ReportParameterControl.xaml
    /// </summary>
    public partial class ReportParameterControl : UserControl
    {
        public ReportParameterControl()
        {
            InitializeComponent();

            _SelectionType = ParameterType.SingleDate;
        }

        private ReportParmeter _Value;
        public ReportParmeter Value
        {
            get
            {
                return _Value;
            }

            set
            {
                if (AllowableValue(value))
                {
                    _Value = value;
                    SetValue(ValueProperty, value);
                }
                else
                    throw new ArgumentOutOfRangeException();
              } 
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(ReportParmeter), typeof(ReportParameterControl), null);

        private ParameterType _SelectionType;
        public ParameterType SelectionType
        {
            get
            {
                return _SelectionType;
            }

            set
            {
                _SelectionType = value;

                if ((_Value == null)  || (!AllowableValue(_Value)))
                    Value = DefaultValue(_SelectionType);
            } 
        }

        private bool AllowableValue(ReportParmeter value)
        {
            if (_SelectionType == ParameterType.DateRange)
                return true;
            else
                return (_SelectionType == value.Type);
        }

        private ReportParmeter DefaultValue(ParameterType type)
        {
            if (type == ParameterType.SingleDate)
                return ReportParmeter.Today();
            else
                return ReportParmeter.CurrentFinancialYear();
        }
        
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Value = ReportParmeter.CurrentFinancialYear();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Value = ReportParmeter.PreviousFinancialYear();
        }
    }
}
