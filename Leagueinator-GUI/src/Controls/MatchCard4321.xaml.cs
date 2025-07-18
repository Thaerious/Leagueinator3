﻿using Leagueinator.GUI.Utility.Extensions;
using System.Windows.Controls;

namespace Leagueinator.GUI.Controls {
    public partial class MatchCard4321 : MatchCard {
        public MatchCard4321() : base() {
            this.InitializeComponent();

            this.Loaded += (s, e) => {
                foreach (TextBox textBox in this.FindByTag("Bowls").Cast<TextBox>()) {
                    textBox.PreviewTextInput += InputHandlers.OnlyNumbers;
                }
            };
        }
    }
}
