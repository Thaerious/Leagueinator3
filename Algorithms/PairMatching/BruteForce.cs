using System.Diagnostics;

namespace Algorithms.PairMatching {

    public class BruteForce<T> where T : notnull {
        private readonly Graph<T> graph;
        private readonly SortedSet<int> Results = [];

        public BruteForce(Graph<T> graph) {
            this.graph = graph;
        }

        public IEnumerable<Solution<T>> Run() {
            if (graph.Size == 0) throw new ArgumentException("Number of inputs must be greater than 0.");
            if (graph.Size % 2 != 0) throw new ArgumentException("Must have an even number of nodes.");
            HashSet<T> keys = graph.Keys;

            foreach (List<(T, T)> pairs in AllPairs.Generate(this.graph)) {
                Solution<T> solution = new(this.graph, pairs);

                if (solution.IsValid) {
                    yield return solution;
                }
            }
        }
    }
}
