namespace Core.Engine.Scene
{
    using CarbonCore.Utils.Contracts;

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

        public override bool CanRender
        {
            get
            {
                // No mesh will not allow this to go into the rendering lists
                return this.mesh != null;
            }
        }

        public override bool Update(ITimer gameTime)
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
                Matrix world = this.OverrideWorld ?? this.GetWorld();
                frameSet.Instructions.Add(new FrameInstruction { Mesh = this.Mesh, Material = this.Material, World = world * this.Local });
            }
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
