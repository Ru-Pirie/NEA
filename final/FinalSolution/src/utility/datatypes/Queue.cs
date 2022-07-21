using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSolution.src.utility
{
    class Queue
    {
        public class Stack<T>
        {
            private List<T> _queue = new List<T>();

            public int Size
            {
                get { return _queue.Count; }
                private set { }
            }

            public Stack(IEnumerable<T> input)
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
}
