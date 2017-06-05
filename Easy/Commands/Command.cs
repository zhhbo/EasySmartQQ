using System;

namespace Easy.Commands
{
    public class Command : CommandBase
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;


        public Command(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        protected override bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        protected override void Execute(object parameter)
        {
            Execute();
        }

        public bool CanExecute()
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute()
        {
            _execute?.Invoke();
        }
    }

    public class Command<T> : CommandBase
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public Command(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        protected override bool CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        protected override void Execute(object parameter)
        {
            Execute((T)parameter);
        }

        public bool CanExecute(T parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(T parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}
