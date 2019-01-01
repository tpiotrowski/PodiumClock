using System;
using System.Windows.Input;

namespace ItSoft.Command
{
    public class NativeCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Func<object, bool> _predicate;

        public NativeCommand(Action<object> action, Func<object, bool> predicate)
        {
            _action = action;
            _predicate = predicate;
        }

        public bool CanExecute(object parameter)
        {
            return _predicate(parameter);
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
