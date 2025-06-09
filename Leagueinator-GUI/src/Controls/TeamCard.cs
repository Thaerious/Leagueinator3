using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using static Leagueinator.GUI.Controls.DragDropDelegates;

namespace Leagueinator.GUI.Controls {
    public class TeamCard : Border {

        public TeamCard() {
            this.AllowDrop = true;
            this.Loaded += this.HndLoaded;
        }

        public event DragBegin OnDragBegin {
            add { this.AddHandler(DragDropController.RegisteredDragBeginEvent, value); }
            remove { this.RemoveHandler(DragDropController.RegisteredDragBeginEvent, value); }
        }

        public event DragEnd OnDragEnd {
            add { this.AddHandler(DragDropController.RegisteredDragEndEvent, value); }
            remove { this.RemoveHandler(DragDropController.RegisteredDragEndEvent, value); }
        }

        public static readonly DependencyProperty TeamIndexProperty = DependencyProperty.Register(
            "TeamIndex",
            typeof(int),
            typeof(TeamCard),
            new PropertyMetadata(default(int))
        );

        public static readonly DependencyProperty TeamBowlsPropery = DependencyProperty.Register(
            "Bowls",
            typeof(int),
            typeof(TeamCard),
            new PropertyMetadata(default(int))
        );

        private void HndLoaded(object sender, RoutedEventArgs e) {
            DragDropController controller = new DragDropController(this);

            this.PreviewMouseDown += controller.HndPreMouseDown;
            this.DragEnter += controller.HndDragEnter;
            this.Drop += controller.HndDrop;

            this.Descendants<MemoryTextBox>()
                .Where(textBox => textBox.HasTag("PlayerName"))
                .ToList()
                .ForEach(textBox => {
                    textBox.AllowDrop = true;
                    textBox.DragEnter += controller.HndDragEnter;
                    textBox.PreviewDragOver += controller.HndPreviewDragOver;
                });
        }

        /// <summary>
        /// Gets or sets the index of the team associated with this <see cref="TeamCard"/>.
        /// </summary>
        /// <value>
        /// An integer representing the index of the team. The default value is determined by the 
        /// <see cref="TeamIndexProperty"/> dependency property.
        /// </value>
        /// <remarks>
        /// The <see cref="TeamIndex"/> property is a dependency property, meaning it supports data binding, 
        /// animation, styling, and other WPF features.
        /// </remarks>
        public int TeamIndex {
            get { return (int)this.GetValue(TeamIndexProperty); }
            set { this.SetValue(TeamIndexProperty, value); }
        }

        public int Bowls {
            get { return (int)this.GetValue(TeamBowlsPropery); }
            set { 
                this.SetValue(TeamBowlsPropery, value); 
            }
        }  

        /// <summary>
        /// Determines whether any <see cref="MemoryTextBox"/> within the current object
        /// contains the specified name as its text.
        /// </summary>
        /// <param name="name">The name To search for within the text of <see cref="MemoryTextBox"/> controls.</param>
        /// <returns>
        /// <c>true</c> if any <see cref="MemoryTextBox"/> contains the specified name as its text;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method searches through all <see cref="MemoryTextBox"/> descendants of the current object
        /// and returns <c>true</c> if a match is found.
        /// </remarks>
        public bool HasName(string name) {
            return this.Descendants<MemoryTextBox>()
                       .Where(textBox => textBox.HasTag("PlayerName"))
                       .Where(textBox => textBox.Text == name)
                       .Any();
        }

        /// <summary>
        /// Sets the text of the first available <see cref="MemoryTextBox"/> To name.
        /// </summary>
        /// <param name="name">The name To add To a <see cref="MemoryTextBox"/> control.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if there are no available <see cref="MemoryTextBox"/> controls with empty text To add the name.
        /// </exception>
        /// <remarks>
        /// If the specified name is already present in any <see cref="MemoryTextBox"/>, the method returns without making any changes.
        /// Otherwise, it assigns the name To the first <see cref="MemoryTextBox"/> that has empty text. 
        /// If no such text box is available, an <see cref="InvalidOperationException"/> is thrown.
        /// </remarks>
        public void AddName(string name) {
            Debug.WriteLine($"Adding name '{name}' to TeamCard {this.TeamIndex}");
            if (this.HasName(name)) return;

            var first = this.Descendants<MemoryTextBox>()
                .Where(textBox => textBox.HasTag("PlayerName"))
                .FirstOrDefault(textBox => textBox.Text.IsEmpty())
                ?? throw new InvalidOperationException("No available text boxes.");

            first.Text = name;
        }

        /// <summary>
        /// Removes the specified name from all <see cref="MemoryTextBox"/> controls that are descendants
        /// of the current object. If a <see cref="MemoryTextBox"/> contains the specified name, 
        /// its text is cleared.
        /// </summary>
        /// <param name="name">The name To remove from the text boxes.</param>
        public void RemoveName(string name) {
            this.Descendants<MemoryTextBox>()
                .Where(textBox => textBox.HasTag("PlayerName"))
                .Where(textBox => textBox.Text == name)
                .ToList()
                .ForEach(textBox => {
                    textBox.Clear();
                });
        }

        /// <summary>
        /// Clears the text from all <see cref="MemoryTextBox"/> controls that are descendants
        /// of the current object. 
        /// </summary>
        public void ClearNames() {
            this.Descendants<MemoryTextBox>()
                .Where(textBox => textBox.HasTag("PlayerName"))
                .ToList()
                .ForEach(textBox => {
                    textBox.Clear();
                });
        }
    }
}
