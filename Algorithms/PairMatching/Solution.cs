using System.Diagnostics;
using Utility.Extensions;

namespace Algorithms.PairMatching {
    public class Solution<T> : IEnumerable<(T, T)> where T : notnull {

        private int _fitness = 0;

        private readonly List<(T, T)> list = [];

        private bool dirty = false;

        public int Fitness { 
            get {
                if (this.dirty) this.Evaluate();
                return this._fitness;
            }
        }

        public bool IsValid { get; private set; } = false;
        
        public Graph<T> Graph { get; private set; }

        public Solution(Graph<T> graph){
            this.Graph = graph;
        }

        public Solution(Graph<T> graph, IEnumerable<(T, T)> source) {
            this.Graph = graph;
            foreach (var pair in source) {
                this.Set(pair);
            }
            this.Evaluate();
        }

        public Solution(Solution<T> source) {
            this.Graph = source.Graph;
            this.list.AddRange(source.list);
            this.IsValid = source.IsValid;
            this._fitness = source.Fitness;
            this.dirty = source.dirty;
        }

        public int Count => this.list.Count;

        public (T, T) this[int i] => this.list[i];

        public Solution<T> Copy() {
            return new(this);
        }

        public override string ToString() {
            string[] array = this.Select(item => item.ToString()).ToArray();
            var names = string.Join("", array);
            return $"{this.Fitness}{(this.IsValid ? "+" : "-")}:[{names}] {this.Count}";
        }

        public List<T> ToList() {
            List<T> list = [];
            foreach (var item in this) {
                list.Add(item.Item1);
                list.Add(item.Item2);
            }
            return list;
        }

        private int Evaluate() {
            this._fitness = 0;
            this.IsValid = true;

            if (this.Count * 2 < this.Graph.Size) {
                this.IsValid = false;
            }

            foreach ((T, T) pair in this) {
                if (!this.Graph.HasEdge(pair.Item1, pair.Item2)) {
                    this.IsValid = false;
                }
                else {
                    var edge = this.Graph.GetEdge(pair.Item1, pair.Item2);
                    this._fitness += edge.Weight;
                }
            }

            this.dirty = false;
            return this.Fitness;
        }

        public (T, T) GetPairContaining(T t) {
            foreach ((T, T) pair in this) {
                if (pair.Item1.Equals(t)) return pair;
                if (pair.Item2.Equals(t)) return pair;
            }
            throw new InvalidOperationException($"Solution does not contain item '{t}'.");
        }

        public int Find(T t) {
            for(int i = 0; i < this.Count; i++) {
                var pair = this[i];
                if (pair.Item1.Equals(t)) return i;
                if (pair.Item2.Equals(t)) return i;
            }
            return -1;
        }

        public void Set((T, T) pair) {
            int i = this.Find(pair.Item1);
            if (i != -1) this.RemoveAt(i);

            int j = this.Find(pair.Item2);
            if (j != -1) this.RemoveAt(j);

            this.list.Add(pair);
            this.dirty = true;
        }

        private void RemoveAt(int i) {
            this.list.RemoveAt(i);
        }

        public void Set(List<T> list) {
            if (list.Count % 2 != 0) throw new ArgumentException("List must have even number of items.");

            for (int i = 0; i < list.Count; i += 2) {
                this.Set((list[i], list[i + 1]));
            }
        }

        public void Set(Graph<T>.Edge edge) {
            this.Set((edge.Item1, edge.Item2));
        }

        public IEnumerator<(T, T)> GetEnumerator() => list.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
