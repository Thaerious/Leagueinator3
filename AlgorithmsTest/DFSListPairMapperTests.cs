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
    [TestClass]
    public class DFSListPairMapperTests {
        [TestMethod]
        public void TestBasicAssignment() {
            List<string> keys = ["A", "B", "C", "D"];

            var mapGenerator = new DFSListPairMapper<string>();
            Dictionary<string, string> map = mapGenerator.GenerateMap(keys);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");

            Assert.IsNotNull(map);
            Assert.AreEqual(2, map.Count);
            Assert.AreEqual(map["A"], "B");
            Assert.AreEqual(map["C"], "D");
        }

        [TestMethod]
        public void EmptyList() {
            List<string> keys = [];

            var mapGenerator = new DFSListPairMapper<string>();
            Dictionary<string, string> map = mapGenerator.GenerateMap(keys);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");

            Assert.IsNotNull(map);
            Assert.AreEqual(0, map.Count);
        }

        [TestMethod]
        public void TestMoreValues() {
            List<string> keys = ["A", "B", "C"];
            List<int> values = [1, 2, 3, 4];

            var mapGenerator = new DFSListToListMapper<string, int>();
            Dictionary<string, int> map = mapGenerator.GenerateMap(keys, values);

            Assert.IsNotNull(map);
            Assert.AreEqual(3, map.Count);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");
        }

        [TestMethod]
        public void TestInvalidPairs() {
            MultiMap<string, string> blacklist = new() {
                ["A"] = ["B", "C"]
            };

            List<string> keys = ["A", "B", "C", "D"];

            var mapGenerator = new DFSListPairMapper<string>();
            Dictionary<string, string> map = mapGenerator.GenerateMap(keys, blacklist);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");

            Assert.IsNotNull(map);
            Assert.AreEqual(2, map.Count);
            Assert.AreEqual(map["A"], "D");
            Assert.AreEqual(map["B"], "C");
        }

        [TestMethod]
        [ExpectedException(typeof(UnsolvedException))]
        public void NoValidMap() {
            MultiMap<string, string> blacklist = new() {
                ["A"] = ["B", "C", "D"]
            };

            List<string> keys = ["A", "B", "C", "D"];

            var mapGenerator = new DFSListPairMapper<string>();
            Dictionary<string, string> map = mapGenerator.GenerateMap(keys, blacklist);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");
        }


        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void InvalidNumberOfKeys() {
            MultiMap<string, string> blacklist = new() {
                ["A"] = ["B", "C"]
            };

            List<string> keys = ["A", "B", "C", "D", "E"];

            var mapGenerator = new DFSListPairMapper<string>();
            Dictionary<string, string> map = mapGenerator.GenerateMap(keys, blacklist);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");
        }

        [TestMethod]
        public void MaintainOrder() {
            MultiMap<string, string> blacklist = new() {
                ["B"] = ["A", "D", "E"]
            };

            List<string> keys = ["A", "B", "C", "D", "E", "F"];

            var mapGenerator = new DFSListPairMapper<string>();
            Dictionary<string, string> map = mapGenerator.GenerateMap(keys, blacklist);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");

            Assert.AreEqual(map["A"], "C");
            Assert.AreEqual(map["B"], "F");
            Assert.AreEqual(map["D"], "E");
        }
    }
}
