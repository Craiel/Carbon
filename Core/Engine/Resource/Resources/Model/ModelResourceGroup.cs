namespace Core.Engine.Resource.Resources.Model
{
    using System.Collections.Generic;
    using System.IO;

    using Core.Utils;

    using SlimDX;

    public class ModelResourceGroup : ProtocolResource
    {
        private const int Version = 1;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModelResourceGroup()
        {
            this.Scale = new Vector3(1);
        }

        public ModelResourceGroup(Protocol.Resource.ModelGroup data)
            : this()
        {
            this.DoLoad(data);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }
        
        public Vector3 Offset { get; set; }
        public Vector3 Scale { get; set; }
        public Quaternion Rotation { get; set; }

        public IList<ModelResource> Models { get; set; }

        public override void Load(Stream source)
        {
            Protocol.Resource.ModelGroup entry = Protocol.Resource.ModelGroup.ParseFrom(source);
            this.DoLoad(entry);
        }

        public override long Save(Stream target)
        {
            Protocol.Resource.ModelGroup.Builder builder = this.GetBuilder();
            Protocol.Resource.ModelGroup entry = builder.Build();
            entry.WriteTo(target);
            return entry.SerializedSize;
        }

        public Protocol.Resource.ModelGroup.Builder GetBuilder()
        {
            var builder = new Protocol.Resource.ModelGroup.Builder
                              {
                                  Name = this.Name,
                                  Version = Version
                              };

            builder.AddRangeOffset(this.Offset.ToList());
            builder.AddRangeScale(this.Scale.ToList());
            builder.AddRangeRotation(this.Rotation.ToList());
            
            if (this.Models != null)
            {
                foreach (ModelResource element in this.Models)
                {
                    builder.AddModels(element.GetBuilder());
                }
            }
            
            return builder;
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoLoad(Protocol.Resource.ModelGroup entry)
        {
            if (entry.Version != Version)
            {
                throw new InvalidDataException("Model group version is not correct: " + entry.Version);
            }

            System.Diagnostics.Debug.Assert(entry.OffsetCount == 3, "Offset data has invalid count");
            System.Diagnostics.Debug.Assert(entry.ScaleCount == 3, "Scale data has invalid count");
            System.Diagnostics.Debug.Assert(entry.RotationCount == 4, "Rotation data has invalid count");

            this.Name = entry.Name;
            this.Offset = VectorExtension.Vector3FromList(entry.OffsetList);
            this.Scale = VectorExtension.Vector3FromList(entry.ScaleList);
            this.Rotation = QuaternionExtension.QuaterionFromList(entry.RotationList);
            
            if (entry.ModelsCount > 0)
            {
                this.Models = new List<ModelResource>(entry.ModelsCount);
                foreach (Protocol.Resource.Model element in entry.ModelsList)
                {
                    this.Models.Add(new ModelResource(element));
                }
            }
        }
    }
}
