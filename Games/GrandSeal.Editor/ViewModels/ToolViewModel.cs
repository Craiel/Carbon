﻿namespace GrandSeal.Editor.ViewModels
{
    using System;

    using CarbonCore.ToolFramework.ViewModel;

    using GrandSeal.Editor.Contracts;

    public abstract class ToolViewModel : BaseViewModel, IEditorTool
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

        public string ContentId
        {
            get
            {
                return string.Concat("Tool|", this.GetType().ToString());
            }
        }

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
