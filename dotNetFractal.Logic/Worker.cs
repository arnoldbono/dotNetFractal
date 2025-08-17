using System;
using System.Threading;

namespace dotNetFractal
{
    public abstract class Worker
	{
		private Thread m_thread = null;
        private Mutex m_mutex = null;
        private bool m_stop = false;
        private bool m_stopped = false;

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
        }

        // placeholder
        abstract protected void ThreadProc();

        public virtual void StartThread()
		{
			LockMutex();
            m_stop = false;
            m_stopped = false;
            m_thread = new Thread(new ThreadStart(ThreadProc));
			m_thread.Start();
			UnlockMutex();
		}

        public virtual void StopThread()
		{
			Stop = true;
            if (m_thread == null)
            {
                m_stopped = true;
            }
            else if (m_thread.IsAlive)
            {
                while (!m_stopped)
                {
                    m_thread.Join(); // to wait until a Thread ends
                }
            }
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
