using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib.Datatypes
{
    public class PriorityQueue<T>
    {
        private List<int> _priorityQueue = new List<int>();
        private List<T> _queue = new List<T>();

        public int Size => _priorityQueue.Count;
        private int _size => _priorityQueue.Count - 1;

        public PriorityQueue() { }

        private T GetParent(int index) => _queue[Parent(index)];
        private int Parent(int index) => (index - 1) / 2;

        private T GetLeftChild(int index) => _queue[LeftChild(index)];
        private int LeftChild(int index) => (index * 2) + 1;

        private T GetRightChild(int index) => _queue[RightChild(index)];
        private int RightChild(int index) => (index * 2) + 2;

        private void ShiftNodeUp(int index)
        {
            while (index > 0 && _priorityQueue[Parent(index)] < _priorityQueue[index])
            {
                Swap(Parent(index), index);
                index = Parent(index);
            }
        }


        //TODO make all recursion?
        private void ShiftNodeDown(int index)
        {
            int maxIndex = index;

            int left = LeftChild(index);
            if (left <= _size && _priorityQueue[left] > _priorityQueue[maxIndex]) maxIndex = left;

            int right = RightChild(index);
            if (right <= _size && _priorityQueue[right] > _priorityQueue[maxIndex]) maxIndex = right;

            if (index != maxIndex)
            {
                Swap(index, maxIndex);
                ShiftNodeDown(maxIndex);
            }
        }

        public void Enqueue(T item, int priority)
        {
            _queue.Add(item);
            _priorityQueue.Add(priority);

            ShiftNodeUp(_size);
        }

        public T Dequeue() => RemoveMax().Item1;

        private (T, int) RemoveMax()
        {
            int res = _priorityQueue[0];
            T result = _queue[0];
            _priorityQueue.RemoveAt(0);
            _queue.RemoveAt(0);

            ShiftNodeDown(0);

            return (result, res);
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
    }
}
