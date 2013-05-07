using Carbon.Engine.Contracts.Scene;
using Carbon.Engine.Rendering;

namespace Carbon.Engine.Scene
{
    public interface IModelEntity : ISceneEntity
    {
        Mesh Mesh { get; set; }
        Material Material { get; set; }
    }

    public class ModelEntity : SceneEntity, IModelEntity
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Mesh Mesh { get; set; }
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

        public override bool Update(Core.Utils.Contracts.ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            // Todo: Recalculate Bounding Box

            return true;
        }

        public override void Render(FrameInstructionSet frameSet)
        {
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
