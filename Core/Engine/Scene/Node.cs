namespace Core.Engine.Scene
{
    using System;
    using System.Collections.Generic;

    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;

    using SharpDX;

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
        public Quaternion Rotation { get; set; }

        public IReadOnlyCollection<INode> Children
        {
            get
            {
                return this.children.AsReadOnly();
            }
        }

        public string Name { get; set; }

        public INode Parent { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Scale { get; set; }

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

        public override bool Update(Utils.Contracts.ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            return true;
        }
    }
}
