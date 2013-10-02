namespace Core.Engine.Contracts.Scene
{
    using System.Collections.Generic;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;

    public interface IStage : IEngineComponent
    {
        IDictionary<string, IProjectionCamera> Cameras { get; }
        IDictionary<string, ILightEntity> Lights { get; }
        IList<IModelEntity> Models { get; }
        IList<IModelEntity> RootModels { get; }
        IDictionary<IModelEntity, IList<IModelEntity>> ModelHirarchy { get; }
    }
}
