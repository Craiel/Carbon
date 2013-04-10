using System;
using System.Collections.Generic;

using Carbon.Engine.Rendering;

namespace Carbon.Engine.Scene
{
    using Carbon.Engine.Contracts.Scene;

    public interface INode : IEntity
    {
        IReadOnlyCollection<IEntity> Children { get; }

        void AddChild(IEntity entity);
        void RemoveChild(IEntity entity);

        void Clear();
    }

    /// <summary>
    /// Container class for entities, used by all kinds of system to "bag" entities
    /// </summary>
    public class Node : Entity, INode
    {
        private readonly List<IEntity> children;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Node()
        {
            this.children = new List<IEntity>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReadOnlyCollection<IEntity> Children
        {
            get
            {
                return this.children.AsReadOnly();
            }
        }

        public override void Dispose()
        {
            this.Clear();

            base.Dispose();
        }

        public override void Update(Core.Utils.Contracts.ITimer gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < this.children.Count; i++)
            {
                this.children[i].Update(gameTime);
            }

            this.WasUpdated = false;
        }

        public override void Render(FrameInstructionSet frameSet)
        {
            for (int i = 0; i < this.children.Count; i++)
            {
                this.children[i].Render(frameSet);
            }
        }

        public void AddChild(IEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Entity was null");
            }

            if (this.children.Contains(entity))
            {
                throw new InvalidOperationException("Entity was already in child collection");
            }

            this.children.Add(entity);
            entity.Parent = this;
        }

        public void RemoveChild(IEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Entity was null");
            }

            if (!this.children.Contains(entity))
            {
                throw new InvalidOperationException("Entity was not in child collection");
            }

            this.children.Remove(entity);
            entity.Parent = null;
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
    }
}
