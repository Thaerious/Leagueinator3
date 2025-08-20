using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Extensions;

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
    public class MapGenerator<K, V> where K : notnull {

        private Dictionary<K, V> _map = [];
        public Dictionary<K, V> Map => new(_map);

        private List<K> _remainingKeys = [];
        public IReadOnlyList<K> RemainingKeys => _remainingKeys;

        private List<V> _remainingValues = [];
        public IReadOnlyList<V> RemainingValues => _remainingValues;

        private Func<K, V, bool> IsValid = (k, v) => true;

        public Dictionary<K, V> GenerateMap(List<K> keys, List<V> values) => this.GenerateMap(keys, values, null);

        public Dictionary<K, V> GenerateMap(List<K> keys, List<V> values, Func<K, V, bool>? isValid = default) {
            if (keys.Count > values.Count) throw new ArgumentException("Not enough values to map all keys.");
            this.IsValid = isValid ?? this.IsValid;
            this._remainingKeys = [.. keys];
            this._remainingValues = [.. values];

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
