namespace Carbon.Engine.Resource.Resources.Stage
{
    using Carbon.Engine.Logic;

    using SlimDX;

    public class StageModelElement : StageElement
    {
        public Vector3 Translation { get; set; }
        public Vector4 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            this.Translation = new Vector3 { X = source.ReadSingle(), Y = source.ReadSingle(), Z = source.ReadSingle() };

            this.Rotation = new Vector4
            {
                X = source.ReadSingle(),
                Y = source.ReadSingle(),
                Z = source.ReadSingle(),
                W = source.ReadSingle()
            };

            this.Scale = new Vector3 { X = source.ReadSingle(), Y = source.ReadSingle(), Z = source.ReadSingle() };
            
            base.DoLoad(source);
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(this.Translation.X);
            target.Write(this.Translation.Y);
            target.Write(this.Translation.Z);

            target.Write(this.Rotation.X);
            target.Write(this.Rotation.Y);
            target.Write(this.Rotation.Z);
            target.Write(this.Rotation.W);

            target.Write(this.Scale.X);
            target.Write(this.Scale.Y);
            target.Write(this.Scale.Z);

            base.DoSave(target);
        }
    }
}
