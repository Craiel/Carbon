namespace Core.Engine.Scene
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.Contracts;
    using CarbonCore.UtilsDX;

    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Rendering;
    
    using SharpDX;

    public abstract class SceneEntity : EngineComponent, ISceneEntity
    {
        private Quaternion rotation;
        private Vector3 position;
        private Vector3 scale;

        private bool needBoundingUpdate = true;
        private bool needLocalMatrixUpdate = true;

        private List<ISceneEntity> children;
        private List<ISceneEntity> parents;
        private List<Matrix> localTransforms;

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
                    this.needLocalMatrixUpdate = true;
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
                    this.needLocalMatrixUpdate = true;
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
                    this.needLocalMatrixUpdate = true;
                    this.UpdateLink();
                }
            }
        }

        public Matrix Local { get; protected set; }

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

        public IReadOnlyCollection<Matrix> LocalTransforms
        {
            get
            {
                return this.localTransforms;
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

        public void ClearChildren()
        {
            this.children.Clear();
            this.children = null;
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

        public void ClearParents()
        {
            this.parents.Clear();
            this.parents = null;
        }

        public void AddTransform(Matrix matrix)
        {
            if (this.localTransforms == null)
            {
                this.localTransforms = new List<Matrix>();
            }

            this.localTransforms.Add(matrix);
            this.needLocalMatrixUpdate = true;
        }

        public void RemoveTransform(Matrix matrix)
        {
            this.localTransforms.Remove(matrix);
            this.needLocalMatrixUpdate = true;
        }

        public void ClearTransforms()
        {
            this.localTransforms.Clear();
            this.localTransforms = null;
            this.needLocalMatrixUpdate = true;
        }

        // Clone is only cloning this entity without the children
        public virtual ISceneEntity Clone()
        {
            var clone = this.DoClone();
            clone.Name = this.Name;
            clone.Position = this.Position;
            clone.Rotation = this.Rotation;
            clone.Scale = this.Scale;

            // Clone the custom transforms as well
            if (this.localTransforms != null)
            {
                foreach (Matrix matrix in this.localTransforms)
                {
                    clone.AddTransform(matrix);
                }
            }

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

        public override bool Update(ITimer gameTime)
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

            if (this.needLocalMatrixUpdate)
            {
                Matrix newLocal = MatrixExtension.GetLocalMatrix(this.scale, this.Rotation, this.Position);

                if (this.localTransforms != null)
                {
                    foreach (Matrix localTransform in this.localTransforms)
                    {
                        newLocal = newLocal * localTransform;
                    }
                }

                this.Local = newLocal;
            }

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
