using Leagueinator.GUI.Utility.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Leagueinator.GUI.Controls.DragDropDelegates;


namespace Leagueinator.GUI.Controls {
    /// <summary>
    /// Interaction logic for InfoCard.xaml
    /// </summary>
    public partial class InfoCard : UserControl {
        public InfoCard() {
            this.Loaded += this.HndLoaded;
            this.AllowDrop = true;
            this.InitializeComponent();
        }

        public event DragBegin OnDragBegin {
            add { this.AddHandler(DragDropController.RegisteredDragBeginEvent, value); }
            remove { this.RemoveHandler(DragDropController.RegisteredDragBeginEvent, value); }
        }

        public event DragEnd OnDragEnd {
            add { this.AddHandler(DragDropController.RegisteredDragEndEvent, value); }
            remove { this.RemoveHandler(DragDropController.RegisteredDragEndEvent, value); }
        }

        public int Lane {
            get {
                return int.Parse(this.LblLane.Text);
            }
            set {
                this.LblLane.Text = value.ToString();
            }
        }

        private void HndLoaded(object sender, RoutedEventArgs e) {
            DragDropController controller = new DragDropController(this);

            this.PreviewMouseDown += controller.HndPreMouseDown;
            this.DragEnter += controller.HndDragEnter;
            this.Drop += controller.HndDrop;

            this.GetDescendantsOfType<TextBox>()
                .ToList()
                .ForEach(textBox => {
                    textBox.AllowDrop = true;
                    textBox.DragEnter += controller.HndDragEnter;
                    textBox.PreviewDragOver += controller.HndPreviewDragOver;
                });
        }

        public void OnlyNumbers(object sender, TextCompositionEventArgs e) {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string text) {
            return text.All(char.IsDigit);
        }
    }
}
