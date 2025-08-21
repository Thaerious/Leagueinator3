namespace Algorithms.Mapper {
    class KVNode<K, V> where K : notnull {
        public K Key { get; }
        public V? Value { get; }

        public KVNode<K, V>? Parent = default;

        public KVNode(K key, V value, KVNode<K, V>? parent) {
            this.Key = key;
            this.Value = value;
            this.Parent = parent;
        }
    }
}
