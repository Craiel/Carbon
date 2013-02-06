using Carbon.Engine.Resource.Resources;

namespace Carbon.Editor.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Carbon.Editor.Contracts;
    using Carbon.Editor.Processors;
    using Carbon.Editor.Resource;
    using Carbon.Engine.Contracts;
    using Carbon.Engine.Contracts.Resource;
    using Carbon.Engine.Resource;
    using Carbon.Engine.Resource.Content;

    public class CarbonBuilderEntry
    {
        public Type Type { get; set; }
        public string Path { get; set; }
        public SourceFolderContent Content { get; set; }
    }

    public class CarbonBuilder : ICarbonBuilder
    {
        private static readonly IDictionary<Type, IContentProcessor> contentProcessors; 

        private readonly IDictionary<Type, IList<CarbonBuilderEntry>> contentDictionary;

        private readonly IResourceManager resourceManager;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static CarbonBuilder()
        {
            /*contentProcessors = new Dictionary<Type, IContentProcessor>
                {
                    { typeof(SourceTextureFont), new FontProcessor() },
                    { typeof(SourceModel), new ModelProcessor() }
                };*/
        }

        public CarbonBuilder(IEngineFactory factory)
        {
            this.resourceManager = factory.Get<IResourceManager>();

            this.contentDictionary = new Dictionary<Type, IList<CarbonBuilderEntry>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event CarbonBuilderProgressChangedDelegate ProgressChanged;

        public void Build(string target, SourceProject project)
        {
            this.PrepareBuild();
            this.GatherContent(string.Empty, project.Root);

            this.resourceManager.Clear();
            this.resourceManager.AddContent(new FolderContent(target));

            this.ProcessContent();
        }

        public void Process(Stream target, SourceFolderContent content)
        {
            Type contentType = content.GetType();
            if (!contentProcessors.ContainsKey(contentType))
            {
                throw new InvalidDataException("No content processor for type " + contentType);
            }

            //contentProcessors[contentType].Process(target, new CarbonBuilderEntry { Content = content });
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void PrepareBuild()
        {
            this.contentDictionary.Clear();
        }

        private void GatherContent(string currentPath, SourceProjectFolder currentFolder)
        {
            foreach (SourceFolderContent content in currentFolder.Contents)
            {
                var folder = content as SourceProjectFolder;
                if (folder != null)
                {
                    this.GatherContent(Path.Combine(currentPath, folder.Name), folder);
                    continue;
                }
                
                this.AddContent(new CarbonBuilderEntry { Type = content.GetType(), Path = currentPath, Content = content });
            }
        }

        private void AddContent(CarbonBuilderEntry entry)
        {
            if (string.IsNullOrEmpty(entry.Content.Name))
            {
                throw new InvalidDataException("Content name can not be null for export");
            }

            if (!contentProcessors.ContainsKey(entry.Type))
            {
                throw new InvalidDataException("No processor available for content with type " + entry.Type);
            }

            if (!this.contentDictionary.ContainsKey(entry.Type))
            {
                this.contentDictionary.Add(entry.Type, new List<CarbonBuilderEntry>());
            }

            this.contentDictionary[entry.Type].Add(entry);
        }

        private void ProcessContent()
        {
            foreach (Type contentType in this.contentDictionary.Keys)
            {
                int progressMax = this.contentDictionary[contentType].Count;
                int progressValue = 0;
                IContentProcessor processor = contentProcessors[contentType];
                foreach (CarbonBuilderEntry entry in this.contentDictionary[contentType])
                {
                    this.NotifyProgressChanged(string.Format("Processing {0} of type {1}", entry.Path, contentType), progressValue, progressMax);

                    string key = Path.Combine(entry.Path, entry.Content.Name);
                    using (var stream = new MemoryStream())
                    { 
                        processor.Process(entry);
                        stream.Position = 0;
                        var res = new ResourceLink { Source = key };
                        this.resourceManager.StoreOrReplace(ref res, new RawResource(stream));
                    }

                    progressValue++;
                }
            }
        }

        private void NotifyProgressChanged(string message, int min, int max)
        {
            var eventHandler = this.ProgressChanged;
            if (eventHandler != null)
            {
                eventHandler(message, min, max);
            }
        }
    }
}
