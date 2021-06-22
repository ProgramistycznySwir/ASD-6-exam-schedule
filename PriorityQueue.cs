using System;
using System.Collections.Generic;
using System.Text;

namespace ASD___6
{
    /// <typeparam name="T">Item with int indicating priority.</typeparam>
    class PriorityQueue<T> where T : IItemWithPriority<T>
    {
        // Można by zamiast tego użyć bst z poprzedniego zadania, ale byłby to overkill, ponieważ potrzebujemy struktury która
        //  jedynie utrzymuje pierwszy element jako element o najniższym priorytecie.

        public readonly int heapCapacity;
        T[] heap;
        int _count;
        public int count => _count;

        public bool IsEmpty => (_count == 0);

        // Ściągawka:
        //  parent = (n-1)/2
        //  left = 2*n+1
        //  right = 2*n+2

        public PriorityQueue(int heapCapacity)
        {
            this.heapCapacity = heapCapacity;
            heap = new T[heapCapacity];
        }

        public void Add(T addedItem)
        {
            int addedItemIndex = _count++;
            heap[addedItemIndex] = addedItem;
            while (heap[addedItemIndex].Priority > heap[(addedItemIndex - 1) / 2].Priority)
            {
                Swap(addedItemIndex, (addedItemIndex - 1) / 2);
                addedItemIndex = (addedItemIndex - 1) / 2;
            }
        }
        public T Peek => heap[0];
        public T Poll()
        {
            if (IsEmpty)
                throw new System.NullReferenceException();
            T mostPriorityItem = heap[0];
            heap[0] = heap[--_count];

            int currentItemIndex = 0;
            int left, right, swapIndex;
            while (true)
            {
                left = 2 * currentItemIndex + 1;
                right = 2 * currentItemIndex + 2;
                if (left < _count)
                {
                    swapIndex = left;
                    if (right < _count && heap[left].Priority < heap[right].Priority)
                        swapIndex = right;

                    if (heap[currentItemIndex].Priority < heap[swapIndex].Priority)
                        Swap(currentItemIndex, currentItemIndex = swapIndex);
                    break;
                }
                break;
            }

            return mostPriorityItem;
        }

        /// <summary>
        /// Takes two indexes to swap in heap;
        /// </summary>
        void Swap(int a, int b)
        {
            T temp = heap[a];
            heap[a] = heap[b];
            heap[b] = temp;
        }
    }


    public interface IItemWithPriority<T>
    {
        public int Priority { get; }
    }
}
