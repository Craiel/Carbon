using System;
using System.Collections.Generic;
using System.Linq;

using GrandSeal.Editor.Contracts;

using Core.Engine.Contracts;

namespace GrandSeal.Editor.Logic
{
    using System.Windows;

    using Core.Editor.Resource.Collada;

    internal struct SynchronizationEntry
    {
        public string Name;
        public string File;

        public bool IsNormal;
        public bool ToNormal;
    }

    public class TextureSynchronizer : EditorBase, ITextureSynchronizer
    {
        private readonly IList<SynchronizationEntry> queuedForAdd;
        private readonly IList<IResourceViewModel> queuedForDelete;
        private readonly IList<SynchronizationEntry> synchronizedFiles;
        private readonly IList<SynchronizationEntry> missingFiles;

        private readonly IEditorLogic logic;
        
        private IFolderViewModel target;
        private ColladaInfo source;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TextureSynchronizer(IEngineFactory factory)
        {
            this.logic = factory.Get<IEditorLogic>();

            this.queuedForAdd = new List<SynchronizationEntry>();
            this.queuedForDelete = new List<IResourceViewModel>();
            this.synchronizedFiles = new List<SynchronizationEntry>();
            this.missingFiles = new List<SynchronizationEntry>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Synchronized
        {
            get
            {
                return this.synchronizedFiles.Count;
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
                IList<string> list = this.synchronizedFiles.Select(x => x.File).ToList();
                return string.Join(Environment.NewLine, list);
            }
        }

        public string NewTextList
        {
            get
            {
                IList<string> list = this.queuedForAdd.Select(x => string.Format("{0} -> {1}", x.File, x.Name ?? System.IO.Path.GetFileName(x.File))).ToList();
                return string.Join(Environment.NewLine, list);
            }
        }

        public string DeletedTextList
        {
            get
            {
                IList<string> list = this.queuedForDelete.Select(x => x.SourcePath).ToList();
                return string.Join(Environment.NewLine, list);
            }
        }

        public string MissingTextList
        {
            get
            {
                IList<string> list = this.missingFiles.Select(x => x.File).ToList();
                return string.Join(Environment.NewLine, list);
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
            this.synchronizedFiles.Clear();

            if (this.target == null || this.source == null)
            {
                return;
            }

            string sourcePath = System.IO.Path.GetDirectoryName(this.source.Source);
            if (string.IsNullOrEmpty(sourcePath))
            {
                return;
            }

            IList<IResourceViewModel> targetResources = this.target.Content.Where(x => x as IResourceViewModel != null).Cast<IResourceViewModel>().ToList();
            IList<IResourceViewModel> targetResourcesFound = new List<IResourceViewModel>();
            IList<SynchronizationEntry> resources = new List<SynchronizationEntry>();
            foreach (string file in this.source.ImageInfos.Values)
            {
                var entry = new SynchronizationEntry
                    {
                        File = System.IO.Path.Combine(sourcePath, Uri.UnescapeDataString(file)),
                        IsNormal = this.source.NormalImages.Contains(file),
                    };

                resources.Add(entry);
            }

            foreach (string normalName in this.source.ColorToNormalImages.Keys)
            {
                var entry = new SynchronizationEntry
                {
                    File = System.IO.Path.Combine(sourcePath, Uri.UnescapeDataString(this.source.ColorToNormalImages[normalName])),
                    Name = Uri.UnescapeDataString(normalName),
                    ToNormal = true
                };

                resources.Add(entry);
            }

            foreach (SynchronizationEntry resource in resources)
            {
                bool found = false;
                foreach (IResourceViewModel resourceViewModel in targetResources)
                {
                    if (resourceViewModel.SourcePath.Equals(resource.File, StringComparison.OrdinalIgnoreCase))
                    {
                        if (resource.Name == null)
                        {
                            if (!resourceViewModel.Name.Equals(System.IO.Path.GetFileName(resource.File), StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (!resourceViewModel.Name.Equals(resource.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
                        }
                        
                        found = true;
                        this.synchronizedFiles.Add(resource);
                        targetResourcesFound.Add(resourceViewModel);
                        break;
                    }
                }

                if (!found)
                {
                    if (System.IO.File.Exists(resource.File))
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
            foreach (SynchronizationEntry entry in this.queuedForAdd)
            {
                IResourceTextureViewModel viewModel = this.logic.AddResourceTexture();
                viewModel.SelectFile(entry.File);
                viewModel.IsNormalMap = entry.IsNormal;
                viewModel.ConvertToNormalMap = entry.ToNormal;
                if (entry.Name != null)
                {
                    viewModel.Name = entry.Name;
                }

                Application.Current.Dispatcher.Invoke(() => this.target.AddContent(viewModel));
                this.synchronizedFiles.Add(entry);
            }
            this.queuedForAdd.Clear();

            foreach (IResourceViewModel viewModel in this.queuedForDelete)
            {
                IResourceViewModel model = viewModel;
                Application.Current.Dispatcher.Invoke(() => this.logic.Delete(model));
            }

            this.queuedForDelete.Clear();

            this.NotifyPropertyChanged();
        }
    }
}
