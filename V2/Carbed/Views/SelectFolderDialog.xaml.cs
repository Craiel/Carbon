﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using Carbed.Contracts;

namespace Carbed.Views
{
    public partial class SelectFolderDialog
    {
        private readonly List<IFolderViewModel> folders;

        public SelectFolderDialog(ICarbedLogic logic)
        {
            this.folders = new List<IFolderViewModel>();
            foreach (IFolderViewModel folder in logic.Folders)
            {
                this.AppendFolder(folder);
            }

            this.DataContext = this;

            InitializeComponent();
        }

        public ReadOnlyCollection<IFolderViewModel > Folders
        {
            get
            {
                return this.folders.AsReadOnly();
            }
        }

        public IFolderViewModel SelectedFolder { get; set; }

        private void OnSelectClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void AppendFolder(IFolderViewModel folder)
        {
            this.folders.Add(folder);
            foreach (ICarbedDocument content in folder.Content)
            {
                if ((content as IFolderViewModel) == null)
                {
                    continue;
                }

                this.AppendFolder(content as IFolderViewModel);
            }
        }
    }
}
