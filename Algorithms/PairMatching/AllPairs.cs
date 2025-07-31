namespace Algorithms.PairMatching {
    public class AllPairs {

        public static IEnumerable<List<(T, T)>> Generate<T>(Graph<T> graph) where T : notnull {
            if (graph.Size % 2 != 0) throw new ArgumentException("Graph must have even number of nodes.");

            List<T> keys = [.. graph.Keys];
            var rng = new Random();
            return Generate(graph, keys);

            static IEnumerable<List<(T, T)>> Generate(Graph<T> graph, List<T> remaining) {
                if (remaining.Count == 0) {
                    yield return new List<(T, T)>();
                    yield break;
                }

                T first = remaining[0];
                for (int i = 1; i < remaining.Count; i++) {
                    T second = remaining[i];
                    if (!graph.HasEdge(first, second)) continue;

                    var pair = (first, second);

                    // Build new list without the paired elements
                    List<T> rest = [..remaining];
                    rest.RemoveAt(i);  // Remove second first to preserve index
                    rest.RemoveAt(0);  // Then first

                    foreach (List<(T, T)> subPairs in Generate(graph, rest)) {
                        subPairs.Insert(0, pair);
                        yield return subPairs;
                    }
                }
            }
        }
    }
}
