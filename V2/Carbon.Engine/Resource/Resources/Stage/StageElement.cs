using System.Collections.Generic;

namespace Carbon.Engine.Resource.Resources.Stage
{
    public abstract class StageElement
    {
        public string Id { get; set; }

        public IList<bool> LayerFlags { get; set; }
        public IList<StagePropertyElement> Properties { get; set; }

        protected void LoadProperties(IList<Protocol.Resource.StageProperty> propertiesList)
        {
        }

        protected void LoadLayerData(int layerFlags)
        {
            throw new System.NotImplementedException();
        }

        protected IEnumerable<Protocol.Resource.StageProperty> SaveProperties()
        {
            throw new System.NotImplementedException();
        }

        protected int SaveLayerData()
        {
            throw new System.NotImplementedException();
        }

        private static int TranslateLayerFlags(int[] data)
        {
            int flags = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 1)
                {
                    flags = flags & 1 << i;
                }
            }

            return flags;
        }

        /*protected override void DoLoad(CarbonBinaryFormatter source)
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
        }*/

        
    }
}
