﻿using System.Collections.Generic;

namespace BackendLib.Datatypes
{
    public class Queue<T>
    {
        private List<T> _queue = new List<T>();
        public int Size => _queue.Count;

        public Queue() { }

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

        public bool Contains(T item) => _queue.Contains(item);
    }
}