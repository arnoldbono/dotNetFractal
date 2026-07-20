using System.Windows.Threading;
using dotNetFractal.UI.Services;

namespace dotNetFractal.WPF.Services;

/// <summary>
/// WPF implementation of IDispatcherAdapter using System.Windows.Threading.Dispatcher.
/// </summary>
internal class WpfDispatcherAdapter : IDispatcherAdapter
{
    private readonly Dispatcher m_dispatcher;

    public WpfDispatcherAdapter(Dispatcher dispatcher)
    {
        m_dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    public bool IsOnUIThread => m_dispatcher.CheckAccess();

    public void RunOnUIThread(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (m_dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            m_dispatcher.Invoke(action);
        }
    }

    public void RunOnUIThreadAsync(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        m_dispatcher.InvokeAsync(action);
    }
}
