namespace Core.Engine.Contracts.Scene
{
    using System.Collections.Generic;

    using Core.Engine.Contracts.Logic;

    using SharpDX;

    public interface IEntityNode : INode
    {
        ISceneEntity Entity { get; set; }

        void ResetTransformations();
    }

    public interface INode : IEngineComponent
    {
        string Name { get; set; }

        INode Parent { get; set; }

        Vector3 Position { get; set; }
        Vector3 Scale { get; set; }
        Quaternion Rotation { get; set; }

        IReadOnlyCollection<INode> Children { get; }

        void AddChild(INode node);
        void RemoveChild(INode node);

        void Clear();
    }
}
