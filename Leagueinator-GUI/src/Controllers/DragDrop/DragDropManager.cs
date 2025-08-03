using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Leagueinator.Utility.Extensions;

namespace Leagueinator.GUI.Controllers.DragDropManager {
    public class DragDropManager<T> where T : FrameworkElement, IDragDrop {

        public DragDropManager(T targetElement) {
            this.TargetElement = targetElement;
            targetElement.PreviewMouseDown += this.HndPreMouseDown;
            targetElement.DragEnter += this.HndDragEnter;
            targetElement.Drop += this.HndDrop;
        }

        public T TargetElement { get; private set; }

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

            DataObject dataObject = new(DataFormats.Serializable, this.TargetElement);
            object value = DragDrop.DoDragDrop(this.TargetElement, dataObject, DragDropEffects.Move);
        }

        public void HndDrop(object _, DragEventArgs e) {
            // If the data is not of type FrameworkElement, ignore the drop.
            if (e.Data.GetData(DataFormats.Serializable) is not FrameworkElement from) return;
            e.Handled = true;
            this.TargetElement.DoDrop(from);
        }
    }
}
