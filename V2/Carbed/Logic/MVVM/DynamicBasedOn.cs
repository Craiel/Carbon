using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Carbed.Logic.MVVM
{
    public static class DynamicBasedOn
    {
        public static readonly DependencyProperty DynamicBasedOnProperty = DependencyProperty.RegisterAttached("DynamicBasedOn", typeof(Style), typeof(DynamicBasedOn), new UIPropertyMetadata(null, OnDynamicBasedOnChanged));
        public static readonly DependencyProperty BaseStyleProperty = DependencyProperty.RegisterAttached("BaseStyle", typeof(Style), typeof(DynamicBasedOn), new UIPropertyMetadata(null));

        private static bool basedOnChangedLock;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Style GetBaseStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(BaseStyleProperty);
        }

        public static void SetBaseStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(BaseStyleProperty, value);
        }

        public static Style GetDynamicBasedOn(DependencyObject obj)
        {
            return (Style)obj.GetValue(DynamicBasedOnProperty);
        }

        public static void SetDynamicBasedOn(DependencyObject obj, Style value)
        {
            obj.SetValue(DynamicBasedOnProperty, value);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void OnDynamicBasedOnChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (basedOnChangedLock)
            {
                return;
            }

            FrameworkElement frameworkElement = dependencyObject as FrameworkElement;
            if (frameworkElement == null)
            {
                throw new InvalidOperationException("Cannot use DynamicBasedOn on a non FrameworkElement object");
            }

            Style style = GetBaseStyle(frameworkElement);
            if (style == null)
            {
                style = frameworkElement.Style ?? frameworkElement.GetType().GetProperty("ThemeStyle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(frameworkElement, null) as Style;

                SetBaseStyle(frameworkElement, style);
            }

            if (style == null)
            {
                style = frameworkElement.Style;
                SetBaseStyle(frameworkElement, style);
            }

            Style dynamicStyle = args.NewValue as Style;
            if (style != null && dynamicStyle == style)
            {
                // two cases :
                // 1 - the style of the element is actually defined in it's own resource. We need then to
                // remove it from the element resource dictionary and set the "binding" of the style so that
                // it get updated whenever the style for this type is changed.
                if (frameworkElement.Resources.Contains(style.TargetType))
                {
                    // we need to execute that after not in this function, if not we will recourse (well actually not since we are removing
                    // the dependence but the framework will think so and raise an exception.
                    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        frameworkElement.Resources.Remove(style.TargetType);
                        frameworkElement.SetResourceReference(args.Property, style.TargetType);
                    }));
                    return;
                }

                // 2 - the style is defined on one of the parent, walk the hierarchy and try to find the style that is defined
                // in a parent of the parent. At last resort, get the default style.
                FrameworkElement parent = frameworkElement;
                while (parent != null)
                {
                    if (parent.Resources.Contains(style.TargetType) && parent.Resources[style.TargetType] != style)
                    {
                        dynamicStyle = parent.Resources[style.TargetType] as Style;
                        break;
                    }

                    parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
                }

                // no style found, get the default one.
                if (dynamicStyle == style)
                {
                    dynamicStyle = Application.Current.FindResource(style.TargetType) as Style;
                }
            }

            // no style apply on this item, we should normally traverse the visual tree to get the style.
            // here we are just getting the global style for the target type.
            if (style == null)
            {
                frameworkElement.Style = dynamicStyle;
            }
            else
            {
                Style newStyle = new Style(style.TargetType) { Resources = style.Resources };
                foreach (SetterBase setter in style.Setters)
                {
                    newStyle.Setters.Add(setter);
                }

                foreach (TriggerBase trigger in style.Triggers)
                {
                    newStyle.Triggers.Add(trigger);
                }

                newStyle.BasedOn = dynamicStyle;

                // Don't know why but the OnDynamicBasedOnChanged keeps firing itself in Release builds so we just lock it while we set our new style
                basedOnChangedLock = true;
                try
                {
                    frameworkElement.Style = newStyle;
                }
                catch (InvalidOperationException)
                {
                    // Keep getting this error
                }

                basedOnChangedLock = false;
            }
        }
    }
}
