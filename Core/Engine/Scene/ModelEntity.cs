namespace Core.Engine.Scene
{
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Rendering;

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

        public override void Dispose()
        {
            if (this.Mesh != null)
            {
                this.Mesh.Dispose();
            }

            if (this.Material != null)
            {
                this.Material.Dispose();
            }

            base.Dispose();
        }

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
                frameSet.Instructions.Add(new FrameInstruction { Mesh = this.Mesh, Material = this.Material, World = this.World });
            }
        }
    }
}
