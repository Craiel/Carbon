using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;

namespace GrandSeal.Editor.Logic.MVVM
{
    // http://meleak.wordpress.com/2011/08/28/onewaytosource-binding-for-readonly-dependency-property/
    public class OneWayBindingManager
    {
        public static DependencyProperty OneWayBindingsProperty = DependencyProperty.RegisterAttached(
            "OneWayBindingsInternal",
            typeof(OneWayBindingCollection),
            typeof(OneWayBindingManager),
            new UIPropertyMetadata(null));

        public static OneWayBindingCollection GetOneWayBindings(DependencyObject obj)
        {
            if (obj.GetValue(OneWayBindingsProperty) == null)
            {
                obj.SetValue(OneWayBindingsProperty, new OneWayBindingCollection(obj));
            }

            return (OneWayBindingCollection)obj.GetValue(OneWayBindingsProperty);
        }

        public static void SetOneWayBindings(DependencyObject obj, OneWayBindingCollection value)
        {
            obj.SetValue(OneWayBindingsProperty, value);
        }
    }

    public class OneWayBindingCollection : FreezableCollection<OneWayBinding>
    {
        public OneWayBindingCollection(DependencyObject target)
        {
            Target = target;
            ((INotifyCollectionChanged)this).CollectionChanged += this.OnCollectionChanged;
        }

        public DependencyObject Target { get; private set; }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (OneWayBinding binding in args.NewItems)
                {
                    binding.SetupTargetBinding((FrameworkElement)this.Target);
                }
            }
        }
    }

    public class OneWayBinding : FreezableBinding
    {
        public static DependencyProperty TargetMirrorProperty = DependencyProperty.Register(
            "TargetMirror", typeof(object), typeof(OneWayBinding));

        public static DependencyProperty TargetListenerProperty = DependencyProperty.Register(
            "TargetListener", typeof(object), typeof(OneWayBinding), new UIPropertyMetadata(null, OnTargetListenerChanged));

        public string TargetProperty { get; set; }

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public OneWayBinding()
        {
            this.Mode = BindingMode.OneWayToSource;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void SetupTargetBinding(FrameworkElement target)
        {
            var binding = new Binding
                {
                    Source = target,
                    Path = new PropertyPath(TargetProperty),
                    Mode = BindingMode.OneWay
                };

            BindingOperations.SetBinding(this, TargetListenerProperty, binding);
            BindingOperations.SetBinding(this, TargetMirrorProperty, this.Binding);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void OnTargetListenerChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            OneWayBinding binding = sender as OneWayBinding;
            binding.TargetValueChanged();
        }

        private void TargetValueChanged()
        {
            object targetValue = GetValue(TargetListenerProperty);
            this.SetValue(TargetMirrorProperty, targetValue);
        }
    }
}
