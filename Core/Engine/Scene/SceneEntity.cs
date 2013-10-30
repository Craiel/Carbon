namespace Core.Engine.Scene
{
    using System;
    using System.Collections.Generic;

    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Rendering;

    using Core.Utils;

    using SharpDX;

    public abstract class SceneEntity : EngineComponent, ISceneEntity
    {
        private Quaternion rotation;
        private Vector3 position;
        private Vector3 scale;

        private bool needBoundingUpdate = true;

        private List<ISceneEntity> children;
        private List<ISceneEntity> parents; 

        private IScene linkedScene;
        private int linkedStack;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        protected SceneEntity()
        {
            this.Scale = VectorExtension.Vector3Identity;
            this.Rotation = Quaternion.Identity;

            this.Local = Matrix.Identity;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract bool CanRender { get; }

        public string Name { get; set; }

        public Vector3 Position
        {
            get
            {
                return this.position;
            }

            set
            {
                if (this.position != value)
                {
                    this.position = value;
                    this.UpdateLink();
                }
            }
        }

        public Vector3 Scale
        {
            get
            {
                return this.scale;
            }

            set
            {
                if (this.scale != value)
                {
                    this.scale = value;
                    this.needBoundingUpdate = true;
                    this.UpdateLink();
                }
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return this.rotation;
            }

            set
            {
                if (this.rotation != value)
                {
                    this.rotation = value;
                    this.UpdateLink();
                }
            }
        }

        public Matrix Local { get; private set; }

        public Matrix? OverrideWorld { get; set; }

        public BoundingSphere? BoundingSphere { get; set; }
        public BoundingBox? BoundingBox { get; set; }

        public IReadOnlyCollection<ISceneEntity> Parents
        {
            get
            {
                if (this.parents == null)
                {
                    return null;
                }

                return this.parents.AsReadOnly();
            }
        }
        
        public IReadOnlyCollection<ISceneEntity> Children
        {
            get
            {
                if (this.children == null)
                {
                    return null;
                }

                return this.children.AsReadOnly();
            }
        }

        public void AddChild(ISceneEntity child)
        {
            if (this.children == null)
            {
                this.children = new List<ISceneEntity>();
            }

            this.children.Add(child);
        }

        public void RemoveChild(ISceneEntity child)
        {
            this.children.Remove(child);
        }

        public void AddParent(ISceneEntity parent)
        {
            if (this.parents == null)
            {
                this.parents = new List<ISceneEntity>();
            }
            
            this.parents.Add(parent);
            parent.AddChild(this);
        }

        public void RemoveParent(ISceneEntity parent)
        {
            this.parents.Remove(parent);
            parent.RemoveChild(this);
        }

        // Clone is only cloning this entity without the children
        public virtual ISceneEntity Clone()
        {
            var clone = this.DoClone();
            clone.Name = this.Name;
            clone.Position = this.Position;
            clone.Rotation = this.Rotation;
            clone.Scale = this.Scale;
            return clone;
        }

        public void Link(Scene scene, int targetStack)
        {
            if (this.linkedScene != null)
            {
                throw new InvalidOperationException("Entity is already linked! Unlink before linking again");
            }

            this.linkedScene = scene;
            this.linkedStack = targetStack;
        }

        public void Unlink()
        {
            if (this.linkedScene == null)
            {
                throw new InvalidOperationException("Entity is not linked, avoid call to unlink!");
            }

            this.linkedScene = null;
        }

        public Matrix GetWorld()
        {
            if (this.Parents == null || this.Parents.Count <= 0)
            {
                return Matrix.Identity;
            }

            var matrixStack = new Stack<Matrix>();
            var parentQueue = new Queue<ISceneEntity>();
            parentQueue.Enqueue(this);
            while (parentQueue.Count > 0)
            {
                ISceneEntity current = parentQueue.Dequeue();
                if (current != this)
                {
                    matrixStack.Push(current.Local);
                }
                
                if (current.Parents != null)
                {
                    foreach (ISceneEntity parent in current.Parents)
                    {
                        // Todo: we don't know what to do with multiple parents yet
                        parentQueue.Enqueue(parent);
                        break;
                    }
                }
            }

            Matrix result = Matrix.Identity;
            while (matrixStack.Count > 0)
            {
                result *= matrixStack.Pop();
            }

            return result;
        }

        public override bool Update(Utils.Contracts.ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            if (this.needBoundingUpdate)
            {
                if (this.BoundingBox != null)
                {
                    this.BoundingBox = new BoundingBox(this.BoundingBox.Value.Minimum * this.scale, this.BoundingBox.Value.Maximum * this.scale);
                }

                this.needBoundingUpdate = false;
            }

            this.Local = MatrixExtension.GetLocalMatrix(this.scale, this.Rotation, this.Position);
            
            return true;
        }

        public virtual void Render(FrameInstructionSet frameSet)
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract ISceneEntity DoClone();

        protected void UpdateLink()
        {
            if (this.linkedScene == null)
            {
                return;
            }

            this.linkedScene.InvalidateSceneEntity(this, this.linkedStack);
        }
    }
}
