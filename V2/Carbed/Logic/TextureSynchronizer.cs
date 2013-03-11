using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Carbed.Contracts;

using Carbon.Editor.Resource.Collada;

namespace Carbed.Logic
{
    public class TextureSynchronizer : CarbedBase, ITextureSynchronizer
    {
        private readonly IList<string> queuedForAdd;
        private readonly IList<IResourceViewModel> queuedForDelete;
        private readonly IList<string> synchronizedFiles;
        private readonly IList<string> missingFiles;

        private readonly ICarbedLogic logic;

        private int synchronized;

        private IFolderViewModel target;
        private ColladaInfo source;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TextureSynchronizer(ICarbedLogic logic)
        {
            this.logic = logic;

            this.queuedForAdd = new List<string>();
            this.queuedForDelete = new List<IResourceViewModel>();
            this.synchronizedFiles = new List<string>();
            this.missingFiles = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Synchronized
        {
            get
            {
                return this.synchronized;
            }

            private set
            {
                if (this.synchronized != value)
                {
                    this.synchronized = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public int NewTextures
        {
            get
            {
                return this.queuedForAdd.Count;
            }
        }

        public int Deleted
        {
            get
            {
                return this.queuedForDelete.Count;
            }
        }

        public int Missing
        {
            get
            {
                return this.missingFiles.Count;
            }
        }

        public string SynchronizedTextList
        {
            get
            {
                return string.Join(Environment.NewLine, this.synchronizedFiles);
            }
        }

        public string NewTextList
        {
            get
            {
                return string.Join(Environment.NewLine, this.queuedForAdd);
            }
        }

        public string DeletedTextList
        {
            get
            {
                IList<string> deleteList = this.queuedForDelete.Select(viewModel => viewModel.SourcePath).ToList();
                return string.Join(Environment.NewLine, deleteList);
            }
        }

        public string MissingTextList
        {
            get
            {
                return string.Join(Environment.NewLine, this.missingFiles);
            }
        }

        public void SetTarget(IFolderViewModel folder)
        {
            if (this.target != folder)
            {
                this.target = folder;
                this.Refresh();
            }
        }

        public void SetSource(ColladaInfo info)
        {
            if (this.source != info)
            {
                this.source = info;
                this.Refresh();
            }
        }

        public void Refresh()
        {
            this.queuedForAdd.Clear();
            this.queuedForDelete.Clear();
            this.missingFiles.Clear();
            this.Synchronized = 0;

            if (this.target == null || this.source == null)
            {
                return;
            }

            string sourcePath = System.IO.Path.GetDirectoryName(this.source.Source);
            if (string.IsNullOrEmpty(sourcePath))
            {
                return;
            }

            IList<IResourceViewModel> targetResources = this.target.Content.Where(carbedDocument => carbedDocument as IResourceViewModel != null).Cast<IResourceViewModel>().ToList();
            IList<IResourceViewModel> targetResourcesFound = new List<IResourceViewModel>();
            IList<string> resources = new List<string>();
            foreach (string file in this.source.ImageInfos.Values)
            {
                resources.Add(System.IO.Path.Combine(sourcePath, Uri.UnescapeDataString(file)));
            }

            foreach (string resource in resources)
            {
                bool found = false;
                foreach (IResourceViewModel resourceViewModel in targetResources)
                {
                    if (resourceViewModel.SourcePath.Equals(resource, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        this.synchronizedFiles.Add(resource);
                        targetResourcesFound.Add(resourceViewModel);
                        break;
                    }
                }

                if (!found)
                {
                    if (System.IO.File.Exists(resource))
                    {
                        this.queuedForAdd.Add(resource);
                    }
                    else
                    {
                        this.missingFiles.Add(resource);
                    }
                }
            }

            foreach (IResourceViewModel resourceViewModel in targetResources)
            {
                if (!targetResourcesFound.Contains(resourceViewModel))
                {
                    this.queuedForDelete.Add(resourceViewModel);
                }
            }

            this.NotifyPropertyChanged();
        }

        public void Synchronize()
        {
            foreach (string file in this.queuedForAdd)
            {
                IResourceViewModel viewModel = this.logic.AddResource();
                viewModel.SelectFile(file);
                this.target.AddContent(viewModel);
                this.synchronizedFiles.Add(file);
            }
            this.queuedForAdd.Clear();

            foreach (IResourceViewModel viewModel in this.queuedForDelete)
            {
                this.logic.Delete(viewModel);
            }

            this.queuedForDelete.Clear();

            this.NotifyPropertyChanged();
        }
    }
}
