using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;

using LiveCharts.Wpf;

namespace PortfolioManager.UI.Controls
{
    /// <summary>
    /// Interaction logic for PieChartLegend.xaml
    /// </summary>
    public partial class PieChartLegend : UserControl, IChartLegend
    {

        private List<SeriesViewModel> _series;

        public ObservableCollection<PieSeriesViewModel> PieSeries { get; set; }

        public string ValueFormat { get; set; }
        public string PercentFormat { get; set; }

        public PieChartLegend()
        {
            InitializeComponent();

            PieSeries = new ObservableCollection<PieSeriesViewModel>();

            ValueFormat = "{0:c0}";
            PercentFormat = "{0:p2}";

            DataContext = this;
        }

        public List<SeriesViewModel> Series
        {
            get { return _series; }
            set
            {
                _series = value;

                CreateLegend();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CreateLegend()
        {
            PieSeries.Clear();

            var pieChart = FindParent<PieChart>(this);
            if (pieChart == null)
                return;

            var seriesCollection = pieChart.Series;

            foreach (var series in seriesCollection)
            {
                var pieSeries = series as PieSeries;
                if (pieSeries != null)
                {
                    var item = new PieSeriesViewModel();
                    item.Fill = pieSeries.Fill;
                    item.PointGeometry = pieSeries.PointGeometry;
                    item.Stroke = pieSeries.Stroke;
                    item.StrokeThickness = pieSeries.StrokeThickness;
                    item.Title = pieSeries.Title;

                    var values = pieSeries.Values.GetPoints(pieSeries);
                    var firstChartPoint = values.First();

                    item.Value = string.Format(ValueFormat, firstChartPoint.Y);
                    item.Percentage = string.Format(PercentFormat, firstChartPoint.Participation);


                    PieSeries.Add(item);
                }
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }

    public class PieSeriesViewModel : SeriesViewModel
    {
        public string Value { get; set; }
        public string Percentage { get; set; }
    }

}

