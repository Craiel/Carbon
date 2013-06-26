namespace Core.Engine.Contracts.Scene
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Core.Engine.Contracts.Logic;

    public interface IStage : IEngineComponent
    {
        ReadOnlyCollection<ISceneEntity> Entities { get; }
    }
}
