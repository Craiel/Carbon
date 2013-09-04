namespace Core.Engine.Rendering
{
    using System;

    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Logic;
    using Core.Engine.Rendering.Camera;

    using SharpDX;

    public enum LightType
    {
        Ambient,
        Direction,
        Point,
        Spot
    }

    public class Light : EngineComponent, ILight
    {
        private LightType type;

        private Vector3 direction;

        private Vector2 spotAngles;

        private float range;

        private Matrix view;

        private Matrix projection;

        private Vector3 position;

        private bool isCastingShadow;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Light()
        {
            this.Type = LightType.Ambient;
            this.SpecularPower = 1.0f;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsCastingShadow
        {
            get
            {
                return this.isCastingShadow;
            }

            set
            {
                if (this.isCastingShadow != value)
                {
                    this.isCastingShadow = value;
                    this.CheckLightViewProjectionUpdate();
                }
            }
        }

        public bool NeedShadowUpdate { get; set; }

        public LightType Type
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
                    this.CheckLightViewProjectionUpdate();
                }
            }
        }

        public Vector4 Color { get; set; }

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
                    this.CheckLightViewProjectionUpdate();
                }
            }
        }
        
        public Vector3 Direction
        {
            get
            {
                return this.direction;
            }

            set
            {
                if (this.direction != value)
                {
                    this.direction = value;
                    this.CheckLightViewProjectionUpdate();
                }
            }
        }

        public Vector2 SpotAngles
        {
            get
            {
                return this.spotAngles;
            }

            set
            {
                if (this.spotAngles != value)
                {
                    this.spotAngles = value;
                    this.CheckLightViewProjectionUpdate();
                }
            }
        }

        public float Range
        {
            get
            {
                return this.range;
            }

            set
            {
                System.Diagnostics.Debug.Assert(false, "This code needs to be tested");
                if (Math.Abs(this.range - value) > float.Epsilon)
                {
                    this.range = value;
                    this.CheckLightViewProjectionUpdate();
                }
            }
        }

        public float SpecularPower { get; set; }

        public Matrix View
        {
            get
            {
                return this.view;
            }
        }

        public Matrix Projection
        {
            get
            {
                return this.projection;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void CheckLightViewProjectionUpdate()
        {
            this.view = Matrix.Identity;
            this.projection = Matrix.Identity;
            if (this.type == LightType.Spot)
            {
                this.UpdateLightViewProjection();
            }
        }

        private void UpdateLightViewProjection()
        {
            if (!this.IsCastingShadow)
            {
                return;
            }

            lock (ProjectionCamera.Camera)
            {
                // Todo: calculate proper view / projection for the spot parameters
                ProjectionCamera.Camera.SetPerspective(new TypedVector2<int>(1), 0.05f, this.range, (float)Math.PI / 2.0f);
                ProjectionCamera.Camera.Position = this.position;
                // Todo: clean this up, confusing what lookat is in this context
                //       also light direction and camera direction are not the same
                ProjectionCamera.Camera.LookAt(-this.direction);
                ProjectionCamera.Camera.Update(null);

                this.view = ProjectionCamera.Camera.View;
                this.projection = ProjectionCamera.Camera.Projection;

                // Todo: this is quite the hack but we have no better way of communicating with the frame manager at the moment from here
                this.NeedShadowUpdate = true;
            }
        }
    }
}
