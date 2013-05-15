namespace Carbon.Engine.Resource.Resources.Stage
{
    using Carbon.Engine.Logic;
    public enum StagePropertyType
    {
        Unknown = 0,
        String = 1,
        Float = 2,
        Int = 3
    }

    public abstract class StagePropertyElement : ResourceBase
    {
        public string Id { get; set; }

        public StagePropertyType Type { get; protected set; }

        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            this.Id = source.ReadString();
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(this.Id);
        }
    }

    public class StagePropertyElementString : StagePropertyElement
    {
        public StagePropertyElementString()
        {
            this.Type = StagePropertyType.String;
        }

        public string Value { get; set; }
        
        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            base.DoLoad(source);
            this.Value = source.ReadString();
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            base.DoSave(target);
            target.Write(this.Value);
        }
    }

    public class StagePropertyElementFloat : StagePropertyElement
    {
        public StagePropertyElementFloat()
        {
            this.Type = StagePropertyType.Float;
        }

        public float Value { get; set; }
        
        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            base.DoLoad(source);
            this.Value = source.ReadSingle();
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            base.DoSave(target);
            target.Write(this.Value);
        }
    }

    public class StagePropertyElementInt : StagePropertyElement
    {
        public StagePropertyElementInt()
        {
            this.Type = StagePropertyType.Int;
        }

        public int Value { get; set; }
        
        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            base.DoLoad(source);
            this.Value = source.ReadInt();
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            base.DoSave(target);
            target.Write(this.Value);
        }
    }
}
