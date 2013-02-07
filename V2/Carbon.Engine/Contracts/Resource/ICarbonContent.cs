namespace Carbon.Engine.Contracts.Resource
{
    public interface ICarbonContent
    {
        // These are for undo / redo support
        ICarbonContent Clone();
        void LoadFrom(ICarbonContent source);
    }
}
