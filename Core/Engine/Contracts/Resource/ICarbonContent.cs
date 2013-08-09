namespace Core.Engine.Contracts.Resource
{
    using Core.Engine.Resource.Content;

    public interface ICarbonContent
    {
        bool IsNew { get; }
        bool IsChanged { get; }

        MetaDataTargetEnum MetaDataTarget { get; }
        
        // These are for undo / redo support
        ICarbonContent Clone(bool fullCopy = false);
        void LoadFrom(ICarbonContent source);

        void LockChangeState();
        void Invalidate();
    }
}
