using System;
using System.IO;
using System.Windows;

using Carbed.Contracts;

using Carbon.Engine.Contracts;
using Carbon.Engine.Resource.Content;

using ICSharpCode.AvalonEdit.Document;

namespace Carbed.ViewModels
{
    public class ResourceScriptViewModel : ResourceViewModel, IResourceScriptViewModel
    {
        private bool scriptWasChanged;
        private bool keepLocalScriptChanges;

        private ITextSource scriptDocument;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceScriptViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
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

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnScriptDocumentChanged(object sender, EventArgs e)
        {
            this.scriptWasChanged = true;
        }

        private ITextSource GetScriptingSource()
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

        private void UnloadScript()
        {
            if (this.scriptDocument != null)
            {
                this.scriptDocument.TextChanged -= this.OnScriptDocumentChanged;
                this.scriptDocument = null;
                this.NotifyPropertyChanged("ScriptDocument");
            }
        }

        private void HandleSourceScriptChange()
        {
            if (this.scriptWasChanged)
            {
                if (
                    MessageBox.Show(
                        "Source script was changed and you have un-saved modifications. Do you want to reload the source?\n(This will discard all local changes)",
                        "Source changed",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    this.keepLocalScriptChanges = true;
                    return;
                }
            }

            this.UnloadScript();
        }

        private void SaveScript()
        {
            using (var writer = new StreamWriter(this.SourcePath, false))
            {
                writer.Write(this.scriptDocument.Text);
            }

            this.scriptWasChanged = false;
            this.keepLocalScriptChanges = false;
        }
    }
}
