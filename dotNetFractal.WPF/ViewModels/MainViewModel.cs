using System.Windows;
using System.Windows.Threading;
using dotNetFractal.UI.ViewModels;
using dotNetFractal.WPF.Services;

namespace dotNetFractal.WPF.ViewModels;

public class MainViewModel : SharedMainViewModel
{
    public MainViewModel()
        : base(
            new WpfDispatcherAdapter(Dispatcher.CurrentDispatcher),
            new WpfBitmapConverter(),
            new WpfFileDialogService(),
            new WpfClipboardService(),
            new WpfWindowManager(Application.Current.MainWindow),
            new WpfDistributionGraphService())
    {
    }
}
