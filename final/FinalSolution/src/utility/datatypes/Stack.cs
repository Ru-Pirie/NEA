using System.Collections.Generic;

namespace FinalSolution.src.utility.datatypes
{
    public class Stack<T>
    {
        private List<T> _stack = new List<T>();

        public int Size
        {
            get { return _stack.Count; }
            private set { }
        }

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
