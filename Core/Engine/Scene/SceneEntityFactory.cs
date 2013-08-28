﻿namespace Core.Engine.Scene
{
    using System;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Rendering;
    using Core.Engine.Resource.Resources.Stage;
    using Core.Protocol.Resource;

    using SlimDX;

    public class SceneEntityFactory : EngineComponent, ISceneEntityFactory
    {
        private readonly IEngineFactory factory;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneEntityFactory(IEngineFactory factory)
        {
            this.factory = factory;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ICamera BuildCamera(StageCameraElement cameraElement)
        {
            var camera = this.factory.Get<IProjectionCamera>();
            camera.Position = new Vector4(cameraElement.Position, 1);
            camera.Rotation = Quaternion.RotationYawPitchRoll(cameraElement.Orientation.X, cameraElement.Orientation.Y, cameraElement.Orientation.Z);

            // Todo: near / far plane and fov
            return camera;
        }

        public ILight BuildLight(StageLightElement lightElement)
        {
            var light = new Light();
            if (lightElement.Location != null)
            {
                light.Position = new Vector4((Vector3)lightElement.Location, 1);
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
            return light;
        }

        public IModelEntity BuildModel(StageModelElement modelElement)
        {
            var model = new ModelEntity
                            {
                                Position = new Vector4(modelElement.Translation, 1),
                                Scale = modelElement.Scale,
                                Rotation =
                                    Quaternion.RotationYawPitchRoll(
                                        modelElement.Rotation.X,
                                        modelElement.Rotation.Y,
                                        modelElement.Rotation.Z)
                            };
            return model;
        }
    }
}
