using System.Diagnostics;
using Utility.Extensions;

namespace Algorithms.PairMatching {

    public class GreedyAugmenting<T> where T : notnull {
        private readonly Graph<T> graph;
        private readonly SortedSet<int> Results = [];

        public GreedyAugmenting(Graph<T> graph) {
            this.graph = graph;
        }

        public Solution<T> Run(int popsize, int gencount) {
            Random rng = new Random(123654789);
            List<Solution<T>> population = [];

            // Build initial solutions from randomly selected initialEdges.
            while (population.Count < popsize) {
                Solution<T> solution = new(graph);
                var initialEdges = graph.Edges.OrderBy(edge => rng.Next()).ToList();
                foreach (var edge in initialEdges) {
                    if (solution.Find(edge.Item1) != -1) continue;
                    if (solution.Find(edge.Item2) != -1) continue;
                    solution.Set(edge);
                }
                var repaired = new RepairSolution<T>(graph, solution).Repair();
                population.Add(repaired);
            }

            var edges = graph.Edges.OrderBy(edge => rng.Next()).ToList();
            int bestGen = 0;
            int bestFit = 0;

            for (int i = 0; i < gencount; i++) {
                List<Solution<T>> next = [];

                foreach (var solution in population) {
                    var copy = solution.Copy();
                    int nextEdge = rng.Next(edges.Count);
                    var edge = edges[nextEdge];
                    copy.Set(edge);
                    var repaired = new RepairSolution<T>(graph, copy).Repair();
                    next.Add(repaired);
                }

                population.AddRange(next);
                population = population.OrderBy(s => s.Fitness).Reverse().Take(popsize).ToList();

                if (population[0].Fitness > bestFit) {
                    bestFit = population[0].Fitness;
                    bestGen = i;
                }
            }
            return population[0];
        }
    }
}
