using System.Collections.ObjectModel;

using Core.Engine.Contracts.Logic;

namespace Core.Engine.Contracts.Scene
{
    public interface IStage : IEngineComponent
    {
        ReadOnlyCollection<ISceneEntity> Entities { get; }
    }
}
