using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetStreamService
{
    public class HashQueueCache<T>
    {
        public int Capacity { get; private set; }
        private HashSet<T> _HashSet;
        private Queue<T> _Queue;

        public HashQueueCache(int capacity)
        {
            if (capacity <= 0) throw new ArgumentException("Capacity must be greater then 0.");
            Capacity = capacity;
            _HashSet = new HashSet<T>(Enumerable.Repeat(default(T), capacity));
            _Queue = new Queue<T>(Enumerable.Repeat(default(T), capacity));
        }

        public bool Add(T item)
        {
            if (_HashSet.Contains(item)) return false;

            var lastItem = _Queue.Dequeue();
            _HashSet.Remove(lastItem);

            _Queue.Enqueue(item);
            _HashSet.Add(item);

            return true;
        }
    }
}
