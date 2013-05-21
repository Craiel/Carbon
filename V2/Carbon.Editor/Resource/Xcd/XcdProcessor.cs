namespace Carbon.Editor.Resource.Xcd
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Carbon.Editor.Resource.Generic.Data;
    using Carbon.Editor.Resource.Xcd.Scene;
    using Carbon.Engine.Resource.Resources.Stage;

    using SlimDX;

    public struct XcdProcessingOptions
    {
    }

    public static class XcdProcessor
    {
        private static readonly IList<StageCameraElement> CameraElements;
        private static readonly IList<StageLightElement> LightElements;
        private static readonly IList<StageModelElement> ModelElements;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static XcdProcessor()
        {
            CameraElements = new List<StageCameraElement>();
            LightElements = new List<StageLightElement>();
            ModelElements = new List<StageModelElement>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static StageResource Process(string path, XcdProcessingOptions options)
        {
            ClearCache();

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var stage = Xcd.Load(stream);
                StageResource resource = new StageResource();
                if (stage.Scene == null)
                {
                    throw new InvalidDataException("XCD File contains no scene information!");
                }

                TranslateScene(stage.Scene);
                if (CameraElements.Count > 0)
                {
                    resource.Cameras = CameraElements.ToArray();
                }

                if (LightElements.Count > 0)
                {
                    resource.Lights = LightElements.ToArray();
                }

                if (ModelElements.Count > 0)
                {
                    resource.Models = ModelElements.ToArray();
                }

                return resource;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void ClearCache()
        {
            CameraElements.Clear();
            LightElements.Clear();
            ModelElements.Clear();
        }

        private static void TranslateScene(XcdScene scene)
        {
            if (scene.Cameras != null)
            {
                foreach (XcdCamera camera in scene.Cameras)
                {
                    TranslateCamera(camera);
                }
            }

            if (scene.Lights != null)
            {
                foreach (XcdLight light in scene.Lights)
                {
                    TranslateLight(light);
                }
            }

            if (scene.Meshes != null)
            {
                foreach (XcdMesh mesh in scene.Meshes)
                {
                    TranslateModel(mesh);
                }
            }
        }

        private static void TranslateCamera(XcdCamera camera)
        {
            Vector3 position = DataConversion.ToVector3(camera.Position.Data)[0];
            Vector4 orientation = DataConversion.ToVector4(camera.Orientation.Data)[0];
            var element = new StageCameraElement
                       {
                           Id = camera.Id,
                           FieldOfView = camera.FieldOfView,
                           Orientation = orientation,
                           Position = position,
                           Properties = TranslateProperties(camera.CustomProperties)
                       };

            if (camera.LayerInfo != null)
            {
                element.LayerFlags = new List<bool>(camera.LayerInfo.Data);
            }

            CameraElements.Add(element);
        }

        private static void TranslateLight(XcdLight light)
        {
            Vector3? direction = null;
            Vector3? location = null;
            Vector3? color = null;

            if (light.Direction != null)
            {
                direction = DataConversion.ToVector3(light.Direction.Data)[0];
            }

            if (light.Location != null)
            {
                location = DataConversion.ToVector3(light.Location.Data)[0];
            }

            if (light.Color != null)
            {
                color = DataConversion.ToVector3(light.Color.Data)[0];
            }

            var type = (Protocol.Resource.StageLight.Types.StageLightType)Enum.Parse(typeof(Protocol.Resource.StageLight.Types.StageLightType), light.Type);
            var element = new StageLightElement
            {
                Id = light.Id,
                Type = type,
                Direction = direction,
                Location = location,
                Color = color,
                Intensity = light.Intensity,
                AmbientIntensity = light.AmbientIntensity,
                SpotSize = light.SpotSize,
                Angle = light.Angle,
                Radius = light.Radius,
                Properties = TranslateProperties(light.CustomProperties)
            };

            if (light.LayerInfo != null)
            {
                element.LayerFlags = new List<bool>(light.LayerInfo.Data);
            }

            LightElements.Add(element);
        }

        private static void TranslateModel(XcdMesh mesh)
        {
            Vector3 translation = DataConversion.ToVector3(mesh.Translation.Data)[0];
            Vector4 rotation = DataConversion.ToVector4(mesh.Rotation.Data)[0];
            Vector3 scale = DataConversion.ToVector3(mesh.Scale.Data)[0];
            var element = new StageModelElement
            {
                Id = mesh.Id,
                Translation = translation,
                Rotation = rotation,
                Scale = scale,
                Properties = TranslateProperties(mesh.CustomProperties)
            };

            if (mesh.LayerInfo != null)
            {
                element.LayerFlags = new List<bool>(mesh.LayerInfo.Data);
            }

            ModelElements.Add(element);
        }

        private static StagePropertyElement[] TranslateProperties(XcdCustomProperties customProperties)
        {
            if (customProperties == null || customProperties.Properties == null || customProperties.Properties.Length <= 0)
            {
                return null;
            }

            var elements = new StagePropertyElement[customProperties.Properties.Length];
            for (int i = 0; i < customProperties.Properties.Length; i++)
            {
                var type = (Protocol.Resource.StageProperty.Types.StagePropertyType)Enum.Parse(typeof(Protocol.Resource.StageProperty.Types.StagePropertyType), customProperties.Properties[i].Type);
                switch (type)
                {
                    case Protocol.Resource.StageProperty.Types.StagePropertyType.String:
                        {
                            elements[i] = new StagePropertyElementString
                                              {
                                                  Id = customProperties.Properties[i].Id,
                                                  Value = customProperties.Properties[i].Value
                                              };
                            break;
                        }

                    case Protocol.Resource.StageProperty.Types.StagePropertyType.Float:
                        {
                            elements[i] = new StagePropertyElementFloat
                                              {
                                                  Id = customProperties.Properties[i].Id,
                                                  Value = float.Parse(customProperties.Properties[i].Value)
                                              };
                            break;
                        }

                    case Protocol.Resource.StageProperty.Types.StagePropertyType.Int:
                        {
                            elements[i] = new StagePropertyElementInt
                                              {
                                                  Id = customProperties.Properties[i].Id,
                                                  Value = int.Parse(customProperties.Properties[i].Value)
                                              };
                            break;
                        }

                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }

            return elements;
        }
    }
}
