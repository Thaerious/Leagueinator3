namespace Algorithms.Tests {
    [TestClass]
    public class AssignValuesTests {

        [TestMethod]
        public void TestBasicAssignment() {
            var input = new Dictionary<string, List<int>> {
                ["A"] = [1],
                ["B"] = [2]
            };

            var assigner = new AssignValues<string, int>(input);
            var result = assigner.Run();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result["A"]);
            Assert.AreEqual(2, result["B"]);
        }

        [TestMethod]
        public void TestNoValidAssignment() {
            var input = new Dictionary<string, List<int>> {
                ["A"] = [1],
                ["B"] = [1]
            };

            var assigner = new AssignValues<string, int>(input);
            var result = assigner.Run();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestEmptyInput() {
            var input = new Dictionary<string, List<int>>();
            var assigner = new AssignValues<string, int>(input);
            var result = assigner.Run();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestMultipleValidAssignments() {
            var input = new Dictionary<string, List<int>> {
                ["A"] = [1, 2],
                ["B"] = [2, 1]
            };

            var assigner = new AssignValues<string, int>(input);
            var result = assigner.Run();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreNotEqual(result["A"], result["B"]);
        }
    }
}
