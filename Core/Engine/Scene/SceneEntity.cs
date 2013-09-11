namespace Core.Engine.Scene
{
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Rendering;

    using Core.Utils;

    using SharpDX;

    public abstract class SceneEntity : EngineComponent, ISceneEntity
    {
        private Vector3 scale;

        private bool needBoundingUpdate = true;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        protected SceneEntity()
        {
            this.Scale = VectorExtension.Vector3Identity;
            this.Rotation = Quaternion.Identity;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }

        public Vector3 Position { get; set; }

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
                }
            }
        }

        public Quaternion Rotation { get; set; }

        public Matrix Local { get; private set; }
        public Matrix World { get; set; }

        public BoundingSphere? BoundingSphere { get; set; }
        public BoundingBox? BoundingBox { get; set; }
        
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

            this.Local = Matrix.Scaling(this.Scale) * Matrix.RotationQuaternion(this.Rotation)
                         * Matrix.Translation(new Vector3(this.Position.X, this.Position.Y, this.Position.Z));

            // Todo: check if this is not obsolete
            this.World = this.Local;
            return true;
        }

        public virtual void Render(FrameInstructionSet frameSet)
        {
        }
    }
}
