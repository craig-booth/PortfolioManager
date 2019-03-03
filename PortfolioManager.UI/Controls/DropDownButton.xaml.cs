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
    /// Interaction logic for DropDownButton.xaml
    /// </summary>
    public partial class DropDownButton : UserControl
    {
        public static DependencyProperty SelectedCommandProperty = DependencyProperty.Register(
            "SelectedCommand", typeof(RelayUICommand), typeof(DropDownButton),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public RelayUICommand SelectedCommand
        {
            get { return (RelayUICommand)GetValue(SelectedCommandProperty); }
            set { SetValue(SelectedCommandProperty, value); }
        }

        public static DependencyProperty CommandsProperty = DependencyProperty.Register(
              "Commands", typeof(ObservableCollection<RelayUICommand>), typeof(DropDownButton),
              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public ObservableCollection<RelayUICommand> Commands
        {
            get { return (ObservableCollection<RelayUICommand>)GetValue(CommandsProperty); }
            set { SetValue(CommandsProperty, value); }
        }
        public DropDownButton()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedCommand = (RelayUICommand)((Button)e.OriginalSource).Command;
            this.expander.IsExpanded = false;
        }
    }
}
