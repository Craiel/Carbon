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
            this.Location = new Vector3 { X = source.ReadSingle(), Y = source.ReadSingle(), Z = source.ReadSingle() };
            this.Direction = new Vector3 { X = source.ReadSingle(), Y = source.ReadSingle(), Z = source.ReadSingle() };
            this.Color = new Vector3 { X = source.ReadSingle(), Y = source.ReadSingle(), Z = source.ReadSingle() };

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
            target.Write(this.Location.X);
            target.Write(this.Location.Y);
            target.Write(this.Location.Z);

            target.Write(this.Direction.X);
            target.Write(this.Direction.Y);
            target.Write(this.Direction.Z);

            target.Write(this.Color.X);
            target.Write(this.Color.Y);
            target.Write(this.Color.Z);

            target.Write(this.Radius);
            target.Write(this.Intensity);
            target.Write(this.AmbientIntensity);
            target.Write(this.BeamWidth);
            target.Write(this.CutoffAngle);

            base.DoSave(target);
        }
    }
}
