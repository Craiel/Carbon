namespace GrandSeal.DataDemon.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    using CarbonCore.Utils.Contracts.IoC;
    
    using GrandSeal.DataDemon.Contracts;

    public class DemonFileInfo : IDemonFileInfo
    {
        private readonly List<string> sourceIncludes;
        private readonly List<string> intermediateIncludes; 

        private readonly Queue<string> pendingRefreshQueue;

        private bool needRefresh = true;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DemonFileInfo(IFactory factory)
        {
            this.pendingRefreshQueue = new Queue<string>();
            
            this.sourceIncludes = new List<string>();
            this.intermediateIncludes = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ReadOnlyCollection<string> SourceIncludes
        {
            get
            {
                return this.sourceIncludes.AsReadOnly();
            }
        }

        public ReadOnlyCollection<string> IntermediateIncludes
        {
            get
            {
                return this.intermediateIncludes.AsReadOnly();
            }
        }

        public int PendingEntries
        {
            get
            {
                return -1;
            }
        }

        public void Refresh()
        {
            if (this.needRefresh)
            {
                this.needRefresh = false;
            }

            for (int i = 0; i < this.sourceIncludes.Count; i++)
            {
                this.RefreshFromSource(this.sourceIncludes[i]);
            }

            for (int i = 0; i < this.intermediateIncludes.Count; i++)
            {
                this.RefreshFromIntermediate(this.intermediateIncludes[i]);
            }
        }
        
        public void RefreshFromSource(string includeRoot)
        {
            System.Diagnostics.Trace.TraceInformation("Refreshing FileInfo or source includes");
            this.pendingRefreshQueue.Enqueue(includeRoot);

            while (this.pendingRefreshQueue.Count > 0)
            {
                string root = this.pendingRefreshQueue.Dequeue();
                string[] files = Directory.GetFiles(root);
                for (int i = 0; i < files.Length; i++)
                {
                    System.Diagnostics.Trace.TraceWarning("Processing: "+files[i]);
                    /*var entry = new ContentInfoEntry();
                    entry.InitializeFromSource(files[i]);
                    if (this.CheckFile(entry))
                    {
                        continue;
                    }

                    this.pendingEntries.Add(entry);*/
                }

                // Process sub-directories
                string[] directories = Directory.GetDirectories(root);
                for (int i = 0; i < directories.Length; i++)
                {
                    this.pendingRefreshQueue.Enqueue(directories[i]);
                }
            }
        }

        public void RefreshFromIntermediate(string includeRoot)
        {
            System.Diagnostics.Trace.TraceInformation("Refreshing FileInfo for intermediate includes");
            this.pendingRefreshQueue.Enqueue(includeRoot);

            while (this.pendingRefreshQueue.Count > 0)
            {
                string root = this.pendingRefreshQueue.Dequeue();
                string[] files = Directory.GetFiles(root);
                for (int i = 0; i < files.Length; i++)
                {
                    System.Diagnostics.Trace.TraceWarning("File: "+files[i]);
                    /*var entry = new ContentInfoEntry();
                    entry.InitializeFromIntermediate(files[i]);
                    if (this.CheckFile(entry))
                    {
                        continue;
                    }

                    this.pendingEntries.Add(entry);*/
                }

                // Process sub-directories
                string[] directories = Directory.GetDirectories(root);
                for (int i = 0; i < directories.Length; i++)
                {
                    this.pendingRefreshQueue.Enqueue(directories[i]);
                }
            }
        }

        public void AddSourceInclude(string path)
        {
            this.needRefresh = true;
            this.sourceIncludes.Add(path);
        }

        public void AddIntermediateInclude(string path)
        {
            this.needRefresh = true;
            this.intermediateIncludes.Add(path);
        }

        public void RemoveSourceInclude(string path)
        {
            this.needRefresh = true;
            this.sourceIncludes.Remove(path);
        }

        public void RemoveIntermediateInclude(string path)
        {
            this.needRefresh = true;
            this.intermediateIncludes.Remove(path);
        }

        public void Clear()
        {
            this.needRefresh = true;
            this.sourceIncludes.Clear();
            this.intermediateIncludes.Clear();
        }
    }
}
