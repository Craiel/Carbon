using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Carbed.Logic.MVVM
{
    public static class TreeViewExtension
    {
        private static TreeViewItem currentItem;

        private static readonly DependencyPropertyKey IsMouseOverItemProperty =
            DependencyProperty.RegisterAttachedReadOnly(
                "IsMouseOverItem", typeof(bool), typeof(TreeViewExtension), new FrameworkPropertyMetadata(null, CalculateMouseOverItem));

        public static readonly DependencyProperty IsMouseOverItem = IsMouseOverItemProperty.DependencyProperty;

        public static bool GetIsMouseOverItem(DependencyObject obj)
        {

            return (bool)obj.GetValue(IsMouseOverItem);

        }

        public static object CalculateMouseOverItem(DependencyObject item, object value)
        {
            return item == currentItem;
        }

        static TreeViewExtension()
        {
            EventManager.RegisterClassHandler(typeof(TreeViewItem), TreeViewItem.MouseEnterEvent, new MouseEventHandler(OnMouseTransition), true);
            EventManager.RegisterClassHandler(typeof(TreeViewItem), TreeViewItem.MouseLeaveEvent, new MouseEventHandler(OnMouseTransition), true);
            EventManager.RegisterClassHandler(typeof(TreeViewItem), UpdateOverItemEvent, new RoutedEventHandler(OnUpdateOverItem));
        }

        private static readonly RoutedEvent UpdateOverItemEvent = EventManager.RegisterRoutedEvent(
            "UpdateOverItem", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeViewExtension));

        private static void OnUpdateOverItem(object sender, RoutedEventArgs args)
        {
            currentItem = sender as TreeViewItem;
            currentItem.InvalidateProperty(IsMouseOverItem);
            args.Handled = true;
        }
   
        private static void OnMouseTransition(object sender, MouseEventArgs args)
        {
            lock (IsMouseOverItem)
            {
                if (currentItem != null)
                {
                    DependencyObject oldItem = currentItem;
                    currentItem = null;
                    oldItem.InvalidateProperty(IsMouseOverItem);
                }

                IInputElement currentElement = Mouse.DirectlyOver;
                if (currentElement != null)
                {
                    RoutedEventArgs elementArgs = new RoutedEventArgs(UpdateOverItemEvent);
                    currentElement.RaiseEvent(elementArgs);
                }
            }
        }
    }
}
