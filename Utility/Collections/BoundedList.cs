using System.Collections;

namespace Utility.Collections {
    public class BoundedList<T> : IReadOnlyList<T> {
        private readonly List<T> Items = [];
        private readonly int MaxSize;

        public BoundedList(int maxSize) => MaxSize = maxSize;

        public void Add(T item) {
            if (this.IsFull()) {
                throw new InvalidOperationException("List full");
            }
            Items.Add(item);
        }

        public T this[int i] => Items[i];
        public int Count => Items.Count;
        public bool IsFull() => this.Items.Count >= this.MaxSize;

        public IEnumerator<T> GetEnumerator() {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
