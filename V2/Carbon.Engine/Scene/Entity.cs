using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Logic;
using Carbon.Engine.Rendering;
using SlimDX;

namespace Carbon.Engine.Scene
{
    public interface IEntity : IEngineComponent, IRenderable
    {
        INode Parent { get; set; }

        bool WasUpdated { get; }

        string Name { get; set; }

        Vector4 Position { get; set; }
        Vector3 Scale { get; set; }
        Quaternion Rotation { get; set; }

        Matrix Local { get; }
        Matrix World { get; }

        BoundingSphere BoundingSphere { get; }
    }

    public abstract class Entity : EngineComponent, IEntity
    {
        private INode parent;
        private Vector4 position;
        private Vector3 scale = new Vector3(1);
        private Quaternion rotation = Quaternion.Identity;

        private bool needUpdate = true;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public INode Parent
        {
            get
            {
                return this.parent;
            }

            set
            {
                if (this.parent != value)
                {
                    this.parent = value;
                    this.needUpdate = true;
                }
            }
        }

        public string Name { get; set; }

        public Vector4 Position
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
                    this.needUpdate = true;
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
                    this.needUpdate = true;
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
                    this.needUpdate = true;
                }
            }
        }

        public Matrix Local { get; private set; }
        public Matrix World { get; private set; }
        public BoundingSphere BoundingSphere { get; set; }

        public bool WasUpdated { get; protected set; }

        public override void Update(Core.Utils.Contracts.ITimer gameTime)
        {
            if (this.needUpdate)
            {
                this.Local = Matrix.Scaling(this.scale) * Matrix.RotationQuaternion(this.rotation)
                             * Matrix.Translation(new Vector3(this.position.X, this.position.Y, this.position.Z));

                this.needUpdate = false;
                this.WasUpdated = true;

                if (parent == null)
                {
                    this.World = this.Local;
                    return;
                }
                
                this.World = this.Local * this.parent.World;
                return;
            }

            if (parent == null || !parent.WasUpdated)
            {
                return;
            }
            
            this.World = this.Local * this.parent.World;
            this.WasUpdated = true;
        }

        public abstract void Render(FrameInstructionSet frameSet);
    }
}
