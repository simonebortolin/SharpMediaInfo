using System.Collections.Generic;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Fixed Sized Queue
    /// </summary>
    /// <typeparam name="T">The typeparam</typeparam>
    /// <seealso cref="System.Collections.Generic.Queue{T}" />
    public class FixedSizedQueue<T> : Queue<T>
    {
        private int maxSize;

        /// <summary>
        /// The maximum size of the queue
        /// </summary>
        public int MaxSize
        {
            get
            {
                return maxSize;
            }
            set
            {
                if (value > 0) maxSize = value;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="MaxSize">The maximum size</param>
        public FixedSizedQueue(int MaxSize) : base()
        {
            this.MaxSize = MaxSize;
        }

        /// <summary>
        /// Secondary constructor.
        /// </summary>
        /// <param name="MaxSize">The maximum size</param>
        /// <param name="collection">The collection used to inizialize the queue</param>
        public FixedSizedQueue(int MaxSize, IEnumerable<T> collection) : base(collection)
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
    }
}
