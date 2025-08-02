using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Leagueinator.Utility.Extensions {
    public static class ControlExtensions {

        public static void AddMenuItem(this ContextMenu contextMenu, string header, RoutedEventHandler hnd) {
            var menuItem = new MenuItem { Header = header };
            menuItem.Click += hnd;
            contextMenu.Items.Add(menuItem);
        }

        public static Color ToMediaColor(this System.Drawing.Color color) {
             return System.Windows.Media.Color.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B
             );
        }

        public static MenuItem AddMenuItem(this Menu menu, string[] headers, RoutedEventHandler hnd) {
            if (headers.Length == 0) throw new Exception("Must include at least 1 header");
            ItemsControl current = menu;

            // Traverse down the menu hierarchy by matching headers
            foreach (var header in headers) {
                ItemsControl? next = current.GetMenuItem(header);
                if (next == null) {
                    next = new MenuItem() { Header = header };
                    current.Items.Add(next);
                }
                current = next;
            }

            MenuItem newMenuItem = (MenuItem)current;
            newMenuItem.Click += hnd;
            return newMenuItem;
        }

        /// <summary>
        /// Removes a menu item from a Menu given a header path (e.g., ["View", "ELO"]).
        /// Supports nested menus.
        /// </summary>
        /// <param name="menu">The root Menu to remove from.</param>
        /// <param name="headers">An array representing the menu hierarchy.</param>
        /// <returns>True if the menu item was found and removed; false otherwise.</returns>
        public static bool RemoveMenuItem(this Menu menu, string[] headers) {
            // Find the target MenuItem to remove
            MenuItem? menuItem = menu.GetMenuItem(headers);
            if (menuItem is null) return false;

            if (headers.Length == 1) {
                // Top-level item (e.g., menu.Keys.Remove("Help"))
                menu.Items.Remove(menuItem);
            }
            else {
                // Nested item — get parent and remove child from it
                var parent = menu.GetMenuItem(headers[..^1]);
                parent?.Items.Remove(menuItem);
            }

            return true;
        }

        /// <summary>
        /// Finds a MenuItem by following a path of headers (e.g., ["File", "Export", "PDF"]).
        /// </summary>
        /// <param name="menu">The root Menu to search from.</param>
        /// <param name="headers">An array of header strings representing the menu hierarchy.</param>
        /// <returns>The MenuItem at the end of the path, or null if not found.</returns>
        public static MenuItem? GetMenuItem(this Menu menu, string[] headers) {
            ItemsControl? current = menu;

            // Traverse down the menu hierarchy by matching headers
            foreach (var header in headers) {
                if (current is null) return null;
                current = current.GetMenuItem(header);
            }

            // Final check: must be a MenuItem
            if (current is MenuItem menuItem) return menuItem;
            return null;
        }

        /// <summary>
        /// Finds the first MenuItem under the given ItemsControl with a matching header.
        /// </summary>
        /// <param name="menu">A Menu or MenuItem acting as a container.</param>
        /// <param name="header">The header string to search for.</param>
        /// <returns>The matching MenuItem, or null if not found.</returns>
        public static MenuItem? GetMenuItem(this ItemsControl menu, string header) {
            return menu.Items
                .OfType<MenuItem>()
                .FirstOrDefault(m => m.Header?.ToString() == header);
        }


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
