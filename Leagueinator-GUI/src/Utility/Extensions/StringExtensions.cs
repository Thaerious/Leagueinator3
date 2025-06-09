namespace Leagueinator.GUI.Utility.Extensions {
    public static class StringExtensions {

        public static string WTF<T>(this IEnumerable<T> enumerable, string del = ", ") {
            return string.Join(del, enumerable.Select(t => t?.ToString()).ToArray());
        }

        public static string DelString<T>(this IEnumerable<T> enumerable, string del = ", ") {
            return string.Join(del, enumerable.Select(t => t?.ToString()).ToArray());
        }

        public static bool IsEmpty(this string? str) {
            if (str is null) return true;
            if (str.Equals("")) return true;
            return false;
        }

        /// <summary>
        /// Creates a copy of the string in all lower case with spliters (-, _).
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToFlatCase(this string source) {
            return source.ToLower().Split('_', '-').DelString("");
        }

        /// <summary>
        /// Replaces placeholders within it with corresponding values from a dictionary.
        /// The placeholders are expected to be in the format "${key}", where "key" is a dictionary key.
        /// </summary>
        /// <param name="input">The string containing placeholders to be replaced.</param>
        /// <param name="dictionary">A dictionary where each key corresponds to a placeholder in the input string and its value is the replacement..</param>
        /// <returns>A new string with all placeholders replaced by their corresponding values.</returns>
        public static string Interpolate(this string input, Dictionary<string, string> dictionary) {
            foreach (string key in dictionary.Keys) {
                input = input.Replace("${" + key + "}", dictionary[key]);
            }

            return input;
        }
    }
}
