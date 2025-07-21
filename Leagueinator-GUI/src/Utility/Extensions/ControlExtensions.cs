using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Leagueinator.GUI.Utility.Extensions {
    public static class ControlExtensions {

        /// <summary>
        /// Finds all descendant <see cref="FrameworkElement"/>s of the specified root that have a matching <see cref="FrameworkElement.Tag"/>.
        /// </summary>
        /// <param name="root">The root element to start the search from.</param>
        /// <param name="tag">The tag value to match.</param>
        /// <returns>An <see cref="IEnumerable{FrameworkElement}"/> containing all matching elements.</returns>
        public static IEnumerable<FrameworkElement> FindByTag(this DependencyObject root, string tag) {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++) {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is FrameworkElement frameworkElement && frameworkElement.HasTag(tag))
                    yield return frameworkElement;

                foreach (var match in FindByTag(child, tag))
                    yield return match;
            }
        }

        /// <summary>
        /// Checks whether a <see cref="FrameworkElement"/> has a specific tag among space-separated tag values.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <param name="tag">The tag to look for.</param>
        /// <returns><c>true</c> if the tag is found; otherwise, <c>false</c>.</returns>
        public static bool HasTag(this FrameworkElement element, string tag) {
            if (element is null) return false;
            if (element.Tag is null) return false;
            if (element.Tag is not string tagString) return false;

            List<string> tags = [.. tagString.Split(' ')];
            return tags.Contains(tag);
        }

        /// <summary>
        /// Retrieves all descendant elements of a given type from the visual tree, starting from the specified root.
        /// </summary>
        /// <typeparam name="T">The type of descendant to retrieve.</typeparam>
        /// <param name="root">The root element from which to begin the search.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of matching elements.</returns>
        public static IEnumerable<T> GetDescendantsOfType<T>(this DependencyObject root) where T : DependencyObject {
            ArgumentNullException.ThrowIfNull(root);

            Queue<DependencyObject> queue = new();
            queue.Enqueue(root);

            while (queue.Count > 0) {
                DependencyObject current = queue.Dequeue();
                int childrenCount = VisualTreeHelper.GetChildrenCount(current);

                for (int i = 0; i < childrenCount; i++) {
                    var child = VisualTreeHelper.GetChild(current, i);
                    if (child is T match) yield return match;
                    queue.Enqueue(child);
                }
            }
        }

        /// <summary>
        /// Retrieves all ancestor elements of a given type from the visual tree, starting from the specified element and walking upward.
        /// </summary>
        /// <typeparam name="T">The type of ancestor to retrieve.</typeparam>
        /// <param name="child">The starting element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of matching ancestors.</returns>
        public static IEnumerable<T> Ancestors<T>(this DependencyObject child) where T : DependencyObject {
            DependencyObject current = VisualTreeHelper.GetParent(child);

            while (current != null) {
                if (current is T match) yield return match;
                current = VisualTreeHelper.GetParent(current);
            }
        }

        public static MenuItem? FindMenuItemByHeader(this Menu menu, string header) {
            return FindMenuItemByHeader(menu.Items, header);
        }

        private static MenuItem? FindMenuItemByHeader(ItemCollection items, string header) {
            foreach (var item in items) {
                if (item is MenuItem menuItem) {
                    if (menuItem.Header?.ToString() == header)
                        return menuItem;

                    var found = FindMenuItemByHeader(menuItem.Items, header);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }
    }
}
