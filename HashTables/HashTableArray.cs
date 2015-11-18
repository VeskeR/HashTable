using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    class HashTableArray<TKey, TValue, TContainer> : IDictionary<TKey, TValue> where TContainer : class, IDictionary<TKey, TValue>, new()
    {
        private TContainer[] _array;
        private int _count;
        private Func<TKey, int> _hashFunction; 


        public int Capacity
        {
            get { return _array.Length; }
        }
        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                TryGetValue(key, out value);
                return value;
            }
            set { Update(key, value); }
        }

        public ICollection<TKey> Keys
        {
            get { return _array.Where(nodes => nodes != null).SelectMany(nodes => nodes.Keys).ToList(); }
        }
        public ICollection<TValue> Values
        {
            get { return _array.Where(nodes => nodes != null).SelectMany(nodes => nodes.Values).ToList(); }
        }
        

        public HashTableArray(int capacity)
            :this(capacity, null)
        {
            
        }
        public HashTableArray(int capacity, Func<TKey, int> hashFunction)
        {
            _array = new TContainer[capacity];
            _count = 0;

            if (hashFunction != null)
                _hashFunction = hashFunction;
            else
                _hashFunction = key => key.GetHashCode();
        }


        private int Hash(TKey key)
        {
            return Math.Abs(_hashFunction(key))%Capacity;
        }


        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            int index = Hash(item.Key);

            TContainer nodes = _array[index];

            if (nodes == null)
            {
                nodes = new TContainer();
                _array[index] = nodes;
            }

            nodes.Add(item);
            _count++;
        }


        public void Update(TKey key, TValue value)
        {
            TContainer nodes = _array[Hash(key)];

            if (nodes == null)
                throw new ArgumentException("Key not found", "key");

            nodes[key] = value;
        }


        public bool ContainsKey(TKey key)
        {
            TContainer nodes = _array[Hash(key)];

            return nodes != null && nodes.ContainsKey(key);
        }
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TContainer nodes = _array[Hash(item.Key)];

            return nodes != null && nodes.Contains(item);
        }


        public bool TryGetValue(TKey key, out TValue value)
        {
            TContainer nodes = _array[Hash(key)];

            if (nodes != null)
            {
                return nodes.TryGetValue(key, out value);
            }

            value = default(TValue);
            return false;
        }


        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            TContainer nodes = _array[Hash(item.Key)];

            if (nodes != null && nodes.Remove(item))
            {
                _count--;
                return true;
            }

            return false;
        }
        public bool Remove(TKey key)
        {
            TContainer nodes = _array[Hash(key)];

            if (nodes != null && nodes.Remove(key))
            {
                _count--;
                return true;
            }

            return false;
        }


        public void Clear()
        {
            _array = new TContainer[4];
            _count = 0;
        }


        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                array[arrayIndex++] = pair;
            }
        }


        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _array.Where(nodes => nodes != null).SelectMany(nodes => nodes).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
