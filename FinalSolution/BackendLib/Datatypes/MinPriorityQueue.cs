using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib.Datatypes
{
    public class MinPriorityQueue<T>
    {
        private List<int> _priorityQueue = new List<int>();
        private List<T> _queue = new List<T>();

        public int Size => _priorityQueue.Count;
        private int _size => _priorityQueue.Count - 1;

        public MinPriorityQueue() { }

        private int Parent(int index) => (index - 1) / 2;
        private int Left(int index) => (2 * index) + 1;
        private int Right(int index) => (2 * index) + 2;

        public void Enqueue(T value, int priority)
        {
            int oldSize = Size;

            _queue.Add(value);
            _priorityQueue.Add(priority);

            while (oldSize != 0 && _priorityQueue[oldSize] < _priorityQueue[Parent(oldSize)])
            {
                Swap(oldSize, Parent(oldSize));
                oldSize = Parent(oldSize);
            }
        }

        public void ChangePriority(T item, int newPriority)
        {
            int index = _queue.FindIndex(i => Equals(i, item));

            if (_priorityQueue[index] > newPriority)
            {
                _priorityQueue[index] = newPriority;

                while (index != 0 && _priorityQueue[index] < _priorityQueue[Parent(index)])
                {
                    Swap(index, Parent(index));
                    index = Parent(index);
                }
            }
            else
            {
                _priorityQueue[index] = newPriority;
                MinifyHeap(index);
            }
        }

        public T Dequeue()
        {
            if (Size == 1)
            {
                T val = _queue[0];
             
                _queue.RemoveAt(0);
                _priorityQueue.RemoveAt(0);
                
                return val;
            }

            T res = _queue[0];

            int oldSize = _size;

            _queue[0] = _queue[oldSize];
            _queue.RemoveAt(oldSize);
            _priorityQueue[0] = _priorityQueue[oldSize];
            _priorityQueue.RemoveAt(oldSize);

            MinifyHeap(0);

            return res;
        }

        private void MinifyHeap(int index)
        {
            int left = Left(index);
            int right = Right(index);

            int smallest = index;

            if (left < Size && _priorityQueue[left] < _priorityQueue[smallest]) smallest = left;
            if (right < Size && _priorityQueue[right] < _priorityQueue[smallest]) smallest = right;
            if (smallest != index)
            {
                Swap(index, smallest);
                MinifyHeap(smallest);
            }
        }

        private void Swap(int indexX, int indexY)
        {
            T tempValue = _queue[indexX];
            _queue[indexX] = _queue[indexY];
            _queue[indexY] = tempValue;

            int tempPriority = _priorityQueue[indexX];
            _priorityQueue[indexX] = _priorityQueue[indexY];
            _priorityQueue[indexY] = tempPriority;
        }

        public bool Contains(T neighbor) => _queue.Contains(neighbor);
    }
}
