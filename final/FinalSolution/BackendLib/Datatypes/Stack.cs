using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib.Datatypes
{
    internal class Stack<T>
    {
        private List<T> _stack = new();
        public int Size => _stack.Count;

        public Stack(IEnumerable<T> input)
        {
            foreach (var item in input) _stack.Add(item);
        }
        public T Peek() => _stack[_stack.Count - 1];

        public void Push(T item) => _stack.Add(item);

        public T Pop()
        {
            T item = _stack[_stack.Count - 1];
            _stack.RemoveAt(_stack.Count - 1);
            return item;
        }

        public bool IsEmpty() => _stack.Count == 0;
    }
}
