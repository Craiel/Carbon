namespace Carbon.Engine.Resource.Resources.Stage
{
    using Carbon.Engine.Logic;

    public class StageElement : ResourceBase
    {
        public string Id { get; set; }

        public int LayerFlags { get; set; }

        public StagePropertyElement[] Properties { get; set; }

        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            this.LayerFlags = source.ReadInt();

            short count = source.ReadShort();
            if (count > 0)
            {
                this.Properties = new StagePropertyElement[count];
                for (int i = 0; i < count; i++)
                {
                    short type = source.ReadShort();
                    switch ((StagePropertyType)type)
                    {
                        case StagePropertyType.String:
                            {
                                this.Properties[i] = new StagePropertyElementString();
                                this.Properties[i].Load(source);
                                break;
                            }

                        case StagePropertyType.Float:
                            {
                                this.Properties[i] = new StagePropertyElementFloat();
                                this.Properties[i].Load(source);
                                break;
                            }

                        case StagePropertyType.Int:
                            {
                                this.Properties[i] = new StagePropertyElementInt();
                                this.Properties[i].Load(source);
                                break;
                            }
                    }
                }
            }

            this.Id = source.ReadString();
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(this.LayerFlags);

            if (this.Properties == null)
            {
                target.Write((short)0);
            }
            else
            {
                target.Write(this.Properties.Length);
                for (int i = 0; i < this.Properties.Length; i++)
                {
                    target.Write((short)this.Properties[i].Type);
                    this.Properties[i].Save(target);
                }
            }

            target.Write(this.Id);
        }
    }
}
