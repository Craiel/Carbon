namespace Core.Engine.Contracts.Scene
{
    using System.Collections.Generic;

    using Core.Engine.Contracts.Logic;
    
    public interface INode : IEngineComponent
    {
        string Name { get; }

        INode Parent { get; set; }

        ISceneEntity Entity { get; }

        IReadOnlyCollection<INode> Children { get; }

        void AddChild(INode node);
        void RemoveChild(INode node);

        void Clear();
    }
}
