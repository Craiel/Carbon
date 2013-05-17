namespace Carbon.Engine.Resource.Resources.Stage
{
    using Carbon.Engine.Logic;

    using SlimDX;

    public enum StageLightType
    {
        Unknown = 0,
        Spot = 1,
        Direction = 2,
        Point = 3
    }

    public class StageLightElement : StageElement
    {
        internal enum LightFlags
        {
            None = 0,
            HasLocation = 1,
            HasDirection = 2,
            HasColor = 4,
        }

        public StageLightType Type { get; set; }

        public Vector3? Location { get; set; }
        public Vector3? Direction { get; set; }
        public Vector3? Color { get; set; }

        public float Radius { get; set; }
        public float Intensity { get; set; }
        public float AmbientIntensity { get; set; }
        public float SpotSize { get; set; }
        public float Angle { get; set; }

        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            this.Type = (StageLightType)source.ReadShort();

            uint flags = source.ReadUInt();
            bool hasLocation = (flags & (int)LightFlags.HasLocation) == (int)LightFlags.HasLocation;
            bool hasDirection = (flags & (int)LightFlags.HasDirection) == (int)LightFlags.HasDirection;
            bool hasColor = (flags & (int)LightFlags.HasColor) == (int)LightFlags.HasColor;

            if (hasLocation)
            {
                this.Location = new Vector3
                                    {
                                        X = source.ReadSingle(),
                                        Y = source.ReadSingle(),
                                        Z = source.ReadSingle()
                                    };
            }

            if (hasDirection)
            {
                this.Direction = new Vector3
                                     {
                                         X = source.ReadSingle(),
                                         Y = source.ReadSingle(),
                                         Z = source.ReadSingle()
                                     };
            }

            if (hasColor)
            {
                this.Color = new Vector3 { X = source.ReadSingle(), Y = source.ReadSingle(), Z = source.ReadSingle() };
            }

            this.Radius = source.ReadSingle();
            this.Intensity = source.ReadSingle();
            this.AmbientIntensity = source.ReadSingle();
            this.SpotSize = source.ReadSingle();
            this.Angle = source.ReadSingle();

            base.DoLoad(source);
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write((short)this.Type);

            target.Write(this.GetLightFlags());

            if (this.Location != null)
            {
                target.Write(this.Location.Value.X);
                target.Write(this.Location.Value.Y);
                target.Write(this.Location.Value.Z);
            }

            if (this.Direction != null)
            {
                target.Write(this.Direction.Value.X);
                target.Write(this.Direction.Value.Y);
                target.Write(this.Direction.Value.Z);
            }

            if (this.Color != null)
            {
                target.Write(this.Color.Value.X);
                target.Write(this.Color.Value.Y);
                target.Write(this.Color.Value.Z);
            }

            target.Write(this.Radius);
            target.Write(this.Intensity);
            target.Write(this.AmbientIntensity);
            target.Write(this.SpotSize);
            target.Write(this.Angle);

            base.DoSave(target);
        }

        private uint GetLightFlags()
        {
            uint flags = 0;
            if (this.Location != null)
            {
                flags |= (uint)LightFlags.HasLocation;
            }

            if (this.Direction != null)
            {
                flags |= (uint)LightFlags.HasDirection;
            }

            if (this.Color != null)
            {
                flags |= (uint)LightFlags.HasColor;
            }

            return flags;
        }
    }
}
