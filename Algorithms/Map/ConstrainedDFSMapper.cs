using System.Diagnostics;
using Utility;

namespace Algorithms.Mapper {

    class Node<K, V> where K : notnull {
        public K Key { get; }
        public V? Value { get; }

        public Node<K, V>? Parent = default;

        public List<(K, V)> Visited = [];

        public Node(K key, V value, Node<K, V>? parent) {
            this.Key = key;
            this.Value = value;
            this.Parent = parent;
        }
    }

    /// <summary>
    /// Given two lists, a Key-List and a Value-List find a mapping of all items from the Key-List
    /// to a unique item from the Value-List while adhering to contraints.
    /// </summary>
    public class ConstrainedDFSMapper<K, V> where K : notnull {

        private Dictionary<K, V> _map = [];
        public Dictionary<K, V> Map => new(_map);

        private List<K> _remainingKeys = [];
        public IReadOnlyList<K> RemainingKeys => _remainingKeys;

        private List<V> _remainingValues = [];
        public IReadOnlyList<V> RemainingValues => _remainingValues;

        private Func<K, V, bool> IsValid = (k, v) => true;

        public Dictionary<K, V> GenerateMap(IEnumerable<K> keys, IEnumerable<V> values) => this.GenerateMap(keys, values, (k, v) => true);

        public Dictionary<K, V> GenerateMap(IEnumerable<K> keys, IEnumerable<V> values, MultiMap<K, V> blackList) {
            return this.GenerateMap(keys, values, (k, v) => !blackList.Has(k, v));
        }

        public Dictionary<K, V> GenerateMap(IEnumerable<K> keys, IEnumerable<V> values, Func<K, V, bool> isValid) {
            this.IsValid = isValid;
            this._remainingKeys = [.. keys];
            this._remainingValues = [.. values];
            if (this._remainingKeys.Count > this._remainingValues.Count) throw new ArgumentException("Not enough values to map all keys.");

            if (!TryBuild(null)) {
                throw new UnsolvedException("No valid mapping found.");
            }

            return new(_map);
        }

        private bool TryBuild(Node<K, V>? current) {
            if (_remainingKeys.Count == 0) {
                BuildMapFrom(current);
                return true;
            }

            foreach (var key in _remainingKeys.ToList()) {
                foreach (var val in _remainingValues.ToList()) {
                    if (!IsValid(key, val)) continue;

                    _remainingKeys.Remove(key);
                    _remainingValues.Remove(val);

                    var node = new Node<K, V>(key, val, current);
                    if (TryBuild(node)) return true;

                    // backtrack
                    _remainingKeys.Add(key);
                    _remainingValues.Add(val);
                }
            }

            return false;
        }

        private void BuildMapFrom(Node<K, V>? node) {
            _map.Clear();
            while (node is not null) {
                _map[node.Key] = node.Value!;
                node = node.Parent;
            }
        }
    }
}
