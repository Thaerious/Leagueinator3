
using System.Diagnostics;
using Utility.Extensions;

namespace Algorithms.PairMatching {
    public class ImproveSolution<T> where T : notnull {

        private HashSet<T> Covered = [];
        private HashSet<T> Uncovered = [];

        public int PathCount { get; }
        public Graph<T> Graph { get; }
        public Solution<T> Solution { get; }
        public Solution<T>? RepairedSolutions { get; private set; } = null;

        public ImproveSolution(Graph<T> graph, Solution<T> solution, int pathCount = 10) {
            if (graph.Size % 2 != 0) throw new ArgumentException("Graph must have even number of nodes.");

            this.PathCount = pathCount;
            this.Graph = graph;
            this.Solution = solution.Copy();
            this.UpdateMetrics();
        }

        private void UpdateMetrics() {
            this.Covered = [];
            this.Uncovered = [];

            foreach ((T, T) pair in this.Solution) {
                Covered.Add(pair.Item1);
                Covered.Add(pair.Item2);
            }

            foreach (T key in this.Graph.Keys) {
                if (!this.Covered.Contains(key)) {
                    this.Uncovered.Add(key);
                }
            }
        }

        /// <summary>
        /// Find a pair to a given (root) node, changing the other pairings as neccessary.
        /// 
        /// Starting with a root node, perform DFS:
        /// - If depth is even 
        ///  |- If there is no uncovered edge return failure
        ///  |- Choose an uncovered edge
        ///  |- If neighbor is uncovered, add edge to solution and return success
        ///  |- If neighbor is covered transition to neighbor
        /// - If depth is odd
        ///  |- Transition to covered neighbor
        /// </summary>
        /// <returns></returns>
        public List<List<T>> GetPaths(T root) {
            if (!this.Uncovered.Contains(root)) throw new InvalidOperationException("Root node must be uncovered");

            List<List<T>> paths = [];
            Stack<(T node, int depth)> dfs = new([(root, 0)]);
            HashSet<T> visited = [];
            List<T> path = [];

            while (dfs.Count > 0 && paths.Count < 10) {
                if (path.Count > this.Graph.Size) throw new Exception("Sanity Check Failed");
                var (current, depth) = dfs.Pop();

                //Debug.WriteLine($"[A]  {depth} {path.JoinString("-")}:{current} <- {dfs.JoinString()} [{visited.JoinString()}]");

                // Backtrack
                while (path.Count > depth) {
                    visited.Remove(path[^1]);
                    path.RemoveAt(path.Count - 1);
                }

                visited.Add(current);
                path.Add(current);

                //Debug.WriteLine($"[B]  {depth} {path.JoinString("-")}:{current} <- {dfs.JoinString()} [{visited.JoinString()}]");

                if (depth % 2 == 0) {
                    var edges = this.Graph.GetNode(current).Edges().OrderBy(edge => edge.Weight);

                    // push covered neighbors onto the stack, if no uncovered edge exists
                    foreach (Graph<T>.Edge edge in edges) {
                        var neighbor = edge.NeighborOf(current);
                        if (visited.Contains(neighbor)) continue;

                        if (this.Uncovered.Contains(neighbor)) {
                            path.Add(neighbor);
                            //Debug.WriteLine($"[C1] {depth} {path.JoinString("-")}:{current} <- {dfs.JoinString()} [{visited.JoinString()}]");
                            //Debug.WriteLine($"Found Path: {path.JoinString()}");
                            paths.Add([.. path]);
                        }
                        else {
                            //Debug.WriteLine($"[C2] {depth} {path.JoinString("-")}:{current} <- {dfs.JoinString()} [{visited.JoinString()}]");
                            dfs.Push((neighbor, depth + 1));
                        }
                    }
                }
                else {
                    // All nodes here should be covered
                    var pair = this.Solution.GetPairContaining(current);
                    var edge = this.Graph.GetEdge(pair);
                    var neighbor = edge.NeighborOf(current);

                    if (!visited.Contains(neighbor)) {
                        dfs.Push((neighbor, depth + 1));
                    }
                }

                //Debug.WriteLine($"[D]  {depth} {path.JoinString("-")}:{current} <- {dfs.JoinString()} [{visited.JoinString()}]");
            }

            return paths;
        }

        public Solution<T> Repair() {
            this.UpdateMetrics();

            while (this.Uncovered.Count > 0) {
                var paths = this.GetPaths(this.Uncovered.First());

                //foreach (var path in paths) {
                    //Debug.WriteLine($"Candidate path: {EvaluatePath(path)}:{path.JoinString()}");
                //}

                var chosenPath = paths.OrderBy(p => this.EvaluatePath(p)).Reverse().First();

                //Debug.WriteLine($"Applying Path: {chosenPath.JoinString()}");
                this.Solution.Set(chosenPath);
                this.UpdateMetrics();
            }

            return this.Solution;
        }

        private int EvaluatePath(List<T> path) {
            Debug.WriteLine(path.JoinString());
            int delta = 0;

            for (int i = 0; i < path.Count - 1; i++) {
                int weight = this.Graph.GetEdge((path[i], path[i + 1])).Weight;

                if (i % 2 == 0) delta += weight;
                else delta -= weight;
            }

            return delta;
        }
    }
}
