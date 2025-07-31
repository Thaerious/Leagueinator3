using System.Linq;

namespace Algorithms {

    public class Graph<T> where T : notnull {
        public record Node : IComparable<Node>{
            public Node(T t, Graph<T> graph) {
                this.Item = t;
                this.Graph = graph;
            }

            public T Item { get; }

            public Graph<T> Graph { get; }

            public IEnumerable<Edge> Edges() {
                return this.Graph._edges[this.Item].Values;
            }

            public override string ToString() => this.Item?.ToString() ?? "";

            public int CompareTo(Node? other) {
                if (other is null) return 1;
                if (Item is IComparable<T> comparable) return comparable.CompareTo(other.Item);
                throw new InvalidOperationException($"Type {typeof(T)} must implement IComparable<T>.");
            }
        }

        public record Edge {
            public Edge(Graph<T> graph, T item1, T item2, int weight) {
                this.Graph = graph;
                this.Item1 = item1;
                this.Item2 = item2;
                this.Weight = weight;
            }

            public Graph<T> Graph { get; }

            public T Item1 { get; }

            public T Item2 { get; }

            public int Weight { get; }

            public T NeighborOf(T t) {
                if (t.Equals(this.Item1)) return this.Item2;
                if (t.Equals(this.Item2)) return this.Item1;
                throw new InvalidOperationException("Node must be incident on Edge");
            }
        }

        private readonly Dictionary<T, Dictionary<T, Edge>> _edges = [];
        private readonly Dictionary<T, Node> _nodes = [];

        public int Size => this.Nodes.Count;

        public IEnumerable<Edge> Edges {
            get {
                foreach (var outer in this._edges) {
                    foreach (var inner in outer.Value) {
                       yield return inner.Value;
                    }
                }
            }
        }

        public HashSet<Node> Nodes => [..this._nodes.Values];

        public HashSet<T> Keys => [..this._nodes.Keys];

        public Node GetNode(T item) => this._nodes[item];

        public void AddEdge(T from, T to, int weight) {
            this._edges.TryAdd(from, []);
            this._edges.TryAdd(to, []);
            this._edges[from][to] = new(this, from, to, weight);
            this._edges[to][from] = new(this, to, from, weight);
            this._nodes[from] = new(from, this);
            this._nodes[to] = new(to, this);
        }

        public void RemoveEdge(T from, T to) {
            this._edges[from].Remove(to);  
            this._edges[to].Remove(from);
            if (this._edges[from].Count == 0) this._nodes.Remove(from);
            if (this._edges[to].Count == 0) this._nodes.Remove(to);
        }

        public bool HasEdge(T first, T second) {
            if (this._edges.ContainsKey(first)) {
                if (this._edges[first].ContainsKey(second)) {
                    return true;
                }
            }
            return false;
        }

        public Edge GetEdge(T first, T second) {
            return this._edges[first][second];
        }

        public Edge GetEdge((T, T) pair) {
            try {
                return this._edges[pair.Item1][pair.Item2];
            }
            catch (KeyNotFoundException ex) {
                throw new KeyNotFoundException($"Edge ({pair.Item1},{pair.Item2}) not found.", ex);
            }
        }

        public string ToCSV() {
            List<string> lines = [];

            foreach (Edge edge in this.Edges) {
                lines.Add($"{edge.Item1},{edge.Item2},{edge.Weight}");
            }

            return string.Join(Environment.NewLine, lines);
        }

        public static Graph<T> FromCSV(string csv, Func<string, T>? parseKey = null) {
            parseKey ??= ParseKeyString<T>;

            var graph = new Graph<T>();
            var lines = csv.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines) { // Skip header
                var parts = line.Split(',');

                if (parts.Length != 3)
                    throw new FormatException($"Invalid line: {line}");

                T from = parseKey(parts[0].Trim());
                T to = parseKey(parts[1].Trim());
                if (!int.TryParse(parts[2], out int weight))
                    throw new FormatException($"Invalid weight: {parts[2]}");

                graph.AddEdge(from, to, weight);
            }

            return graph;
        }

        public static U ParseKeyString<U>(string s) {
            if (typeof(U) == typeof(string)) {
                return (U)(object)s.Trim();
            }
            throw new NotSupportedException($"No default parser for type {typeof(U)}.");
        }
    }
}

