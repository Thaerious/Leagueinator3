using System.Collections;

namespace Utility.Collections {

    /// <summary>
    /// A dictionary where each key maps to a list of values.
    /// Useful when multiple values can be associated with the same key.
    /// </summary>
    public class MultiMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
        where TKey : notnull {

        private readonly Dictionary<TKey, List<TValue>> _dict = [];

        public IEnumerable<TKey> Keys => this._dict.Keys;

        public List<TValue> this[TKey key] {
            get {
                if (!this._dict.ContainsKey(key)) {
                    this._dict[key] = [];
                }
                return _dict[key];
            }
            set {
                if (value is null) throw new ArgumentNullException(nameof(value));
                _dict[key] = value;               
            }
        }


        /// <summary>Adds a value under the given key.</summary>
        public void Add(TKey key, TValue value) {
            if (!_dict.TryGetValue(key, out var list)) {
                list = new List<TValue>();
                _dict[key] = list;
            }
            list.Add(value);
        }

        /// <summary>Adds a collection of values under the given key.</summary>
        public void AddRange(TKey key, IEnumerable<TValue> values) {
            foreach (var v in values)
                Add(key, v);
        }

        /// <summary>Removes a single value from the key’s list.</summary>
        public bool Remove(TKey key, TValue value) {
            if (_dict.TryGetValue(key, out var list)) {
                bool removed = list.Remove(value);
                if (list.Count == 0) _dict.Remove(key);
                return removed;
            }
            return false;
        }

        /// <summary>
        /// True if the key-value pair exists.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Has(TKey key, TValue value) {
            if (!this.ContainsKey(key)) return false;
            return this[key].Contains(value);
        }

        /// <summary>Removes all values for a key.</summary>
        public bool RemoveAll(TKey key) => _dict.Remove(key);

        /// <summary>Checks if a key has any values.</summary>
        public bool ContainsKey(TKey key) => _dict.ContainsKey(key);

        /// <summary>Enumerates all key → value-list pairs.</summary>
        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator() => _dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
