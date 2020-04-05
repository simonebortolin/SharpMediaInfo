using System.Collections.Generic;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Fixed Sized Queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.Queue{T}" />
    public class FixedSizedQueue<T> : Queue<T>
    {
        private int maxSize;

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

        public FixedSizedQueue(int MaxSize) : base()
        {
            this.MaxSize = MaxSize;
        }

        public FixedSizedQueue(int MaxSize, IEnumerable<T> collection) : base(collection)
        {
            this.MaxSize = MaxSize;
            while (Count > MaxSize)
            {
                Dequeue();
            }
        }

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
