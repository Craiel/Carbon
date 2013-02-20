namespace Carbon.Engine.Contracts.Resource
{
    public interface ICarbonContent
    {
        // These are for undo / redo support
        ICarbonContent Clone(bool fullCopy = false);
        void LoadFrom(ICarbonContent source);
    }
}
