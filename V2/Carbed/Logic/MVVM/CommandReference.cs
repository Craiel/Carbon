using System;
using System.Windows;
using System.Windows.Input;

namespace Carbed.Logic.MVVM
{
    public class CommandReference : Freezable, ICommand
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(CommandReference), new PropertyMetadata(null, PropertyChangedCallback));

        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(CommandProperty);
            }

            set
            {
                this.SetValue(CommandProperty, value);
            }
        }

        public bool CanExecute(object parameter)
        {
            if (this.Command != null)
            {
                return this.Command.CanExecute(parameter);
            }

            return false;
        }

        public void Execute(object parameter)
        {
            this.Command.Execute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            CommandReference reference = dependencyObject as CommandReference;
            ICommand oldCommand = args.OldValue as ICommand;
            ICommand newCommand = args.NewValue as ICommand;

            if (reference == null)
            {
                throw new InvalidOperationException();
            }

            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= reference.CanExecuteChanged;
            }

            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += reference.CanExecuteChanged;
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
