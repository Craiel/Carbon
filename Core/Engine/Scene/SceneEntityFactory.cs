namespace Core.Engine.Scene
{
    using CarbonCore.Processing.Resource.Stage;
    using CarbonCore.Protocol.Resource;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.UtilsDX;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Rendering;
    using SharpDX;

    public class SceneEntityFactory : EngineComponent, ISceneEntityFactory
    {
        private readonly IFactory factory;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneEntityFactory(IFactory factory)
        {
            this.factory = factory;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ICameraEntity BuildCamera(StageCameraElement cameraElement)
        {
            var camera = this.factory.Resolve<IProjectionCamera>();
            camera.Position = cameraElement.Position;
            camera.Rotation = cameraElement.Rotation;

            // Todo: near / far plane and fov
            return new CameraEntity { Camera = camera };
        }

        public ILightEntity BuildLight(StageLightElement lightElement)
        {
            var light = new Light();
            if (lightElement.Location != null)
            {
                light.Position = (Vector3)lightElement.Location;
            }

            light.Range = lightElement.Radius;
            if (lightElement.Direction != null)
            { 
                light.Direction = (Vector3)lightElement.Direction;
            }

            if (lightElement.Color != null)
            {
                light.Color = new Vector4((Vector3)lightElement.Color, 1);
            }

            switch (lightElement.Type)
            {
                case StageLight.Types.StageLightType.Directional:
                    {
                        light.Type = LightType.Direction;
                        break;
                    }

                case StageLight.Types.StageLightType.Point:
                    {
                        light.Type = LightType.Point;
                        break;
                    }

                case StageLight.Types.StageLightType.Spot:
                    {
                        light.Type = LightType.Spot;
                        break;
                    }

                default:
                    {
                        System.Diagnostics.Trace.TraceError("Unknown Light type: {0}", lightElement.Type);
                        break;
                    }
            }

            // Todo:
            // lightElement.SpotSize
            // lightElement.Intensity;
            // lightElement.AmbientIntensity
            var entity = new LightEntity { Light = light, Name = lightElement.Id };
            if (lightElement.Location != null)
            {
                entity.Position = lightElement.Location.Value;
            }

            if (lightElement.Direction != null)
            {
                entity.Rotation = QuaternionExtension.RotationYawPitchRoll(lightElement.Direction.Value);
            }

            return entity;
        }
    }
}
