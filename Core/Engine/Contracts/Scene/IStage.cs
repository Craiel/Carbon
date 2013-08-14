namespace Core.Engine.Contracts.Scene
{
    using System.Collections.Generic;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Scene;

    public interface IStage : IEngineComponent
    {
        IDictionary<string, ICamera> Cameras { get; }
        IDictionary<string, ILight> Lights { get; }
        IDictionary<string, IModelEntity> Models { get; }
    }
}
