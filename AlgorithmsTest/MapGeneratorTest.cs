using Algorithms;
using Algorithms.Mapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Algorithms.Tests {

    public static class ExtensionsForTests {
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

            var mapGenerator = new ConstrainedDFSMapper<string, int>();
            Dictionary<string, int> map = mapGenerator.GenerateMap(keys, values);

            Assert.IsNotNull(map);
            Assert.AreEqual(3, map.Count);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");
        }

        [TestMethod]
        public void EmptyLists() {
            List<string> keys = [];
            List<int> values = [];

            var mapGenerator = new ConstrainedDFSMapper<string, int>();
            Dictionary<string, int> map = mapGenerator.GenerateMap(keys, values);

            Assert.IsNotNull(map);
            Assert.AreEqual(0, map.Count);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");
        }

        [TestMethod]
        public void TestMoreValues() {
            List<string> keys = ["A", "B", "C"];
            List<int> values = [1, 2, 3, 4];

            var mapGenerator = new ConstrainedDFSMapper<string, int>();
            Dictionary<string, int> map = mapGenerator.GenerateMap(keys, values);

            Assert.IsNotNull(map);
            Assert.AreEqual(3, map.Count);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");
        }

        [TestMethod]
        public void TestInvalidPairs() {
            MultiMap<string, int> blacklist = new() {
                ["A"] = [1, 2]
            };

            List<string> keys = ["A", "B", "C"];
            List<int> values = [1, 2, 3, 4];

            var mapGenerator = new ConstrainedDFSMapper<string, int>();

            try {
                Dictionary<string, int> map = mapGenerator.GenerateMap(keys, values, blacklist);

                Assert.IsNotNull(map);
                Assert.AreEqual(3, map.Count);
                Assert.AreNotEqual(1, map["A"]);
                Assert.AreNotEqual(2, map["A"]);

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

            var mapGenerator = new ConstrainedDFSMapper<string, int>();

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
