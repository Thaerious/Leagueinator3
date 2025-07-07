using Leagueinator.GUI.Utility.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Leagueinator.GUI.Controllers.DragDropDelegates;

namespace Leagueinator.GUI.Controllers {
    public class DragDropController {

        public DragDropController(FrameworkElement targetElement) {
            this.TargetElement = targetElement;
        }

        public static readonly RoutedEvent RequestFocusEvent = EventManager.RegisterRoutedEvent(
            "RequestFocus",                       // Event name
            RoutingStrategy.Bubble,               // Routing strategy (Bubble, Tunnel, or Direct)
            typeof(RequestFocus),                 // Delegate type
            typeof(FrameworkElement)              // MethodSource type
        );

        public static readonly RoutedEvent RegisteredDragBeginEvent = EventManager.RegisterRoutedEvent(
            "DragBegin",                          // Event name
            RoutingStrategy.Bubble,               // Routing strategy (Bubble, Tunnel, or Direct)
            typeof(DragBegin),                    // Delegate type
            typeof(FrameworkElement)              // MethodSource type
        );

        public static readonly RoutedEvent RegisteredDragEndEvent = EventManager.RegisterRoutedEvent(
            "DragEnd",                            // Event name
            RoutingStrategy.Bubble,               // Routing strategy (Bubble, Tunnel, or Direct)
            typeof(DragEnd),                      // Delegate type
            typeof(FrameworkElement)              // MethodSource type
        );

        public FrameworkElement TargetElement { get; private set; }

        public void HndDragEnter(object _, DragEventArgs e) {
            e.Effects = DragDropEffects.Copy;
        }

        public void HndPreviewDragOver(object _, DragEventArgs e) {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        /// <summary>
        /// On left-click, prepare drag-drop operation if the targetElement is not a TextBox or a CheckBox.
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        public void HndPreMouseDown(object _, MouseButtonEventArgs e) {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            // If the clicked element is a TextBox or CheckBox terminate the drag operation.
            var clickedElement = e.OriginalSource as DependencyObject;
            if (clickedElement != null && clickedElement.Ancestors<TextBox>().Any()) return;
            if (clickedElement != null && clickedElement.Ancestors<CheckBox>().Any()) return;

            e.Handled = true;

            DragBeginArgs args = new(RegisteredDragBeginEvent, this.TargetElement);
            this.TargetElement.RaiseEvent(args);
            DataObject dataObject = new(DataFormats.Serializable, this.TargetElement);
            DragDrop.DoDragDrop(this.TargetElement, dataObject, DragDropEffects.Move);
        }

        public void HndDrop(object _, DragEventArgs e) {
            // If the data is not of type FrameworkElement, ignore the drop.
            if (e.Data.GetData(DataFormats.Serializable) is not FrameworkElement from) return;

            e.Handled = true;

            // If the Target element is the same as the source, ignore the drop.
            if (from != this.TargetElement) {
                // Raise the DragEnd event with the source element and the Target element.
                DragEndArgs args = new(RegisteredDragEndEvent, from, this.TargetElement);
                this.TargetElement.RaiseEvent(args);
            }
        }
    }
}
