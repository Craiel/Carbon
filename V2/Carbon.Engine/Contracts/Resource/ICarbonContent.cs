using Carbon.Engine.Resource.Content;

namespace Carbon.Engine.Contracts.Resource
{
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
