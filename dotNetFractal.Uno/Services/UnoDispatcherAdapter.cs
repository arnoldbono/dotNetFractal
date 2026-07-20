using dotNetFractal.UI.Services;
using Microsoft.UI.Dispatching;

namespace dotNetFractal.Uno.Services;

/// <summary>
/// Uno implementation of IDispatcherAdapter using Microsoft.UI.Dispatching.DispatcherQueue.
/// </summary>
internal class UnoDispatcherAdapter : IDispatcherAdapter
{
    private readonly DispatcherQueue m_dispatcher;

    public UnoDispatcherAdapter(DispatcherQueue dispatcher)
    {
        m_dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    public bool IsOnUIThread => m_dispatcher.HasThreadAccess;

    public void RunOnUIThread(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (m_dispatcher.HasThreadAccess)
        {
            action();
        }
        else
        {
            var resetEvent = new ManualResetEvent(false);
            m_dispatcher.TryEnqueue(() =>
            {
                try
                {
                    action();
                }
                finally
                {
                    resetEvent.Set();
                }
            });
            resetEvent.WaitOne();
        }
    }

    public void RunOnUIThreadAsync(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        m_dispatcher.TryEnqueue(new DispatcherQueueHandler(action));
    }
}
