using System.Collections.Generic;
using Easy.Data.Common.Interface;

namespace Easy.Data.Common.Structure_Class
{
    public class SafeQueue<T> where T : class 
    {
        private readonly object _lockObject = new object();
        private readonly Queue<T> _queue;

        public SafeQueue(int capacity = 2)
        {
            _queue = new Queue<T>(capacity);
        }

        public int Enqueue(T item)
        {
            int count = 0;
            lock (_lockObject)
            {
                if (!_queue.Contains(item))
                {
                    _queue.Enqueue(item);
                }
                count = _queue.Count;
            }
            return count;
        }

        public T Dequeue(out bool bIsSuccessed)
        {
            bIsSuccessed = false;
            T item = null;
            lock (_lockObject)
            {
                if (_queue.Count > 0)
                {
                    item = _queue.Dequeue();
                    bIsSuccessed = true;
                }
            }
            return item;
        }

        public int GetCount()
        {
            int count = 0;
            lock(_lockObject)
            {
                count = _queue.Count;
            }
            return count;
        }
    }
}
