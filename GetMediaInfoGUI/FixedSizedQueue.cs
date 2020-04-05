using System.Collections.Generic;

namespace GetMediaInfoGUI
{
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
                if (value <= 0)
                    return;
                maxSize = value;
            }
        }

        public FixedSizedQueue(int MaxSize)
        {
            this.MaxSize = MaxSize;
        }

        public FixedSizedQueue(int MaxSize, IEnumerable<T> collection)
          : base(collection)
        {
            this.MaxSize = MaxSize;
            while (Count > MaxSize)
                Dequeue();
        }

        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            while (Count > MaxSize)
                Dequeue();
        }
    }
}
