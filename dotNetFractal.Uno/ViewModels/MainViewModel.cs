using dotNetFractal.UI.ViewModels;
using dotNetFractal.Uno.Services;
using Microsoft.UI.Dispatching;

namespace dotNetFractal.Uno.ViewModels;

public class MainViewModel : SharedMainViewModel
{
    public MainViewModel()
        : base(
            new UnoDispatcherAdapter(DispatcherQueue.GetForCurrentThread()),
            new UnoBitmapConverter(),
            new UnoFileDialogService(((App)Application.Current).MainWindow!),
            new UnoClipboardService(),
            new UnoWindowManager(),
            new UnoDistributionGraphService())
    {
    }
}
