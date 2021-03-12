using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BattleField
{
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        #endregion Fields

        #region Constructors
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object> execute)
            : this(execute, (Predicate<object>)null)
        { }

        public RelayCommand(Action execute)
            : this((o) => execute())
        { }

        public RelayCommand(Action execute, Func<bool> canExecute)
            : this((o) => execute(), (o) => canExecute())
        { }

        public RelayCommand(Action<object> execute, Func<bool> canExecute)
            : this(execute, (o) => canExecute())
        { }

        public RelayCommand(Action execute, Predicate<object> canExecute)
            : this((o) => execute(), canExecute)
        { }
        #endregion Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        #endregion ICommand Members
    }
}
