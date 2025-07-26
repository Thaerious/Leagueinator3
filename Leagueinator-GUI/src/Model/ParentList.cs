namespace Leagueinator.GUI.Model {
    public class ParentList<T> : List<T> where T : IHasParent<ParentList<T>> {
        public new void Add(T item) {
            item.Parent = this;
            base.Add(item);
        }

        public new void AddRange(IEnumerable<T> collection) {
            foreach (var item in collection) {
                item.Parent = this;
            }
            base.AddRange(collection);
        }

        public new void Insert(int index, T item) {
            item.Parent = this;
            base.Insert(index, item);
        }

        public new void InsertRange(int index, IEnumerable<T> collection) {
            foreach (var item in collection) {
                item.Parent = this;
            }
            base.InsertRange(index, collection);
        }

        public new T this[int index] {
            get => base[index];
            set {
                value.Parent = this;
                base[index] = value;
            }
        }
    }
}
