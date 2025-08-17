// (c) 2017 Roland Boon

using System;
using System.Diagnostics;
using System.Windows.Input;

namespace dotNetFractal.WPF
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Func<T, bool> m_canExecute;
        private readonly Action<T> m_execute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            m_execute = execute ?? throw new ArgumentNullException(nameof(execute));
            m_canExecute = canExecute ?? (_ => true);
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) =>
            m_canExecute == null || m_canExecute((T)parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter) =>
            m_execute((T)parameter);
    }
}