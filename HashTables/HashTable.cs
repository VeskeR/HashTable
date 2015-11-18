using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    public class HashTable<TKey, TValue, TContainer> : IDictionary<TKey, TValue> where TContainer : class, IDictionary<TKey, TValue>, new()
    {
        private const double FillFactor = 0.75;

        private HashTableArray<TKey, TValue, TContainer> _array;
        private int _count;
        private int _maxItemsAtCurrentCapacity;
        private Func<TKey, int> _hashFunction; 

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
            get { return _array[key]; }
            set { _array[key] = value; }
        }

        public ICollection<TKey> Keys
        {
            get { return _array.Keys; }
        }
        public ICollection<TValue> Values
        {
            get { return _array.Values; }
        }

       
        public HashTable()
            :this (1000)
        {
            
        }
        public HashTable(int capacity)
            :this(capacity, null, null)
        {
            
        }
        public HashTable(int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection)
            :this(capacity, collection, null)
        {
            
        }
        public HashTable(int capacity, Func<TKey, int> hashFunction)
            : this(capacity, null, hashFunction)
        {
            
        }
        public HashTable(int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, int> hashFunction)
        {
            if (capacity < 1)
                throw new ArgumentException("Capacity can not be less than 1");

            _hashFunction = hashFunction;

            _array = new HashTableArray<TKey, TValue, TContainer>(capacity, _hashFunction);
            _count = 0;

            _maxItemsAtCurrentCapacity = (int) (capacity*FillFactor) + 1;

            if (collection != null)
                foreach (KeyValuePair<TKey, TValue> pair in collection)
                    Add(pair);
        }


        private void GrowArray()
        {
            HashTableArray<TKey, TValue, TContainer> largerArray = new HashTableArray<TKey, TValue, TContainer>(_array.Capacity << 1, _hashFunction);

            foreach (KeyValuePair<TKey, TValue> item in _array)
            {
                largerArray.Add(item);
            }

            _array = largerArray;

            _maxItemsAtCurrentCapacity = (int)(_array.Capacity * FillFactor) + 1;
        }


        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }
        public void Add(KeyValuePair<TKey, TValue> pair)
        {
            if (_count >= _maxItemsAtCurrentCapacity)
                GrowArray();

            _array.Add(pair);
            _count++;
        }


        public void Update(TKey key, TValue value)
        {
            _array.Update(key, value);
        }


        public bool ContainsKey(TKey key)
        {
            return _array.ContainsKey(key);
        }
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _array.Contains(item);
        }


        public bool TryGetValue(TKey key, out TValue value)
        {
            return _array.TryGetValue(key, out value);
        }


        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            bool r = _array.Remove(item);
            if (r)
                _count--;

            return r;
        }
        public bool Remove(TKey key)
        {
            bool r = _array.Remove(key);
            if (r)
                _count--;

            return r;
        }


        public void Clear()
        {
            _array.Clear();
            _count = 0;
        }


        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _array.CopyTo(array, arrayIndex);
        }


        public void ChangeHashFunction(Func<TKey, int> hashFunction)
        {
            _hashFunction = hashFunction;

            HashTableArray<TKey, TValue, TContainer> newArray = new HashTableArray<TKey, TValue, TContainer>(_array.Capacity, _hashFunction);

            foreach (KeyValuePair<TKey, TValue> item in _array)
            {
                newArray.Add(item);
            }

            _array = newArray;
        }


        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _array.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
