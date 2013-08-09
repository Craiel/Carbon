namespace Core.Engine.Contracts.Logic
{
    public interface IBoundController : IEngineComponent, IInputReceiver
    {
        new bool IsActive { get; set; }

        void SetInputBindings(string name);
    }
}
