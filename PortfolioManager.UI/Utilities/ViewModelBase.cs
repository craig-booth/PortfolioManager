using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace PortfolioManager.UI.Utilities
{

    class ViewModel : NotifyClass, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _Errors;

        public ViewModel()
        {
            _Errors = new Dictionary<string, List<string>>();
        }

        public bool HasErrors
        {
            get
            {
                return (_Errors.Count > 0);
            }
        }
        
        public IEnumerable GetErrors(string propertyName)
        {
            List<string> errors;

            if (_Errors.TryGetValue(propertyName, out errors))
                return errors;

            return null;
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        
        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void AddError(string error, [CallerMemberName] string propertyName = null)
        {
            List<string> errors;

            if (!_Errors.TryGetValue(propertyName, out errors))
            {
                errors = new List<string>();
                _Errors.Add(propertyName, errors);
            }

            errors.Add(error);
            OnErrorsChanged(propertyName);
        }

        protected void ClearErrors([CallerMemberName] string propertyName = null)
        {
            _Errors.Remove(propertyName);
        }
    }





}
