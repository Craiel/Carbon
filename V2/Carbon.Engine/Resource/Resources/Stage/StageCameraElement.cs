using Core.Utils;

using SlimDX;

namespace Carbon.Engine.Resource.Resources.Stage
{
    public class StageCameraElement : StageElement
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public StageCameraElement()
        {
        }

        public StageCameraElement(Protocol.Resource.StageCamera data)
            : this()
        {
            System.Diagnostics.Debug.Assert(data.PositionCount == 3, "Position data has invalid count");
            System.Diagnostics.Debug.Assert(data.OrientationCount == 4, "Orientation data has invalid count");

            this.Id = data.Id;

            this.Position = VectorExtension.Vector3FromList(data.PositionList);
            this.Orientation = VectorExtension.Vector4FromList(data.OrientationList);

            this.LoadLayerData(data.LayerFlags);
            this.LoadProperties(data.PropertiesList);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Vector3 Position { get; set; }
        public Vector4 Orientation { get; set; }

        public float FieldOfView { get; set; }

        public Protocol.Resource.StageCamera.Builder GetBuilder()
        {
            var builder = new Protocol.Resource.StageCamera.Builder { Id = this.Id };

            builder.AddRangePosition(this.Position.ToList());
            builder.AddRangeOrientation(this.Orientation.ToList());

            builder.SetLayerFlags(this.SaveLayerData());
            builder.AddRangeProperties(this.SaveProperties());
            return builder;
        }
    }
}
