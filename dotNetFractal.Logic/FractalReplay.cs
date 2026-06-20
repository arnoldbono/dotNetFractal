using System.Collections.Generic;

namespace dotNetFractal.Logic
{
    // Maintains a list of DisplayArea to store the history of areas.
    public class FractalReplay<T> where T : IFractalUnit<T>, new()
    {
        private readonly List<DisplayArea<T>> m_history = new ();

        // Add an area to the history
        public int Add(DisplayArea<T> area)
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

        public int Add(FractalArea<T> area)
        {
            return Add(new DisplayArea<T>(area.DisplayArea));
        }

        public void ClearHistory()
        {
            m_history.Clear();
        }

        public int HistoryCount => m_history.Count;

        public int GetIndex(DisplayArea<T> area)
        {
            return m_history.FindIndex(p => p == area);
        }

        public DisplayArea<T> this[int index]
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

        public DisplayArea<T>[] GetHistory()
        {
            return [..m_history.AsReadOnly()];
        }
    }
}
