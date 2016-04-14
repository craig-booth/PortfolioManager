using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PortfolioManager.UI.Utilities
{

    public class RelayCommand : ICommand
    {
        private Action _MethodToExecute;
        private Func<bool> _CanExecuteEvaluator;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
        {
            this._MethodToExecute = methodToExecute;
            this._CanExecuteEvaluator = canExecuteEvaluator;
        }

        public RelayCommand(Action methodToExecute)
            : this(methodToExecute, null)
        {
        }

        public bool CanExecute(object Paramater)
        {
            if (this._CanExecuteEvaluator == null)
            {
                return true;
            }
            else
            {
                bool result = this._CanExecuteEvaluator.Invoke();
                return result;
            }
        }

        public void Execute(object Parameter)
        {
            this._MethodToExecute.Invoke();
        }
    }

    public class RelayCommand<T> : ICommand where T : class
    {
        private Action<T> _MethodToExecute;
        private Func<bool> _CanExecuteEvaluator;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<T> methodToExecute, Func<bool> canExecuteEvaluator)
        {
            this._MethodToExecute = methodToExecute;
            this._CanExecuteEvaluator = canExecuteEvaluator;
        }

        public RelayCommand(Action<T> methodToExecute)
            : this(methodToExecute, null)
        {
        }

        public bool CanExecute(object Paramater)
        {
            if (this._CanExecuteEvaluator == null)
            {
                return true;
            }
            else
            {
                bool result = this._CanExecuteEvaluator.Invoke();
                return result;
            }
        }

        public void Execute(object Parameter)
        {
            this._MethodToExecute.Invoke(Parameter as T);
        }
    }
}
