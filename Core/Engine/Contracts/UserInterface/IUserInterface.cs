using System.Collections.ObjectModel;

using Core.Engine.Contracts.Logic;
using Core.Engine.Contracts.Scene;

namespace Core.Engine.Contracts.UserInterface
{
    public interface IUserInterface : IEngineComponent
    {
        ReadOnlyCollection<ISceneEntity> Entities { get; }
    }
}
