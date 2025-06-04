using System.Windows;
using System.Windows.Media;

namespace Leagueinator.GUI.Utility.Extensions {
    public static class ControlExtensions {

        public static bool HasTag(this FrameworkElement source, string tag) {
            if (source is null) return false;
            if (source.Tag is null) return false;
            if (source.Tag is not string allTags) return false;

            List<string> split = [.. allTags.Split(" ")];
            return split.Contains(tag);
        }

        /// <summary>
        /// Retrieve all decedents (child elements recursively) that are of type <T>.
        /// </summary>
        /// <typeparam name="T">The type of decedent to retrieve</typeparam>
        /// <param name="parent">Source element</param>
        /// <returns></returns>
        public static IEnumerable<T> Descendants<T>(this DependencyObject parent) where T : DependencyObject {
            ArgumentNullException.ThrowIfNull(parent);

            Queue<DependencyObject> sourceList = new();
            sourceList.Enqueue(parent);

            while (sourceList.Count > 0) {
                DependencyObject current = sourceList.Dequeue();
                int childCount = VisualTreeHelper.GetChildrenCount(current);

                // Loop through each child
                for (int i = 0; i < childCount; i++) {
                    var child = VisualTreeHelper.GetChild(current, i);

                    // Check if the child is of the specified type
                    if (child is T t) yield return t;

                    // Recursively call this method on the child
                    sourceList.Enqueue(child);
                }
            }
        }

        public static IEnumerable<T> Ancestors<T>(this DependencyObject element) where T : DependencyObject {
            DependencyObject parent = VisualTreeHelper.GetParent(element);

            while (parent != null) {
                if (parent is T t) yield return t;
                parent = VisualTreeHelper.GetParent(parent);
            }
        }
    }
}
