using Carbon.Engine.Rendering;
using Carbon.Engine.Resource;

namespace Carbon.Engine.Scene
{
    public interface IModelNode : INode
    {
        Mesh Mesh { get; set; }
        Material Material { get; set; }
    }

    public class ModelNode : Node, IModelNode
    {
        private Mesh mesh;
        private Material material;

        private bool needUpdate;

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
                    this.needUpdate = true;
                }
            }
        }

        public Material Material
        {
            get
            {
                return this.material;
            }

            set
            {
                if (this.material != value)
                {
                    this.material = value;
                    this.needUpdate = true;
                }
            }
        }

        public override void Dispose()
        {
            if (this.mesh != null)
            {
                this.mesh.Dispose();
            }

            if (this.material != null)
            {
                this.material.Dispose();
            }

            base.Dispose();
        }

        public override void Update(Core.Utils.Contracts.ITimer gameTime)
        {
            base.Update(gameTime);

            if(this.needUpdate)
            {
                
            }
            // Todo: Recalculate Bounding Box
        }

        public override void Render(FrameInstructionSet frameSet)
        {
            /*BoundingBox boundingBox = mesh.BoundingBox.Transform(this.World);
            if (frameSet.Camera.Frustum.Contains(boundingBox) == ContainmentType.Disjoint)
            {
                return;
            }*/

            base.Render(frameSet);

            if (this.mesh != null)
            {
                frameSet.Instructions.Add(new FrameInstruction { Mesh = this.mesh, Material = this.Material, World = this.World });
            }
        }
    }
}
