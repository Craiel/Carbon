using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Carbed.Logic.MVVM
{
    // http://alexshed.spaces.live.com/blog/cns!71C72270309CE838!149.entry
    public static class ListBoxSelectionSynchronization
    {
        private static readonly DependencyPropertyKey IsResynchingPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsResynching", typeof(bool), typeof(ListBoxSelectionSynchronization), new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedItemsSourceProperty = DependencyProperty.RegisterAttached("SelectedItemsSource", typeof(IList), typeof(ListBoxSelectionSynchronization), new PropertyMetadata(null, OnSelectedItemsSourceChanged));

        public static IList GetSelectedItemsSource(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (IList)element.GetValue(SelectedItemsSourceProperty);
        }

        public static void SetSelectedItemsSource(DependencyObject element, IList value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(SelectedItemsSourceProperty, value);
        }

        private static void OnSelectedItemsSourceChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ListBox listBox = source as ListBox;
            if (listBox == null)
            {
                throw new InvalidOperationException("The ListBoxExtension.SelectedItemsSource attached property can only be applied to ListBox controls.");
            }

            listBox.SelectionChanged -= OnListBoxSelectionChanged;

            if (args.NewValue != null)
            {
                ListenForChanges(listBox);
            }
        }

        private static void ListenForChanges(ListBox listBox)
        {
            // Wait until the element is initialised
            if (!listBox.IsInitialized)
            {
                EventHandler callback = null;
                callback = delegate
                    {
                        listBox.Initialized -= callback;
                        ListenForChanges(listBox);
                    };
                listBox.Initialized += callback;
                return;
            }

            listBox.SelectionChanged += OnListBoxSelectionChanged;
            ResynchList(listBox);
        }

        private static void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                bool isResynching = (bool)listBox.GetValue(IsResynchingPropertyKey.DependencyProperty);
                if (isResynching)
                {
                    return;
                }

                IList list = GetSelectedItemsSource(listBox);
                HashSet<object> hash = new HashSet<object>();

                for (int i = 0; i < list.Count; i++)
                {
                    hash.Add(list[i]);
                }

                for (int i = 0; i < args.RemovedItems.Count; i++)
                {
                    hash.Remove(args.RemovedItems[i]);
                }

                for (int i = 0; i < args.AddedItems.Count; i++)
                {
                    if (!hash.Contains(args.AddedItems[i]))
                    {
                        hash.Add(args.AddedItems[i]);
                    }
                }

                list.Clear();
                foreach (object o in hash)
                {
                    list.Add(o);
                }
            }
        }

        private static void ResynchList(ListBox listBox)
        {
            if (listBox != null)
            {
                listBox.SetValue(IsResynchingPropertyKey, true);
                IList list = GetSelectedItemsSource(listBox);

                if (listBox.SelectionMode == SelectionMode.Single)
                {
                    listBox.SelectedItem = null;
                    if (list != null)
                    {
                        if (list.Count > 1)
                        {
                            // There is more than one item selected, but the listbox is in Single selection mode
                            throw new InvalidOperationException("ListBox is in Single selection mode, but was given more than one selected value.");
                        }

                        if (list.Count == 1)
                        {
                            listBox.SelectedItem = list[0];
                        }
                    }
                }
                else
                {
                    listBox.SelectedItems.Clear();
                    if (list != null)
                    {
                        foreach (object obj in listBox.Items)
                        {
                            if (list.Contains(obj))
                            {
                                listBox.SelectedItems.Add(obj);
                            }
                        }
                    }
                }

                listBox.SetValue(IsResynchingPropertyKey, false);
            }
        }
    }
}