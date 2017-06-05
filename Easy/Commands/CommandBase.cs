using System;
using System.Windows.Input;

namespace Easy.Commands
{
    public abstract class CommandBase : ICommand
    {
        public delegate void ExecutedHandler(object parameter);
        private event ExecutedHandler _executed;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public  event ExecutedHandler Executed
        {
            add { _executed += value; }
            remove { _executed -= value; }
        }
        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
            _executed?.Invoke(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        protected abstract void Execute(object parameter);

        protected abstract bool CanExecute(object parameter);
    }
}
