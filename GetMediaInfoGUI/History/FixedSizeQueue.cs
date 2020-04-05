using System;
using System.Collections.Generic;
using System.Linq;

namespace GetMediaInfoGUI.History
{
    /// <summary>
    /// Fixed Sized Queue
    /// </summary>
    /// <typeparam name="T">The typeparam</typeparam>
    /// <seealso cref="System.Collections.Generic.Queue{T}" />
    public class FixedSizeQueue<T> : Queue<T>
    {
        /// <summary>
        /// The maximum size of the queue
        /// </summary>
        public int MaxSize { get; set; } = 0;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="MaxSize">The maximum size</param>
        public FixedSizeQueue(int MaxSize) : base()
        {
            this.MaxSize = MaxSize;
        }

        /// <summary>
        /// Secondary constructor.
        /// </summary>
        /// <param name="MaxSize">The maximum size</param>
        /// <param name="collection">The collection used to inizialize the queue</param>
        public FixedSizeQueue(int MaxSize, IEnumerable<T> collection) : base(collection)
        {
            this.MaxSize = MaxSize;
            while (Count > MaxSize)
            {
                Dequeue();
            }
        }

        /// <summary>
        /// Enqueues an item.
        /// </summary>
        /// <param name="item">The item</param>
        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            while (Count > MaxSize)
            {
                Dequeue();
            }
        }

        /// <summary>
        /// Enqueues multiple items.
        /// </summary>
        /// <param name="items">The items to enqueue</param>
        public void Enqueue(params T[] items)
        {
            for (int i = 0; i < items.Length; i++) {
                base.Enqueue(items[i]);
            }        
        }

        /// <summary>
        /// Changes the maximum size of the queue.
        /// </summary>
        /// <param name="size">The size</param>
        public void ChangeMaximumSize(int size)
        {
            this.MaxSize = size;
            while (Count > MaxSize)
            {
                Dequeue();
            }
        }

        /// <summary>
        /// Removes all items that matches the conditions of the predicate.
        /// </summary>
        /// <param name="predicate">The predicate</param>
        public void Remove(Predicate<T> predicate)
        {
            var oldHistory = this.ToList();
            oldHistory.RemoveAll(predicate);
            this.Clear();
            foreach (var item in oldHistory)
            {
                Enqueue(item);
            }
        }
    }
}
