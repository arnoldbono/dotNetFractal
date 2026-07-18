using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.UI.Xaml;

namespace dotNetFractal.Uno.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
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
    }
}
