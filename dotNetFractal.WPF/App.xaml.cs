using System.Windows;
using ReactiveUI.Builder;

using dotNetFractal.Cuda;

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
