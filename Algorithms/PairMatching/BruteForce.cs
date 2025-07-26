using System.Diagnostics;
using System.Xml.Linq;

namespace Algorithms.PairMatching {

    public class BruteForce<T> where T : notnull {

        public readonly Dictionary<T, Dictionary<T, int>> Weights = [];
        private readonly SortedSet<int> Results = [];
        public Solution<T>? Best { get; private set; } = default;

        public T? UnPaired = default;

        public void AddEdge(T from, T to, int weight) {
            this.Weights.TryAdd(from, []);
            this.Weights.TryAdd(to, []);
            this.Weights[from][to] = weight;
            this.Weights[to][from] = weight;
        }

        // Add a node with a connection to all other nodes with the provided weight.
        public void AddNode(T node, int weight) {
            List<T> nodes = [.. this.Weights.Keys];
            foreach (T t in nodes) {
                this.AddEdge(node, t, weight);
            }
        }

        public void Evaluate(Solution<T> solution) {
            int sum = 0;
            int delta = solution.Count / 2;

            foreach ((T, T) pair in solution){
                var fromVector = this.Weights[pair.Item1];

                if (fromVector.TryGetValue(pair.Item2, out int value)) {
                    sum += value;
                }
                else {
                    solution.IsValid = false;
                    solution.Fitness = 0;
                    return;
                }
            }

            solution.IsValid = true;
            solution.Fitness = sum;
        }

        public Solution<T> Run() {
            if (this.Weights.Keys.Count == 0) throw new ArgumentException("Number of inputs must be greater than 0.");
            if (this.Weights.Keys.Count % 2 != 0) throw new ArgumentException("Must have an even number of nodes.");

            List<T?> nodes = [.. this.Weights.Keys];

            int i = 0;
            foreach (List<(T, T)> pairs in AllPairs.Generate(nodes)) {
                Solution<T> solution = [.. pairs];
                this.Evaluate(solution);

                Debug.WriteLine($"{i}: {solution.IsValid} {solution.Fitness} {solution}");

                if (solution.IsValid) {
                    this.Results.Add(solution.Fitness);
                    if (solution.Fitness == this.Results.Max()) {
                        this.Best = solution.Copy();
                    }
                }
            }
            return this.Best;            
        }
    }
}
