using System.Windows;
using System.Windows.Data;

namespace GrandSeal.Editor.Logic.MVVM
{
    public class FreezableBinding : Freezable
    {
        private Binding binding;

        protected Binding Binding
        {
            get
            {
                if (this.binding == null)
                {
                    this.binding = new Binding();
                }

                return this.binding;
            }
        }

        public PropertyPath Path
        {
            get
            {
                return this.Binding.Path;
            }

            set
            {
                this.Binding.Path = value;
            }
        }

        public BindingMode Mode 
        {
            get
            {
                return this.Binding.Mode;
            }

            set
            {
                this.Binding.Mode = value;
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new FreezableBinding();
        }
    }
}
