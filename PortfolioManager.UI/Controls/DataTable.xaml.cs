using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using PortfolioManager.UI.Utilities;

namespace PortfolioManager.UI.Controls
{
    /// <summary>
    /// Interaction logic for DataTable.xaml
    /// </summary>
    public partial class DataTable : UserControl
    {
        public static DependencyProperty ColumnsProperty = DependencyProperty.Register(
                      "Columns", typeof(ObservableCollection<DataTableColumn>), typeof(DataTable),
                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public ObservableCollection<DataTableColumn> Columns
        {
            get { return (ObservableCollection<DataTableColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public Type DataType { get; set; }

        public static DependencyProperty DataProperty = DependencyProperty.Register(
                      "Data", typeof(IEnumerable), typeof(DataTable), new PropertyMetadata(new PropertyChangedCallback(OnDataChanged)));
        public IEnumerable Data
        {
            get { return (IEnumerable)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }


        public static DependencyProperty SummaryFieldsProperty = DependencyProperty.Register(
                      "SummaryFields", typeof(ObservableCollection<SummaryField>), typeof(DataTable));
        public ObservableCollection<SummaryField> SummaryFields
        {
            get { return (ObservableCollection<SummaryField>)GetValue(SummaryFieldsProperty); }
            set { SetValue(SummaryFieldsProperty, value); }
        }

        public DataTable()
        {
            InitializeComponent();

            DataType = typeof(object);

            Columns = new ObservableCollection<DataTableColumn>();
            Columns.CollectionChanged += ColumnsChanged;

            SummaryFields = new ObservableCollection<SummaryField>();
            CreateSummaryFields(Columns);
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataTable = (DataTable)d;

            var collectionChangedInterface = e.NewValue as INotifyCollectionChanged;
            if (collectionChangedInterface != null)
                collectionChangedInterface.CollectionChanged += dataTable.DataChanged;

            dataTable.CalculateSummary();
        }

        public void DataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CalculateSummary();
        }

        private void ColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var columns = sender as IList<DataTableColumn>;

            int columnNumber;

            // Column Headings
            columnNumber = 0;
            columnHeadings.ColumnDefinitions.Clear();
            foreach (var column in columns)
            {
                columnHeadings.ColumnDefinitions.Add(new ColumnDefinition() { Width = column.Width });

                var columnHeader = new ContentPresenter() { Content = column };
                columnHeadings.Children.Add(columnHeader);
                columnHeader.SetValue(Grid.ColumnProperty, columnNumber);

                columnNumber++;
            }

            // Data
            data.ItemTemplate = CreateDataItemTemplate(columns);

            // Summary   
            CreateSummaryFields(columns);
            CalculateSummary();

            summary.ContentTemplate = CreateSummaryTemplate(columns);
        }

        private DataTemplate CreateDataItemTemplate(IList<DataTableColumn> columns)
        {
            var gridFactory = new FrameworkElementFactory(typeof(Grid));

            int columnNumber = 0;
            foreach (DataTableColumn column in columns)
            {
                var columnFactory = new FrameworkElementFactory(typeof(ColumnDefinition));
                columnFactory.SetValue(ColumnDefinition.WidthProperty, column.Width);
                gridFactory.AppendChild(columnFactory);

                var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                textBlockFactory.SetValue(Grid.ColumnProperty, columnNumber);
                textBlockFactory.SetValue(TextBlock.TextProperty, column.MemberBinding);
                textBlockFactory.SetValue(TextBlock.TextAlignmentProperty, column.TextAlignment);
                textBlockFactory.SetValue(TextBlock.PaddingProperty, new Thickness(5, 0, 0, 5));

                gridFactory.AppendChild(textBlockFactory);

                columnNumber++;
            }

            var template = new DataTemplate()
            {
                VisualTree = gridFactory
            };

            return template;
        }

        private DataTemplate CreateSummaryTemplate(IList<DataTableColumn> columns)
        {
            var gridFactory = new FrameworkElementFactory(typeof(Grid));

            int columnNumber = 0;
            foreach (DataTableColumn column in columns)
            {
                var columnFactory = new FrameworkElementFactory(typeof(ColumnDefinition));
                columnFactory.SetValue(ColumnDefinition.WidthProperty, column.Width);
                gridFactory.AppendChild(columnFactory);

                var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                textBlockFactory.SetValue(Grid.ColumnProperty, columnNumber);

                var binding = new Binding(String.Format("SummaryFields[{0}].Value", columnNumber));
                binding.ElementName = "dataTable";
                binding.StringFormat = column.MemberBinding.StringFormat;
                binding.Mode = BindingMode.OneWay;
                textBlockFactory.SetValue(TextBlock.TextProperty, binding);
                textBlockFactory.SetValue(TextBlock.TextAlignmentProperty, column.TextAlignment);
                textBlockFactory.SetValue(TextBlock.PaddingProperty, new Thickness(5, 0, 0, 5));

                gridFactory.AppendChild(textBlockFactory);

                columnNumber++;
            }

            var template = new DataTemplate()
            {
                VisualTree = gridFactory
            };

            return template;
        }

        private void CreateSummaryFields(IList<DataTableColumn> columns)
        {
            SummaryFields.Clear();

            foreach (var column in columns)
            {
                SummaryField summaryField;

                if (column.SummaryType == SummaryType.Fixed)
                    summaryField = new SummaryField(column.SummaryValue);
                else
                {
                    var propertyInfo = DataType.GetProperty(column.MemberBinding.Path.Path);
                    if (propertyInfo != null)
                       summaryField = new SummaryField(column.SummaryType, propertyInfo);
                    else
                        summaryField = new SummaryField("");                    

                }

                SummaryFields.Add(summaryField);
            }
        }

        public void CalculateSummary()
        {
            if (Data == null)
                return;

            int i = 0;

            foreach (var column in Columns)
                SummaryFields[i++].CalculateValue(Data);
            i++;
        }

    }

    public enum SummaryType { Fixed, Sum, Average, Max, Min };
    public class DataTableColumn
    {
        // General
        public GridLength Width { get; set; }

        // Heading
        public string Heading { get; set; }

        // Data
        public Binding MemberBinding { get; set; }
        public TextAlignment TextAlignment { get; set; }

        // Summary
        public SummaryType SummaryType { get; set; }
        public object SummaryValue { get; set; }
    }

    public class SummaryField : NotifyClass 
    {
        public SummaryType Type { get; }

        private object _Value;
        public object Value
        {
            get
            {
                return _Value;
            }
         
            set
            {
                _Value = value;
                OnPropertyChanged();
            }
        }

        public System.Reflection.PropertyInfo PropertyInfo { get; }

        public SummaryField(object value)
        {
            Type = SummaryType.Fixed;
            _Value = value;
        }

        public SummaryField(SummaryType type, System.Reflection.PropertyInfo propertyInfo)
        {
            Type = type;
            PropertyInfo = propertyInfo;
            _Value = Activator.CreateInstance(PropertyInfo.PropertyType);
        }

        private void ClearValue()
        {
            Value = Activator.CreateInstance(PropertyInfo.PropertyType);
        }

        public void CalculateValue(IEnumerable data)
        {
            if (Type == SummaryType.Sum)
            {
                var totalValue = (decimal)Activator.CreateInstance(PropertyInfo.PropertyType);

                foreach (var item in data)
                {
                    var itemValue = (decimal)PropertyInfo.GetValue(item);
                    totalValue += (decimal)itemValue;
                }

                Value = totalValue;
            }
            else if (Type == SummaryType.Min)
            {
                decimal minValue = 0.00m;
                                
                bool first = true;
                foreach (var item in data)
                {
                    var itemValue = (decimal)PropertyInfo.GetValue(item);

                    if (first)
                    {
                        minValue = itemValue;
                        first = false;
                    }
                    else
                    {
                        if ((minValue as IComparable).CompareTo(itemValue) > 0)
                            minValue = itemValue;
                    }
                }

                Value = minValue;
            }
            else if (Type == SummaryType.Max)
            {
                decimal maxValue = 0.00m;
                bool first = true;
                foreach (var item in data)
                {
                    var itemValue = (decimal)PropertyInfo.GetValue(item);

                    if (first)
                    {
                        maxValue = itemValue;
                        first = false;
                    }
                    else
                    {
                        if ((maxValue as IComparable).CompareTo(itemValue) < 0)
                            maxValue = itemValue;
                    }
                }

                Value = maxValue;
            }
            else if (Type == SummaryType.Average)
            {
                var totalValue = (decimal)Activator.CreateInstance(PropertyInfo.PropertyType);
                int count = 0;

                foreach (var item in data)
                {
                    var itemValue = (decimal)PropertyInfo.GetValue(item);

                    totalValue += (decimal)itemValue;
                    count++;
                }

                if (count > 0)
                    Value = totalValue / count;
            }
        }
    }

}
