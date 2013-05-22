using Carbed.Contracts;

using Carbon.Engine.Contracts;
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
{
    using System.Windows;

    using Carbon.Editor.Contracts;
    using Carbon.Editor.Processors;
    using Carbon.Editor.Resource.Xcd;
    using Carbon.Engine.Contracts.Resource;

    public class ResourceStageViewModel : ResourceViewModel, IResourceStageViewModel
    {
        private enum Flags
        {
        }

        private readonly ICarbedLogic logic;
        private readonly IResourceProcessor resourceProcessor;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceStageViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
            this.logic = factory.Get<ICarbedLogic>();
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
            var options = new XcdProcessingOptions();
            ICarbonResource resource = this.resourceProcessor.ProcessStage(this.SourcePath, options);
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
    }
}
