namespace GrandSeal.Editor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;

    using CarbonCore.Processing.Contracts;
    using CarbonCore.Processing.Processors;
    using CarbonCore.Utils.Contracts.IoC;

    using Core.Engine.Contracts.Resource;

    using GrandSeal.Editor.Contracts;

    using ICSharpCode.AvalonEdit.CodeCompletion;
    using ICSharpCode.AvalonEdit.Document;

    public class ResourceUserInterfaceViewModel : ResourceViewModel, IResourceUserInterfaceViewModel
    {
        private readonly IResourceProcessor resourceProcessor;

        private bool interfaceWasChanged;

        private IResourceScriptViewModel scriptViewModel;
        private ITextSource interfaceDocument;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceUserInterfaceViewModel(IFactory factory)
            : base(factory)
        {
            this.resourceProcessor = factory.Resolve<IResourceProcessor>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ITextSource CsamlDocument
        {
            get
            {
                if (this.interfaceDocument == null)
                {
                    this.interfaceDocument = this.GetInterfaceSource();
                    this.interfaceDocument.TextChanged += this.OnInterfaceDocumentChanged;
                }

                return this.interfaceDocument;
            }
        }

        public ITextSource ScriptDocument
        {
            get
            {
                return this.scriptViewModel.ScriptDocument;
            }
        }

        public void UpdateScriptAutoCompletion(IList<ICompletionData> completionList, string context = null)
        {
            this.scriptViewModel.UpdateAutoCompletion(completionList, context);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void PrepareSave()
        {
            base.PrepareSave();

            // See if we have changes in the script that need to be saved before export
            if (this.interfaceWasChanged && this.interfaceDocument != null)
            {
                Application.Current.Dispatcher.Invoke(this.SaveInterface);
            }
        }

        protected override void DoSave(IContentManager target, IResourceManager resourceTarget)
        {
            ICarbonResource resource = this.resourceProcessor.ProcessUserInterface(this.SourceFile, new UserInterfaceProcessingOptions());

            if (resource != null)
            {
                resourceTarget.StoreOrReplace(this.Data.Hash, resource);
                this.NeedSave = false;
            }
            else
            {
                System.Diagnostics.Trace.TraceError("Failed to export User Interface resource {0}", null, this.SourcePath);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnInterfaceDocumentChanged(object sender, EventArgs e)
        {
            this.interfaceWasChanged = true;
        }

        private ITextSource GetInterfaceSource()
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

        private void SaveInterface()
        {
            using (var stream = this.SourceFile.OpenWrite())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(this.interfaceDocument.Text);
                }
            }

            this.interfaceWasChanged = false;
        }
    }
}
