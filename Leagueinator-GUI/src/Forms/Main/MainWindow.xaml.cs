using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Utility;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Forms.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private EventData EventData { get; set; } = new EventData();
        private EventData FileEvent { get; set; } = new EventData();

        public MainWindow() {
            this.InitializeComponent();
            this.Title = "Leagueinator []";
            //this.InstantiateDragDropHnd();
            this.CurrentRoundButton = this.AddRoundButton();
            this.CurrentRoundButton.Focus();            

            this.Loaded += (s, e) => {
                Debug.WriteLine("MainWindow Loaded");
                MainController mainController = new(this, this.EventData);
                this.InvokeRoundButton();
            };
        }

        public void ClearFocus() {
            FocusManager.SetFocusedElement(this, null);
        }

        public MatchCard GetMatchCard(int lane) {
            var descendants = this.Descendants<MatchCard>();
            return this.Descendants<MatchCard>().First(MatchCard => MatchCard.Lane == lane);
        }
    }
}
