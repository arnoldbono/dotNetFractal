using System.Windows;
using dotNetFractal.Logic;
using ReactiveUI.Builder;

namespace dotNetFractal.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        _ = CudaMandelbrotStartup.TrySelectCudaDevice();

        base.OnStartup(e);

        RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();
    }
}
