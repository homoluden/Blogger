using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Blogger.ViewModels.Commands
{
    public class DelegateCommand : ICommand
    {
        Action<object> _execute;
        Func<bool> _canExecute;
        bool _isCanBeExecuted;        

        public DelegateCommand(Action<object> executeAction, Func<bool> canExecuteAction)
        {
            _execute = executeAction;
            _canExecute = canExecuteAction;
        }
        
        public bool CanExecute(object parameter)
        {
            // Restrict command to execute by default
            var newCanBeExecuted = _canExecute != null ? _canExecute() : false;

            if (newCanBeExecuted != _isCanBeExecuted && CanExecuteChanged != null)
            {
                _isCanBeExecuted = newCanBeExecuted;
                CanExecuteChanged(this, EventArgs.Empty);
            }

            return _isCanBeExecuted;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute(parameter);
            }
        }
    }
}
