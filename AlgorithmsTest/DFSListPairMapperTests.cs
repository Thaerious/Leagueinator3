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
        public void TestInvalidPairs() {
            List<string> keys = ["A", "B", "C", "D"];

            MultiMap<string, string> blacklist = new() {
                ["A"] = ["B", "C"]
            };

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
            List<string> keys = ["A", "B", "C", "D"];

            MultiMap<string, string> blacklist = new() {
                ["A"] = ["B", "C", "D"]
            };

            var mapGenerator = new DFSListPairMapper<string>();
            Dictionary<string, string> map = mapGenerator.GenerateMap(keys, blacklist);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");
        }


        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void InvalidNumberOfKeys() {
            List<string> keys = ["A", "B", "C", "D", "E"];

            MultiMap<string, string> blacklist = new() {
                ["A"] = ["B", "C"]
            };

            var mapGenerator = new DFSListPairMapper<string>();
            Dictionary<string, string> map = mapGenerator.GenerateMap(keys, blacklist);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");
        }

        [TestMethod]
        public void MaintainOrder() {
            List<string> keys = ["A", "B", "C", "D", "E", "F"];

            MultiMap<string, string> blacklist = new() {
                ["B"] = ["A", "D", "E"]
            };

            var mapGenerator = new DFSListPairMapper<string>();
            Dictionary<string, string> map = mapGenerator.GenerateMap(keys, blacklist);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");

            Assert.AreEqual(map["A"], "C");
            Assert.AreEqual(map["B"], "F");
            Assert.AreEqual(map["D"], "E");
        }

        [TestMethod]
        public void BackTrack() {
            List<string> keys = ["A", "B", "C", "D", "E", "F"];

            MultiMap<string, string> blacklist = new() {
                ["E"] = ["D", "F"]
            };

            var mapGenerator = new DFSListPairMapper<string>();
            Dictionary<string, string> map = mapGenerator.GenerateMap(keys, blacklist);

            foreach (var (key, val) in map)
                Debug.WriteLine($"{key} → {val}");

            Assert.AreEqual(map["A"], "B");
            Assert.AreEqual(map["C"], "E");
            Assert.AreEqual(map["F"], "D");
        }
    }
}
