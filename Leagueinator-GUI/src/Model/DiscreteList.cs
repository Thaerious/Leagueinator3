
using System.Collections;

namespace Leagueinator.GUI.Model {
    public class DiscreteList<T> : IEnumerable<T> {
        private readonly T[] Contents;
        private readonly T Default;

        public DiscreteList(int size, T @default){
            this.Contents = new T[size];
            this.Default = @default;
            for (int i = 0; i < size; i++) this.Contents[i] = @default;
        }
        
        public DiscreteList(IEnumerable<T> source, T @default) {
            this.Default = @default;
            this.Contents = [.. source];
        }

        public DiscreteList(DiscreteList<T> source) {
            this.Default = source.Default;
            this.Contents = [.. source];
        }

        public int Size => this.Contents.Length;

        /// <summary>
        /// Size non-empty player names.
        /// </summary>
        /// <returns></returns>
        internal int CountValues() => this.Contents.Select(n => n != null).Count();

        public T this[int i] {
            get {
                return Contents[i];
            }
            set {
                Contents[i] = value;
            }
        }

        public override int GetHashCode() {
            int hash = 17;
            foreach (var s in this)
                hash = hash * 31 + (s?.GetHashCode() ?? 0);
            return hash;
        }

        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>)this.Contents).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public override string ToString() => string.Join(", ", Contents);

        public void Remove(T t) {
            for (int i = 0; i < this.Size; i++) {
                if (this.Contents[i].Equals(t)) {
                    this.Contents[i] = this.Default;
                    return;
                }
            }
        }

        /// <summary>
        /// Return a copy of the this as an array.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray() {
            return [.. this.Contents];
        }

        /// <summary>
        /// True is there are no non-empty player names.
        /// </summary>
        /// <returns></returns>
        internal bool IsEmpty() {
            return this.CountValues() == 0;
        }

        public int IndexOf(T t) => Array.IndexOf(this.Contents, t);

        public IEnumerable<T> Intersect(IEnumerable<T> that) {
            return this.Contents.Intersect(that);
        }

        public DiscreteList<T> Copy() {
            return new DiscreteList<T>(this);
        }

        public void Clear() {
            for (int i = 0; i < this.Size; i++) this.Contents[i] = default;
        }

        /// <summary>
        /// Set the next value in this that is empty (this.Default) to the value t.
        /// </summary>
        /// <param name="t"></param>
        public void Add(T t) {
            for (int i = 0; i < this.Size; i++) {
                if (this.Contents[i].Equals(this.Default)) {
                    this.Contents[i] = t;
                    return;
                }
            }
        }
    }
}
