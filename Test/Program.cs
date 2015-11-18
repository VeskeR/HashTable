using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HashTables;
using MyTreesLib;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rand = new Random();
            Console.WriteLine("Test with count:");
            int[] totalCounts = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
            Console.Clear();

            int repeatCount = 10;
            int searshCount = 500;
            Stopwatch sw = new Stopwatch();
            Stopwatch swAddingTime = new Stopwatch();

            List<string> keys = new List<string>();
            for (int i = 0; i < totalCounts[totalCounts.Length - 1] * 2; i++)
            {
                keys.Add(Guid.NewGuid().ToString());
            }

            HashTable<string, int, Dictionary<string, int>> hashTable =
               new HashTable<string, int, Dictionary<string , int>>(1000000,
                   str => str.Aggregate(5182, (sum, c) => (sum << 4 + sum) + (int)c));

            for (int i = 0; i < totalCounts.Length; i++)
            {
                int added = 0;
                for (int j = 0; j < i; j++)
                {
                    added += totalCounts[j];
                }

                swAddingTime.Start();
                for (int j = 0; j < totalCounts[i] - added; j++)
                {
                    hashTable.Add(keys[added + j], added + j);
                }
                swAddingTime.Stop();
                Console.WriteLine("Adding {0} elements takes:\t\t\t {1} ms", totalCounts[i], swAddingTime.ElapsedMilliseconds);

                sw.Start();
                for (int j = 0; j < repeatCount; j++)
                {
                    hashTable.Add(keys[keys.Count - 1 - j - added], keys.Count - 1 - j - added);
                }
                sw.Stop();
                Console.WriteLine("Adding to {0} elements takes:\t\t\t {1} ms", totalCounts[i], sw.ElapsedMilliseconds / repeatCount);

                sw.Reset();
                sw.Start();
                for (int j = 0; j < repeatCount; j++)
                {
                    for (int k = 0; k < searshCount; k++)
                    {
                        hashTable.Contains(new KeyValuePair<string, int>(keys[i + j + k + added], i + j + k + added));
                    }
                }
                sw.Stop();

                Console.WriteLine("Searching for {0} elements in {1} takes:\t {2} ms", searshCount, totalCounts[i], sw.ElapsedMilliseconds / repeatCount);
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }


    public class AvlTreeDValue<TKey, TValue> : IComparable<AvlTreeDValue<TKey, TValue>> where TKey : IComparable<TKey>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }


        public AvlTreeDValue()
        {
            
        }
        public AvlTreeDValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }


        public int CompareTo(AvlTreeDValue<TKey, TValue> other)
        {
            return Key.CompareTo(other.Key);
        }
    }

    public class AvlTreeD<TKey, TValue> : AvlTree<AvlTreeDValue<TKey, TValue>>, IDictionary<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            base.Add(new AvlTreeDValue<TKey, TValue>(item.Key, item.Value));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return base.Contains(new AvlTreeDValue<TKey, TValue>(item.Key, item.Value));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return base.Remove(new AvlTreeDValue<TKey, TValue>(item.Key, item.Value));
        }

        public bool IsReadOnly { get; private set; }
        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            base.Add(new AvlTreeDValue<TKey, TValue>());
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public TValue this[TKey key]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ICollection<TKey> Keys { get; private set; }
        public ICollection<TValue> Values { get; private set; }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return base.InOrderTraversal().Select(value => new KeyValuePair<TKey, TValue>(value.Key, value.Value)).GetEnumerator();
        }
    }
}
