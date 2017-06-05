using System;
using System.Collections.Generic;

namespace Easy.Data.Common.Structure_Class
{
    public class SafeDictionaryQueue<TKey, TValue>
        where TValue : class
    {
        private Dictionary<TKey, SafeQueue<TValue>> _datas;

        public SafeDictionaryQueue()
        {
            _datas = new Dictionary<TKey, SafeQueue<TValue>>();
        }

        public void Add(TKey key, TValue value)
        {
            lock(this)
            {
                if (!_datas.ContainsKey(key))
                {
                    _datas.Add(key, new SafeQueue<TValue>());
                }
                _datas[key].Enqueue(value);
            }
        }

        public void Clear()
        {
            lock (this)
            {
                _datas.Clear();
            }
        }

        public SafeQueue<TValue> this[TKey key]
        {
            get
            {
                SafeQueue<TValue> result;
                lock(this)
                {
                    result = _datas.ContainsKey(key) ? _datas[key] : new SafeQueue<TValue>();
                }
                return result;
            }
        }

        public TValue GetSingle(TKey key,out bool isSuccessed)
        {
            return this[key].Dequeue(out isSuccessed);
        }

        public int GetSingleCount(TKey key)
        {
            int count = 0;
            lock(this)
            {
                count = _datas.ContainsKey(key) ? _datas[key].GetCount() : 0;
            }
            return count;
        }

        public List<TKey> GetKeys()
        {
            List<TKey> keys = new List<TKey>();
            lock(this)
            {
                foreach(var item in _datas)
                {
                    keys.Add(item.Key);
                }
            }
            return keys;
        }

        public List<TValue> RemainMaxCount(TKey key, int maxCount)
        {
            List<TValue> result = new List<TValue>();
            bool isSuccessed = false;
            lock (this)
            {
                if (_datas.ContainsKey(key))
                {
                    int count = _datas[key].GetCount() - maxCount;
                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            result.Add(_datas[key].Dequeue(out isSuccessed));
                        }
                    }
                }
            }
            return result;
        }
    }
}
