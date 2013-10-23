namespace Core.Engine.Scene
{
    using System.Collections.Generic;

    using Core.Engine.Contracts.Scene;
    using Core.Engine.Rendering;

    using SharpDX;

    public class ModelEntity : SceneEntity, IModelEntity
    {
        private Mesh mesh;

        private bool needMeshUpdate = true;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Mesh Mesh
        {
            get
            {
                return this.mesh;
            }

            set
            {
                if (this.mesh != value)
                {
                    this.mesh = value;
                    this.needMeshUpdate = true;
                }
            }
        }

        public Material Material { get; set; }

        public override bool Update(Utils.Contracts.ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            // Todo: Transform the boundingbox accordingly
            if (this.needMeshUpdate)
            {
                if (this.mesh != null)
                    {
                        if (this.mesh.BoundingBox != null)
                        {
                            this.BoundingBox = this.mesh.BoundingBox.Value;
                        }

                        if (this.mesh.BoundingSphere != null)
                        {
                            this.BoundingSphere = this.mesh.BoundingSphere.Value;
                        }
                    }
                else
                {
                    this.BoundingBox = null;
                    this.BoundingSphere = null;
                }

                this.needMeshUpdate = false;
            }

            return true;
        }

        public override void Render(FrameInstructionSet frameSet)
        {
            // Todo: Think about frustum culling some place
            /*BoundingBox boundingBox = mesh.BoundingBox.Transform(this.World);
            if (frameSet.Camera.Frustum.Contains(boundingBox) == ContainmentType.Disjoint)
            {
                return;
            }*/

            base.Render(frameSet);

            if (this.Mesh != null)
            {
                Matrix world = this.GetWorld();
                frameSet.Instructions.Add(new FrameInstruction { Mesh = this.Mesh, Material = this.Material, World = world * this.Local });
            }
        }

        private Matrix GetWorld()
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
                matrixStack.Push(current.Local);
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

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override ISceneEntity DoClone()
        {
            return new ModelEntity { mesh = this.mesh, Material = this.Material };
        }
    }
}
