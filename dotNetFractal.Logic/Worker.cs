using System;
using System.Threading;

namespace dotNetFractal.Logic
{
    public abstract class Worker
    {
        private RegisteredWaitHandle m_waitHandle = null;
        private ManualResetEvent m_threadCompletedEvent = null;
        private Mutex m_mutex = null;
        private bool m_stop = false;
        private bool m_stopped = false;
        private Action<Action> m_threadPoolExecutor = null;

        public bool Stopped
        {
            get { return m_stopped; }
            protected set { m_stopped = value; }
        }

        public bool Stop
        {
            get { return m_stop; }
            protected set { m_stop = value; }
        }

        public Worker()
        {
            m_mutex = new Mutex(false);
            m_threadCompletedEvent = new ManualResetEvent(false);
        }

        // placeholder
        abstract protected void ThreadProc();

        private void ThreadProcWrapper(object state)
        {
            try
            {
                ThreadProc();
            }
            finally
            {
                // Signal the completion events when the thread finishes
                m_threadCompletedEvent?.Set();
            }
        }

        public virtual void StartThread(Action<Action> threadPoolExecutor = null)
        {
            LockMutex();
            m_stop = false;
            m_stopped = false;
            m_threadPoolExecutor = threadPoolExecutor;
            m_threadCompletedEvent.Reset();

            if (m_threadPoolExecutor != null)
            {
                // Use the provided thread pool executor
                m_threadPoolExecutor(() => ThreadProcWrapper(null));
            }
            else
            {
                // Use the default .NET ThreadPool
                ThreadPool.QueueUserWorkItem(ThreadProcWrapper);
            }
            UnlockMutex();
        }

        public virtual void StopThread()
        {
            Stop = true;
            if (m_stopped)
                return;

            // Wait for the thread pool work item to complete
            m_threadCompletedEvent?.WaitOne();

            if (m_waitHandle != null)
            {
                m_waitHandle.Unregister(null);
                m_waitHandle = null;
            }

            m_stopped = true;
        }

        public void LockMutex()
        {
            m_mutex.WaitOne();
        }

        public void UnlockMutex()
        {
            m_mutex.ReleaseMutex();
        }
    }
}
