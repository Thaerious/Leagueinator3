using Algorithms;
using Algorithms.Mapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.Tests {

    public static class ExtensionsForTests{
        public static bool IsValid(this Dictionary<string, int> dict, string key, int value) {
            bool isValid = true;
            
            if (dict.ContainsKey(key) && dict[key] == value) isValid = false;

            Debug.WriteLine($"isvalid {key} {value} {isValid}");

            return isValid;
        }
    }

    [TestClass]
    public class MapGeneratorTest {
        [TestMethod]
        public void TestBasicAssignment() {
            List<string> keys = ["A", "B", "C"];
            List<int> values = [1, 2, 3];

            var mapGenerator = new MapGenerator<string, int>();
            Dictionary<string, int> map = mapGenerator.GenerateMap(keys, values);

            Assert.IsNotNull(map);
            Assert.AreEqual(3, map.Count);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");
        }

        [TestMethod]
        public void TestMoreValues() {
            List<string> keys = ["A", "B", "C"];
            List<int> values = [1, 2, 3, 4];

            var mapGenerator = new MapGenerator<string, int>();
            Dictionary<string, int> map = mapGenerator.GenerateMap(keys, values);

            Assert.IsNotNull(map);
            Assert.AreEqual(3, map.Count);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");
        }

        [TestMethod]
        public void TestInvalidPairs() {
            Dictionary<string, int> invalid = new() {
                ["A"] = 1
            };

            List<string> keys = ["A", "B", "C"];
            List<int> values = [1, 2, 3, 4];

            var mapGenerator = new MapGenerator<string, int>();

            try {
                Dictionary<string, int> map = mapGenerator.GenerateMap(keys, values, (k, v) => invalid.IsValid(k, v));
                
                Assert.IsNotNull(map);
                Assert.AreEqual(3, map.Count);

                foreach (var (key, val) in map)
                    Debug.WriteLine($"{key} → {val}");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.ToString());

                foreach (var (key, val) in mapGenerator.Map)
                    Debug.WriteLine($"{key} → {val}");
            }
        }

        [TestMethod]
        public void NoValidMap() {
            Dictionary<string, int> invalid = new() {
                ["A"] = 1,
                ["B"] = 1,
                ["C"] = 1
            };

            List<string> keys = ["A", "B", "C"];
            List<int> values = [1, 2, 3];

            var mapGenerator = new MapGenerator<string, int>();

            try {
                Dictionary<string, int> map = mapGenerator.GenerateMap(keys, values, (k, v) => invalid.IsValid(k, v));

                Assert.IsNotNull(map);
                Assert.AreEqual(3, map.Count);

                foreach (var (key, val) in map)
                    Debug.WriteLine($"{key} → {val}");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.ToString());

                foreach (var (key, val) in mapGenerator.Map)
                    Debug.WriteLine($"{key} → {val}");
            }
        }
    }
}
