using System.Windows.Controls;

namespace Leagueinator.GUI.Controls {

    /// <summary>
    /// A button that has an object associated with it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataButton<T>() : Button {
        public T? Data { get; set; } = default;
    }
}
