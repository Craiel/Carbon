namespace GrandSeal.Editor.ViewModels
{
    using System.Windows;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Resource;
    using Core.Engine.Resource.Content;
    using Core.Processing.Contracts;
    using Core.Processing.Resource.Xcd;

    using GrandSeal.Editor.Contracts;

    public class ResourceStageViewModel : ResourceViewModel, IResourceStageViewModel
    {
        private enum Flags
        {
        }

        private readonly IEditorLogic logic;
        private readonly IResourceProcessor resourceProcessor;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceStageViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
            this.logic = factory.Get<IEditorLogic>();
            this.resourceProcessor = factory.Get<IResourceProcessor>();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnDelete(object arg)
        {
            if (MessageBox.Show(
                "Delete stage " + this.Name,
                "Confirmation",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            this.OnClose(null);
            this.logic.Delete(this);
        }
        
        protected override void DoSave(IContentManager target, IResourceManager resourceTarget)
        {
            var options = new XcdProcessingOptions { ReferenceResolver = this.OnResolveReference };
            ICarbonResource resource = this.resourceProcessor.ProcessStage(this.SourceFile, options);
            if (resource != null)
            {
                resourceTarget.StoreOrReplace(this.Data.Hash, resource);
                this.NeedSave = false;
            }
            else
            {
                this.Log.Error("Failed to export Stage resource {0}", null, this.SourcePath);
            }
        }

        private string OnResolveReference(string reference)
        {
            // Todo

            return reference;
        }
    }
}
