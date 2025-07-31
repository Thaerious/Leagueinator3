
namespace Utility.Extensions {
    public static class CollectionExtensions {

        public static string JoinString<K, V>(this IEnumerable<KeyValuePair<K, V>> collection, string delim = ",") {
            if (!collection.Any()) return "";
            string[] array = collection.Select(item => $"({item.Key}:{item.Value})").ToArray();
            return string.Join(delim, array);
        }

        public static string JoinString<T>(this IEnumerable<T> collection, string delim = ",", string wrapper = "") {
            if (!collection.Any()) return "";
            string[] array = collection.Select(item => item.ToString()).Select(s => $"{wrapper}{s}{wrapper}").ToArray();
            return string.Join(delim, array);
        }
        
        public static T Dequeue<T>(this List<T> list) {
            T t = list.First();
            list.RemoveAt(0);
            return t;
        }

        public static void Enqueue<T>(this List<T> list, T item) {
            list.Insert(0, item);
        }

        public static T Pop<T>(this List<T> list) {
            T t = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return t;
        }

        public static void Shuffle<T>(this List<T> list, int seed = 0) {
            Random random = new Random(seed);

            int n = list.Count;
            while (n > 1) {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IEnumerable<T> Do<T>(this IEnumerable<T> list, Action<T> action) {
            return list.Select(item => {
                action(item);
                return item;
            });
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> list, int count) {
            if (count == 1) {
                foreach (T t in list) yield return [t];
                yield break;
            }
            else {
                List<T> sublist = [.. list];
                T item = sublist.Dequeue();

                foreach (IEnumerable<T> comb in sublist.Combinations(count - 1)) {
                    yield return new List<T> { item }.Concat(comb);
                }

                foreach (IEnumerable<T> comb in sublist.Combinations(count)) {
                    yield return comb;
                }
            }
        }
    }
}
