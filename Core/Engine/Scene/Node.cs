using System;
using System.Collections.Generic;

using Core.Engine.Contracts.Logic;
using Core.Engine.Contracts.Scene;
using Core.Engine.Logic;

namespace Core.Engine.Scene
{
    public interface INode : IEngineComponent
    {
        INode Parent { get; set; }

        ISceneEntity Entity { get; set; }

        IReadOnlyCollection<INode> Children { get; }
        
        void AddChild(INode node);
        void RemoveChild(INode node);

        void Clear();
    }

    /// <summary>
    /// Container class for entities, used by all kinds of system to "bag" entities
    /// </summary>
    public class Node : EngineComponent, INode
    {
        private readonly List<INode> children;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Node()
        {
            this.children = new List<INode>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReadOnlyCollection<INode> Children
        {
            get
            {
                return this.children.AsReadOnly();
            }
        }

        public INode Parent { get; set; }

        public ISceneEntity Entity { get; set; }

        public override void Dispose()
        {
            this.Clear();

            base.Dispose();
        }

        public void AddChild(INode node)
        {
            if (node == null || node.Parent != null)
            {
                throw new ArgumentException("Entity was null or invalid");
            }

            if (this.children.Contains(node))
            {
                throw new InvalidOperationException("Entity was already in child collection");
            }

            this.children.Add(node);
            node.Parent = this;
        }

        public void RemoveChild(INode node)
        {
            if (node == null)
            {
                throw new ArgumentException("Entity was null");
            }

            if (!this.children.Contains(node))
            {
                throw new InvalidOperationException("Entity was not in child collection");
            }

            this.children.Remove(node);
            node.Parent = null;
        }

        public void Clear()
        {
            if (this.children.Count > 0)
            {
                for (int i = 0; i < this.children.Count; i++)
                {
                    this.children[i].Dispose();
                }

                this.children.Clear();
            }
        }

        public override bool Update(Core.Utils.Contracts.ITimer gameTime)
        {
            if (this.Entity != null)
            {
                this.Entity.World = this.Entity.Local * this.Parent.Entity.Local;
            }
            
            return true;
        }
    }
}
