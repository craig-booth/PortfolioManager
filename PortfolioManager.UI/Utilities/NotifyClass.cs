﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PortfolioManager.UI.Utilities
{
    public abstract class NotifyClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
