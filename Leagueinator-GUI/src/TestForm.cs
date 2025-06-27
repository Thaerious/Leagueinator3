using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Forms.Print;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Leagueinator.GUI {
    public class TestForm {
        [STAThread]
        public static void Main() {
            var app = new Application();

            string contents = File.ReadAllText("D:\\scratch\\league\\test.league");
            ModelSaveData data = JsonSerializer.Deserialize<ModelSaveData>(contents)
                ?? throw new InvalidDataException("Failed to deserialize the file contents.");

            var roundDataCollection = data.RoundDataCollection;
            var eventData = data.EventData;

            var window = new PrintWindow(roundDataCollection);
            app.Run(window);
        }
    }
}
