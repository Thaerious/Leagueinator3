using System.Diagnostics;
using Utility;
using Utility.Extensions;

namespace Algorithms.Mapper {

    public class DFSListPairMapper<K> where K : notnull {

        private Dictionary<K, K> _map = [];
        public Dictionary<K, K> Map => new(_map);

        private List<K> _remainingKeys = [];
        public IReadOnlyList<K> RemainingKeys => _remainingKeys;

        private Func<K, K, bool> IsValid = (k1, k2) => true;

        public Dictionary<K, K> GenerateMap(IEnumerable<K> keys) => this.GenerateMap(keys, (k1, k2) => true);

        public Dictionary<K, K> GenerateMap(IEnumerable<K> keys, MultiMap<K, K> blackList) {
            Debug.WriteLine($"Keys: {keys.JoinString()}");
            Debug.WriteLine($"BlackList: {blackList.Select(kvp => $"[{kvp.Key},{kvp.Value}]").JoinString()}");
            return this.GenerateMap(keys, (k1, k2) => !blackList.Has(k1, k2));
        }

        public Dictionary<K, K> GenerateMap(IEnumerable<K> keys, Func<K, K, bool> isValid) {
            this.IsValid = isValid;
            this._remainingKeys = [.. keys];
            if (this._remainingKeys.Count % 2 != 0) throw new PreconditionException("Can only map an even number of keys.");

            if (!TryBuild(null)) {
                throw new UnsolvedException("No valid mapping found.");
            }

            return new(_map);
        }

        private bool TryBuild(KVNode<K, K>? current) {
            if (_remainingKeys.Count == 0) {
                BuildMapFrom(current);
                return true;
            }

            foreach (var key1 in _remainingKeys.ToList()) {
                foreach (var key2 in _remainingKeys.ToList()) {
                    if (key1.Equals(key2)) continue;
                    if (!IsValid(key1, key2)) continue;
                    if (!IsValid(key2, key1)) continue;

                    _remainingKeys.Remove(key1);
                    _remainingKeys.Remove(key2);

                    var node = new KVNode<K, K>(key1, key2, current);
                    this.PrintPath(current);
                    if (TryBuild(node)) return true;

                    // backtrack
                    _remainingKeys.Add(key2);
                    _remainingKeys.Add(key1);

                    Debug.WriteLine("Backtrack:");
                    this.PrintPath(current);
                }
            }

            return false;
        }

        private void PrintPath(KVNode<K, K>? node) {
            List<K> keys = [];

            while (node is not null) {
                keys.Add(node.Key);
                node = node.Parent;
            }

            keys.Reverse();
            Debug.WriteLine($"{keys.JoinString("→")}");
        }

        private void BuildMapFrom(KVNode<K, K>? node) {
            _map.Clear();
            while (node is not null) {
                _map[node.Key] = node.Value!;
                node = node.Parent;
            }
        }
    }
}
