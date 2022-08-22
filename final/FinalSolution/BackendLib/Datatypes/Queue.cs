using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib.Datatypes
{
    internal class Queue<T>
    {
        private List<T> _queue = new();
        public int Size => _queue.Count;

        public Queue(IEnumerable<T> input)
        {
            foreach (var item in input) _queue.Add(item);
        }

        public void Enqueue(T item) => _queue.Add(item);

        public T Dequeue()
        {
            T item = _queue[0];
            _queue.RemoveAt(0);
            return item;
        }

        public bool IsEmpty() => _queue.Count == 0;
    }
}