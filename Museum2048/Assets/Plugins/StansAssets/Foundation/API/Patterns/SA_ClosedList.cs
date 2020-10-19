using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Events;

namespace SA.Foundation.Patterns
{
    public class SA_ClosedList<T> : SA_iClosedList<T>
    {
        readonly SA_Event<T> m_onItemAdded = new SA_Event<T>();
        readonly SA_Event<T> s_onItemRemoved = new SA_Event<T>();

        List<T> m_items = new List<T>();

        public IEnumerable<T> Items => m_items;

        public List<T> ItemsList
        {
            get => m_items;

            set
            {
                Clear();
                m_items = value;
                foreach (var item in value) m_onItemAdded.Invoke(item);
            }
        }

        public void SetList() { }

        public void Clear()
        {
            foreach (var item in m_items) s_onItemRemoved.Invoke(item);

            m_items.Clear();
        }

        public void Add(T item)
        {
            m_items.Add(item);
            m_onItemAdded.Invoke(item);
        }

        public void Remove(T item)
        {
            m_items.Remove(item);
            s_onItemRemoved.Invoke(item);
        }

        public SA_iEvent<T> OnItemAdded => m_onItemAdded;

        public SA_iEvent<T> OnItemRemoved => s_onItemRemoved;
    }
}
