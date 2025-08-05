namespace Leagueinator.GUI.Model {
    public interface IHasParent<T> {
        T Parent { get; }
    }

    public static class ParentExtensions {
        public static U? Ancestor<U>(this object? current) where U : class {
            while (current != null) {
                if (current is U match) return match;

                var type = current.GetType();
                var parentProperty = type.GetProperty("Parent");
                current = parentProperty?.GetValue(current);
            }

            return null;
        }
    }
}
