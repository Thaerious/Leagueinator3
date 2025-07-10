using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public record EventRecord {
            public string Name { get; set; } = "";
            public int DefaultEnds { get; set; }
            public int LaneCount { get; set; }
        }

        EventRecord target = new EventRecord { Name = "Event C", DefaultEnds = 10, LaneCount = 8 };

        public ObservableCollection<EventRecord> EventRecords { get; set; }

        public MainWindow() {
            InitializeComponent();

            EventRecords = [
                new EventRecord { Name = "Event A", DefaultEnds = 6, LaneCount = 4 },
                new EventRecord { Name = "Event B", DefaultEnds = 8, LaneCount = 6 },
                target
            ];

            EventData.ItemsSource = EventRecords;
        }

        public void HndClick(object sender, RoutedEventArgs e) {
            Debug.WriteLine($"HndClick");
            this.EventData.SelectedItem = new EventRecord { Name = "Event C", DefaultEnds = 10, LaneCount = 8 };
        }
    }
}
