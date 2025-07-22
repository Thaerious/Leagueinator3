namespace Leagueinator.GUI.Utility {
    public class DefaultDictionary<K, V> : Dictionary<K, V> where K : notnull {

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
