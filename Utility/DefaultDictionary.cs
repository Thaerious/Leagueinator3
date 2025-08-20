namespace Utility {
    public class DefaultDictionary<K, V> : Dictionary<K, V> where K : notnull {

        public DefaultDictionary() {
            var ctor = typeof(V).GetConstructor(Type.EmptyTypes) 
                     ?? throw new NotSupportedException("Must have a no-arg constructor.");

            this.Generator = (_) => Activator.CreateInstance<V>();
        }

        public DefaultDictionary(V value) {
            this.Generator = (_) => value;
        }

        public DefaultDictionary(Func<K, V> generator) {
            this.Generator = generator;
        }

        public Func<K, V> Generator { get; }

        public new V this[K key] {
            get {
                if (!this.ContainsKey(key)) {
                    this[key] = Generator(key);
                }
                return base[key];
            }
            set {
                base[key] = value;
            }
        }
    }
}
