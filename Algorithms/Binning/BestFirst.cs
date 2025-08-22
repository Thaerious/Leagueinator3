using System.Diagnostics;
using Utility.Collections;
using Utility.Extensions;

namespace Algorithms.Binning {
    public class BestFirst <K> where K : notnull {

        public BestFirst(Dictionary<K, int> ratings, List<int> binSizes) {
            if (ratings.Count == 0) throw new Exception("Ratings must have values");

            this.SortedKeys = ratings.Select(kvp => kvp.Key)
                             .OrderByDescending(k => ratings[k])
                             .ToList();            

            foreach (int binSize in binSizes) {
                Bins.Add(new(binSize));
            }
        }

        private List<K> SortedKeys { get; }
        private List<BoundedList<K>> Bins { get; } = [];

        public List<BoundedList<K>> Solve() {
            // Isolate the best players
            List<K> best = [];
            while (best.Count < this.Bins.Count) {
                best.Add(SortedKeys.Dequeue());
            }

            // Randomize the lists
            best.Shuffle();
            SortedKeys.Shuffle();

            Debug.WriteLine($"Best: {best.JoinString()}");

            // Add one player to each bin, starting at the best
            foreach (BoundedList<K> bin in Bins) {
                bin.Add(best.Dequeue());
            }            

            // Fill each bin with the remaining players, starting at the lowest
            foreach (BoundedList<K> bin in Bins) {
                while (!bin.IsFull()) {
                    bin.Add(SortedKeys.Pop());
                }
            }

            return this.Bins;
        }
    }
}
