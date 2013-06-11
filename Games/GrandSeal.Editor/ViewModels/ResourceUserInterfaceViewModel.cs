using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using Core.Editor.Contracts;
using Core.Editor.Processors;
using Core.Engine.Contracts;
using Core.Engine.Contracts.Resource;
using Core.Engine.Resource.Content;

using GrandSeal.Editor.Contracts;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;

namespace GrandSeal.Editor.ViewModels
{
    public class ResourceUserInterfaceViewModel : ResourceViewModel, IResourceUserInterfaceViewModel
    {
        private readonly IResourceProcessor resourceProcessor;

        private bool interfaceWasChanged;

        private IResourceScriptViewModel scriptViewModel;
        private ITextSource interfaceDocument;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceUserInterfaceViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
            this.resourceProcessor = factory.Get<IResourceProcessor>();
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
            ICarbonResource resource = this.resourceProcessor.ProcessUserInterface(this.SourcePath, new UserInterfaceProcessingOptions());

            if (resource != null)
            {
                resourceTarget.StoreOrReplace(this.Data.Hash, resource);
                this.NeedSave = false;
            }
            else
            {
                this.Log.Error("Failed to export User Interface resource {0}", null, this.SourcePath);
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
            if (!File.Exists(this.SourcePath))
            {
                return new TextDocument();
            }

            using (var reader = new StreamReader(this.SourcePath))
            {
                return new TextDocument(reader.ReadToEnd());
            }
        }

        private void SaveInterface()
        {
            using (var writer = new StreamWriter(this.SourcePath, false))
            {
                writer.Write(this.interfaceDocument.Text);
            }

            this.interfaceWasChanged = false;
        }
    }
}
