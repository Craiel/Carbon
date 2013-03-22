using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Scene;

using SlimDX;

namespace Carbon.Engine.Contracts.Scene
{
    public interface IEntity : IEngineComponent, IRenderable
    {
        INode Parent { get; set; }

        bool WasUpdated { get; }

        string Name { get; set; }

        Vector4 Position { get; set; }
        Vector3 Scale { get; set; }
        Quaternion Rotation { get; set; }

        Matrix Local { get; }
        Matrix World { get; }

        BoundingSphere BoundingSphere { get; }
    }
}
