namespace GrandSeal.DataDemon.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    using Core.Engine.Contracts;
    using Core.Utils.Contracts;

    using GrandSeal.DataDemon.Contracts;

    internal class DemonFileInfoEntry
    {
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }

        public string RelativePath { get; set; }

        public string Hash { get; set; }

        public DateTime LastProcessed { get; set; }

        public override bool Equals(object obj)
        {
            var typed = obj as DemonFileInfoEntry;
            if (typed == null)
            {
                return false;
            }

            return typed.SourcePath.Equals(this.RelativePath, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.RelativePath.ToLowerInvariant().GetHashCode();
        }
    }

    public class DemonFileInfo : IDemonFileInfo
    {
        private readonly ILog log;

        private readonly HashSet<DemonFileInfoEntry> fileEntries; 

        private readonly List<string> sourceIncludes;
        private readonly List<string> intermediateIncludes; 

        private readonly Queue<string> pendingRefreshs;
        private readonly IDictionary<string, DateTime> fileLastAccess;

        private readonly Queue<DemonFileInfoEntry> pendingSources;
        private readonly Queue<DemonFileInfoEntry> pendingIntermediates;

        private bool needRefresh;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DemonFileInfo(IEngineFactory factory)
        {
            this.log = factory.Get<IDemonLog>().AquireContextLog("FileInfo");

            this.fileEntries = new HashSet<DemonFileInfoEntry>();

            this.pendingRefreshs = new Queue<string>();
            this.fileLastAccess = new Dictionary<string, DateTime>();

            this.pendingSources = new Queue<DemonFileInfoEntry>();
            this.pendingIntermediates = new Queue<DemonFileInfoEntry>();
            
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

        public int PendingSources
        {
            get
            {
                return this.pendingSources.Count;
            }
        }

        public int PendingIntermediates
        {
            get
            {
                return this.pendingIntermediates.Count;
            }
        }

        public void Refresh()
        {
            this.log.Debug("Refreshing FileInfo");

            for (int i = 0; i < this.sourceIncludes.Count; i++)
            {
                this.pendingRefreshs.Enqueue(this.sourceIncludes[i]);
            }

            while (this.pendingRefreshs.Count > 0)
            {
                string root = this.pendingRefreshs.Dequeue();
                string[] files = Directory.GetFiles(root);
                for (int i = 0; i < files.Length; i++)
                {
                    if (this.CheckFile(files[i]))
                    {
                        continue;
                    }

                    // Todo
                    //this.pendingSources.Enqueue(files[i]);
                }

                // Process sub-directories
                string[] directories = Directory.GetDirectories(root);
                for (int i = 0; i < directories.Length; i++)
                {
                    this.pendingRefreshs.Enqueue(directories[i]);
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
            
            this.pendingSources.Clear();
            this.pendingIntermediates.Clear();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool CheckFile(string file)
        {
            bool skip = false;
            string key = file.ToLowerInvariant();
            DateTime lastAccess = File.GetLastWriteTime(file);
            if (this.fileLastAccess.ContainsKey(key))
            {
                if (lastAccess <= this.fileLastAccess[key])
                {
                    skip = true;
                }
            }
            else
            {
                this.fileLastAccess.Add(file, lastAccess);
            }

            if (skip)
            {
                return true;
            }

            // Todo

            this.fileLastAccess.Add(key, lastAccess);
            return false;
        }
    }
}
