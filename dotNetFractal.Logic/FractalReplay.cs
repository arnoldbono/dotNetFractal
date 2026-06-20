using System.Collections.Generic;

namespace dotNetFractal.Logic
{
    // Maintains a list of DisplayArea to store the history of areas.
    public class FractalReplay
    {
        private readonly List<IDisplayArea> m_history = new ();

        // Add an area to the history
        public int Add(IDisplayArea area)
        {
            if (area == default)
                return -1;
            if (m_history.Count != 0)
            {
                var lastArea = m_history[^1];
                if (lastArea == area)
                    return m_history.Count - 1;
                var index = m_history.FindIndex(a => a == area);
                if (index != -1)
                    return index;
            }
            m_history.Add(area);
            return m_history.Count - 1;
        }

        public int Add(IFractalArea area)
        {
            return Add(area.DisplayArea);
        }

        public void ClearHistory()
        {
            m_history.Clear();
        }

        public int HistoryCount => m_history.Count;

        public int GetIndex(IDisplayArea area)
        {
            return m_history.FindIndex(p => p == area);
        }

        public IDisplayArea this[int index]
        {
            get
            {
                if (index >= 0 && index < m_history.Count)
                    return m_history[index];
                return default;
            }
        }

        // Useful to restart the history from a specific area.
        public void RemoveAllFromIndex(int index)
        {
            int count = m_history.Count;
            if (index >= 0 && index < count)
                m_history.RemoveRange(index + 1, count - index - 1);
        }

        public IDisplayArea[] GetHistory()
        {
            return [..m_history.AsReadOnly()];
        }
    }
}
