namespace GrandSeal.Editor.ViewModels
{
    using System.Windows;

    using CarbonCore.Processing.Contracts;
    using CarbonCore.Processing.Source.Xcd;
    using CarbonCore.Utils.Compat.IO;
    using CarbonCore.Utils.Contracts.IoC;

    using Core.Engine.Contracts.Resource;

    using GrandSeal.Editor.Contracts;
    
    public class ResourceStageViewModel : ResourceViewModel, IResourceStageViewModel
    {
        private readonly IEditorLogic logic;
        private readonly IResourceProcessor resourceProcessor;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceStageViewModel(IFactory factory)
            : base(factory)
        {
            this.logic = factory.Resolve<IEditorLogic>();
            this.resourceProcessor = factory.Resolve<IResourceProcessor>();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnDelete()
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

            this.OnClose();
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
                System.Diagnostics.Trace.TraceError("Failed to export Stage resource {0}", null, this.SourcePath);
            }
        }

        private string OnResolveReference(string reference)
        {
            var resolveRoot = this.SourcePath.ToAbsolute<CarbonDirectory>(this.logic.ProjectLocation);
            var referenceFile = new CarbonFile(reference).ToAbsolute<CarbonFile>(resolveRoot);
            IResourceViewModel resource = this.logic.LocateResource(referenceFile);
            if (resource == null)
            {
                System.Diagnostics.Trace.TraceWarning("Reference could not be resolved: " + reference);
                return null;
            }

            System.Diagnostics.Trace.TraceInformation("Resolved reference {0} as {1}", reference, resource.Name);
            return resource.Hash;
        }
    }
}
