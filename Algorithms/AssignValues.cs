using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Extensions;

namespace Algorithms {

    /// <summary>
    /// Assign each Key a Value from the list of permitted values.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class AssignValues<K, V> where K : notnull{
        public AssignValues(Dictionary<K, List<V>> permittedValues) {
            this.Permitted = permittedValues;
        }

        private Dictionary<K, List<V>> Permitted { get; }

        public Dictionary<K, V>? Run() {
            var keys = Permitted.Keys.ToList();
            var used = new HashSet<V>();
            var result = new Dictionary<K, V>();

            // Return true if all keys assigned values
            bool Seek(int index) {
                if (index == keys.Count) return true; // result is complete
                var key = keys[index];

                foreach (V value in this.Permitted[key]) {
                    if (used.Contains(value)) continue;
                    result[key] = value;
                    used.Add(value);

                    if (Seek(index + 1)) return true; // recurse for each key

                    // Backtrack
                    used.Remove(value);
                    result.Remove(key);
                }
                return false;
            }

            return Seek(0) ? result : null;
        }

    }
}
