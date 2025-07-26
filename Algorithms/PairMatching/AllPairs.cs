namespace Algorithms.PairMatching {
    public class AllPairs {
        public static IEnumerable<List<(T, T)>> Generate<T>(List<T> items) {
            if (items.Count % 2 != 0) throw new ArgumentException("List must have even number of elements.");

            return Generate(items);

            static IEnumerable<List<(T, T)>> Generate(List<T> remaining) {
                if (remaining.Count == 0) {
                    yield return new List<(T, T)>();
                    yield break;
                }

                T first = remaining[0];
                for (int i = 1; i < remaining.Count; i++) {
                    T second = remaining[i];
                    var pair = (first, second);

                    // Build new list without the first and second elements
                    List<T> rest = new(remaining);
                    rest.RemoveAt(i);  // Remove second first to preserve index
                    rest.RemoveAt(0);  // Then first

                    foreach (List<(T, T)> subPairs in Generate(rest)) {
                        subPairs.Insert(0, pair);
                        yield return subPairs;
                    }
                }
            }
        }
    }
}
