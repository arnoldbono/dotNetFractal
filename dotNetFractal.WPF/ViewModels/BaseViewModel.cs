using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace dotNetFractal.WPF.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private RelayCommand<Window> m_acceptCommand;

        public ICommand AcceptCommand => m_acceptCommand ??= new RelayCommand<Window>(w => OnAccept(w));

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaiseIfChanged(ref string property, string value, [CallerMemberName] string name = @"")
        {
            if (string.Equals(property, value, StringComparison.Ordinal))
            {
                return;
            }

            property = value;
            OnPropertyChanged(name);
        }

        public void RaiseIfChanged(ref double property, double value, [CallerMemberName] string name = @"")
        {
            if (property == value)
            {
                return;
            }

            property = value;
            OnPropertyChanged(name);
        }

        public void RaiseIfChanged<T>(ref T property, T value, [CallerMemberName] string name = @"")
        {
            if (ReferenceEquals(property, value))
            {
                return;
            }

            property = value;
            OnPropertyChanged(name);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = @"")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private static void OnAccept(Window window)
        {
            window.DialogResult = true;
        }
    }
}