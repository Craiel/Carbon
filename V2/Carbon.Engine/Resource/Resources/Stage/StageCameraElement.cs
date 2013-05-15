namespace Carbon.Engine.Resource.Resources.Stage
{
    using Carbon.Engine.Logic;

    using SlimDX;
    
    public class StageCameraElement : StageElement
    {
        public Vector3 Position { get; set; }
        public Vector4 Orientation { get; set; }

        public float FieldOfView { get; set; }

        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            this.Position = new Vector3 { X = source.ReadSingle(), Y = source.ReadSingle(), Z = source.ReadSingle() };

            this.Orientation = new Vector4
                                   {
                                       X = source.ReadSingle(),
                                       Y = source.ReadSingle(),
                                       Z = source.ReadSingle(),
                                       W = source.ReadSingle()
                                   };

            this.FieldOfView = source.ReadSingle();

            base.DoLoad(source);
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(this.Position.X);
            target.Write(this.Position.Y);
            target.Write(this.Position.Z);

            target.Write(this.Orientation.X);
            target.Write(this.Orientation.Y);
            target.Write(this.Orientation.Z);
            target.Write(this.Orientation.W);

            target.Write(this.FieldOfView);

            base.DoSave(target);
        }
    }
}
