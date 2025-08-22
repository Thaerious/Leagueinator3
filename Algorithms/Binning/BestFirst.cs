using Utility.Collections;
using Utility.Extensions;

namespace Algorithms.Binning {
    public class BestFirst <K> where K : notnull {

        public BestFirst(Dictionary<K, int> ratings, List<int> binSizes) {
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
            // Add one player to each bin, starting at the best
            foreach (BoundedList<K> bin in Bins) {
                var best = SortedKeys.Dequeue();
                bin.Add(best);
            }

            // Fill each bin with the remaining players, starting at the lowest
            foreach (BoundedList<K> bin in Bins) {
                while (!bin.IsFull()) {
                    var lowest = SortedKeys.Pop();
                    bin.Add(lowest);
                }
            }

            return this.Bins;
        }
    }
}
