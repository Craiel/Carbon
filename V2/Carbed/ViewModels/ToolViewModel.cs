using System;

using Carbed.Contracts;
using Carbed.Logic;

namespace Carbed.ViewModels
{
    public abstract class ToolViewModel : CarbedBase, ICarbedTool
    {
        private bool isVisible;

        private bool isSelected;

        private bool isActive;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ToolViewModel()
        {
            this.isVisible = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract string Title { get; }

        public virtual Uri IconUri
        {
            get
            {
                return StaticResources.ToolIconUri;
            }
        }

        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }
            set
            {
                if (this.isVisible != value)
                {
                    this.isVisible = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                if (this.isActive != value)
                {
                    this.isActive = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
    }
}
