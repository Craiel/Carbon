using Carbon.Engine.Contracts.Scene;
using Carbon.Engine.Logic;
using Carbon.Engine.Rendering;

using Core.Utils;

using SlimDX;

namespace Carbon.Engine.Scene
{
    public abstract class SceneEntity : EngineComponent, ISceneEntity
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        protected SceneEntity()
        {
            this.Scale = VectorExtension.Vector3Identity;
            this.Rotation = Quaternion.Identity;

            this.BoundingSphere = new BoundingSphere(new Vector3(0), 1);
            this.BoundingBox = new BoundingBox(new Vector3(0), new Vector3(1));
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }

        public Vector4 Position { get; set; }

        public Vector3 Scale { get; set; }

        public Quaternion Rotation { get; set; }

        public Matrix Local { get; private set; }
        public Matrix World { get; set; }

        public BoundingSphere BoundingSphere { get; set; }
        public BoundingBox BoundingBox { get; set; }
        
        public override bool Update(Core.Utils.Contracts.ITimer gameTime)
        {
            this.Local = Matrix.Scaling(this.Scale) * Matrix.RotationQuaternion(this.Rotation)
                         * Matrix.Translation(new Vector3(this.Position.X, this.Position.Y, this.Position.Z));

            return true;
        }

        public virtual void Render(FrameInstructionSet frameSet)
        {
        }
    }
}
