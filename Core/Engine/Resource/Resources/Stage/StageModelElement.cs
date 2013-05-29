using Core.Utils;

using SlimDX;

namespace Core.Engine.Resource.Resources.Stage
{
    public class StageModelElement : StageElement
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public StageModelElement()
        {
        }

        public StageModelElement(Protocol.Resource.StageModel data)
            : this()
        {
            System.Diagnostics.Debug.Assert(data.TranslationCount == 3, "Translation data has invalid count");
            System.Diagnostics.Debug.Assert(data.RotationCount == 4, "Rotation data has invalid count");
            System.Diagnostics.Debug.Assert(data.ScaleCount == 3, "Scale data has invalid count");

            this.Id = data.Id;

            this.Translation = VectorExtension.Vector3FromList(data.TranslationList);
            this.Rotation = VectorExtension.Vector4FromList(data.RotationList);
            this.Scale = VectorExtension.Vector3FromList(data.ScaleList);

            if (data.HasLayerFlags)
            {
                this.LoadLayerData(data.LayerFlags);
            }

            if (data.PropertiesCount > 0)
            {
                this.LoadProperties(data.PropertiesList);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Vector3 Translation { get; set; }
        public Vector4 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public Protocol.Resource.StageModel.Builder GetBuilder()
        {
            var builder = new Protocol.Resource.StageModel.Builder { Id = this.Id };

            builder.AddRangeTranslation(this.Translation.ToList());
            builder.AddRangeRotation(this.Rotation.ToList());
            builder.AddRangeScale(this.Scale.ToList());

            if (this.LayerFlags != null)
            {
                builder.SetLayerFlags(this.SaveLayerData());
            }

            if (this.Properties != null)
            {
                builder.AddRangeProperties(this.SaveProperties());
            }

            return builder;
        }
    }
}
