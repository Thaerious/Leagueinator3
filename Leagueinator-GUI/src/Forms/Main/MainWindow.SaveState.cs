namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow {
        private bool IsSaved {
            get => _isSaved;
            set {
                _isSaved = value;
                if (IsSaved) {
                    this.Title = $"Leagueinator [{this.FileName}]";
                }
                else {
                    this.Title = $"Leagueinator [{this.FileName}]*";
                }
            }
        }

        private string FileName { get => _filename; set => _filename = value; }

        private string _filename = "";
        private bool _isSaved = true;
    }
}

