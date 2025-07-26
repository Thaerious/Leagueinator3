namespace Algorithms.PairMatching {
    public class Solution<T> : List<(T, T)> {

        public int Fitness { get; set; }

        public bool IsValid { get; set; }

        public Solution<T> Copy() {
            Solution<T> copy = [.. this];
            copy.Fitness = this.Fitness;
            return copy;
        }

        public override string ToString() {
            string[] array = this.Select(item => item.ToString()).ToArray();
            return string.Join("", array);
        }
    }
}
