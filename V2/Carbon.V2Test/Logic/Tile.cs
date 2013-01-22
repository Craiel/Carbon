using Carbon.Engine.Contracts.Logic;
using Core.Utils.Contracts;
using SlimDX;

namespace Carbon.V2Test.Logic
{
    public enum TileType
    {
        L,
        LReverse,
        S,
        SReverse,
        T,
        Line,
        Cube
    }

    public enum TileMovement
    {
        None,
        Left,
        Right,
        Down,
        RotateLeft,
        RotateRight
    }

    public interface ITile
    {
        Vector3 Origin { get; }
        Quaternion Rotation { get; }
        Vector4[] Segments { get; }

        TileType Type { get; set; }
        
        void MoveLeft();
        void MoveRight();
        void MoveDown();

        void RotateLeft();
        void RotateRight();

        void Destroy();

        void Update(ITimer gameTime);
    }

    public class Tile : ITile
    {
        private Vector3 origin;
        private Quaternion rotation;
        private Vector4[] segments;
        private TileType type;
        private float rotationAngle;

        private bool needUpdate = true;

        public TileType Type
        {
            get
            {
                return this.type;
            }

            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.needUpdate = true;
                }
            }
        }

        public Vector3 Origin
        {
            get
            {
                return this.origin;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return Quaternion.RotationAxis(new Vector3(0, 0, 1), this.rotationAngle);
            }
        }

        public Vector4[] Segments
        {
            get
            {
                return this.segments;
            }
        }

        public void MoveLeft()
        {
            this.origin -= new Vector3(1.0f, 0, 0);
        }

        public void MoveRight()
        {
            this.origin += new Vector3(1.0f, 0, 0);
        }

        public void MoveDown()
        {
            this.origin -= new Vector3(0, 1.0f, 0);
        }

        public void RotateLeft()
        {
            this.rotationAngle -= 90.0f;
        }

        public void RotateRight()
        {
            this.rotationAngle += 90.0f;
        }

        public void Destroy()
        {
        }

        public void Update(ITimer gameTime)
        {
            if (this.needUpdate)
            {
                this.UpdateTileStatus();
            }
        }

        private void UpdateTileStatus()
        {
            this.rotation = Quaternion.Identity;
            this.segments = new Vector4[4];
            switch (this.Type)
            {
                case TileType.Line:
                    {
                        this.segments[0] = new Vector4(0, 0.0f, 0, 2.0f);
                        this.segments[1] = new Vector4(0, 2.0f, 0, 2.0f);
                        this.segments[2] = new Vector4(0, 4.0f, 0, 2.0f);
                        this.segments[3] = new Vector4(0, 6.0f, 0, 2.0f);
                        this.origin = new Vector3(0, 1.0f, 0);
                        break;
                    }

                case TileType.Cube:
                    {
                        this.segments[0] = new Vector4(0, 0, 0, 2.0f);
                        this.segments[1] = new Vector4(2.0f, 0, 0, 2.0f);
                        this.segments[2] = new Vector4(2.0f, 2.0f, 0, 2.0f);
                        this.segments[3] = new Vector4(0, 2.0f, 0, 2.0f);
                        this.origin = new Vector3(1.0f, 1.0f, 0);
                        break;
                    }

                case TileType.T:
                    {
                        this.segments[0] = new Vector4(0, 0, 0, 2.0f);
                        this.segments[1] = new Vector4(2.0f, 0, 0, 2.0f);
                        this.segments[2] = new Vector4(4.0f, 0, 0, 2.0f);
                        this.segments[3] = new Vector4(2.0f, 2.0f, 0, 2.0f);
                        this.origin = new Vector3(0, 1.0f, 0);
                        break;
                    }

                case TileType.S:
                    {
                        this.segments[0] = new Vector4(0, 0, 0, 2.0f);
                        this.segments[1] = new Vector4(2.0f, 0, 0, 2.0f);
                        this.segments[2] = new Vector4(2.0f, 2.0f, 0, 2.0f);
                        this.segments[3] = new Vector4(4.0f, 2.0f, 0, 2.0f);
                        this.origin = new Vector3(0, 1.0f, 0);
                        break;
                    }

                case TileType.SReverse:
                    {
                        this.segments[0] = new Vector4(0, 2.0f, 0, 2.0f);
                        this.segments[1] = new Vector4(2.0f, 2.0f, 0, 2.0f);
                        this.segments[2] = new Vector4(2.0f, 0, 0, 2.0f);
                        this.segments[3] = new Vector4(4.0f, 0, 0, 2.0f);
                        this.origin = new Vector3(0, 1.0f, 0);
                        break;
                    }

                case TileType.L:
                    {
                        this.segments[0] = new Vector4(0, 0, 0, 2.0f);
                        this.segments[1] = new Vector4(0, 2.0f, 0, 2.0f);
                        this.segments[2] = new Vector4(0, 4.0f, 0, 2.0f);
                        this.segments[3] = new Vector4(2.0f, 0, 0, 2.0f);
                        this.origin = new Vector3(0, 1.0f, 0);
                        break;
                    }

                case TileType.LReverse:
                    {
                        this.segments[0] = new Vector4(2.0f, 0, 0, 2.0f);
                        this.segments[1] = new Vector4(2.0f, 2.0f, 0, 2.0f);
                        this.segments[2] = new Vector4(2.0f, 4.0f, 0, 2.0f);
                        this.segments[3] = new Vector4(0, 0, 0, 2.0f);
                        this.origin = new Vector3(0, 1.0f, 0);
                        break;
                    }
            }

            this.needUpdate = false;
        }
    }
}
