namespace GrandSeal.Editor.ViewModels
{
    using System;
    using System.IO;
    using System.Windows;

    using CarbonCore.Utils.Contracts.IoC;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Resource;
    using Core.Engine.Resource.Content;
    using Core.Processing.Contracts;
    using Core.Processing.Processors;

    using GrandSeal.Editor.Contracts;

    using ICSharpCode.AvalonEdit.Document;

    public class ResourceScriptViewModel : ResourceViewModel, IResourceScriptViewModel
    {
        private readonly IResourceProcessor resourceProcessor;

        private bool scriptWasChanged;

        private ITextSource scriptDocument;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceScriptViewModel(IFactory factory)
            : base(factory)
        {
            this.resourceProcessor = factory.Resolve<IResourceProcessor>();

            this.ForceSave = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ITextSource ScriptDocument
        {
            get
            {
                if (this.scriptDocument == null)
                {
                    this.scriptDocument = this.GetScriptingSource();
                    this.scriptDocument.TextChanged += this.OnScriptDocumentChanged;
                }

                return this.scriptDocument;
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void PrepareSave()
        {
            // See if we have changes in the script that need to be saved before export
            if (this.scriptWasChanged && this.scriptDocument != null)
            {
                Application.Current.Dispatcher.Invoke(this.SaveScript);
            }
        }

        protected override void DoSave(IContentManager target, IResourceManager resourceTarget)
        {
            ICarbonResource resource = this.resourceProcessor.ProcessScript(this.SourceFile, new ScriptProcessingOptions());

            if (resource != null)
            {
                resourceTarget.StoreOrReplace(this.Data.Hash, resource);
                this.NeedSave = false;
            }
            else
            {
                this.Log.Error("Failed to export Script resource {0}", null, this.SourcePath);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnScriptDocumentChanged(object sender, EventArgs e)
        {
            this.scriptWasChanged = true;
        }

        private ITextSource GetScriptingSource()
        {
            if (!this.SourceFile.Exists)
            {
                return new TextDocument();
            }

            using (var stream = this.SourceFile.OpenRead())
            {
                using (var reader = new StreamReader(stream))
                {
                    return new TextDocument(reader.ReadToEnd());
                }
            }
        }

        private void SaveScript()
        {
            using (var stream = this.SourceFile.OpenWrite())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(this.scriptDocument.Text);
                }
            }

            this.scriptWasChanged = false;
        }
    }
}
