using Core.Engine.Contracts.Logic;

namespace Core.Engine.Contracts.UserInterface
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Core.Engine.Contracts.Scene;

    public interface IUserInterface : IEngineComponent
    {
        ReadOnlyCollection<ISceneEntity> Entities { get; }
    }
}
