using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using Carbon.Editor.Resource.Collada.Effect;
using Carbon.Editor.Resource.Collada.General;
using Carbon.Editor.Resource.Collada.Geometry;
using Carbon.Engine.Resource.Resources;

namespace Carbon.Editor.Resource.Collada
{
    using Carbon.Engine.Resource.Resources.Model;

    public struct ColladaMeshInfo
    {
        public string Name;

        public int Parts;
    }

    public class ColladaInfo
    {
        private readonly List<ColladaMeshInfo> meshInfos;
        private readonly Dictionary<string, ModelMaterialElement> materialInfo;
        private readonly Dictionary<string, string> imageInfo;
        private readonly List<string> normalImages;
        private readonly Dictionary<string, string> colorToNormalImages;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ColladaInfo(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                throw new ArgumentException("Invalid file specified");
            }

            this.meshInfos = new List<ColladaMeshInfo>();
            this.materialInfo = new Dictionary<string, ModelMaterialElement>();
            this.imageInfo = new Dictionary<string, string>();
            this.normalImages = new List<string>();
            this.colorToNormalImages = new Dictionary<string, string>();

            this.Source = file;

            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var model = ColladaModel.Load(stream);
                this.BuildImageLibrary(model.ImageLibrary);
                this.BuildMaterialLibrary(model.MaterialLibrary, model.EffectLibrary);
                this.BuildMeshLibrary(model.GeometryLibrary);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Source { get; private set; }

        public IReadOnlyCollection<ColladaMeshInfo> MeshInfos
        {
            get
            {
                return this.meshInfos.AsReadOnly();
            }
        }

        public IReadOnlyDictionary<string, ModelMaterialElement> MaterialInfos
        {
            get
            {
                return new ReadOnlyDictionary<string, ModelMaterialElement>(this.materialInfo);
            }
        }

        public IReadOnlyDictionary<string, string> ImageInfos
        {
            get
            {
                return new ReadOnlyDictionary<string, string>(this.imageInfo);
            }
        }

        public IReadOnlyCollection<string> NormalImages
        {
            get
            {
                return this.normalImages.AsReadOnly();
            }
        }

        public IReadOnlyDictionary<string, string> ColorToNormalImages
        {
            get
            {
                return new ReadOnlyDictionary<string, string>(this.colorToNormalImages);
            }
        }

        public static string GetUrlValue(string url)
        {
            if (!url.StartsWith("#"))
            {
                return url;
            }

            return url.Substring(1, url.Length - 1);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void BuildMeshLibrary(ColladaGeometryLibrary library)
        {
            this.meshInfos.Clear();

            foreach (ColladaGeometry colladaGeometry in library.Geometries)
            {
                if (colladaGeometry.Mesh.PolyLists == null)
                {
                    // Todo: Check this
                    continue;
                }

                var info = new ColladaMeshInfo
                    {
                        Name = colladaGeometry.Id,
                        Parts = colladaGeometry.Mesh.PolyLists.Length,
                    };

                this.meshInfos.Add(info);
            }
        }

        private void BuildImageLibrary(ColladaImageLibrary images)
        {
            this.imageInfo.Clear();
            if (images == null || images.Images == null || images.Images.Length <= 0)
            {
                return;
            }

            foreach (ColladaImage image in images.Images)
            {
                if (string.IsNullOrEmpty(image.InitFrom.Source))
                {
                    System.Diagnostics.Trace.TraceError("Collada Image InitFrom value was null for " + image.Name);
                    continue;
                }

                this.imageInfo.Add(image.Id, image.InitFrom.Source);
            }
        }

        private string ResolveEffectTexture(ColladaEffect effect, string initFromValue)
        {
            foreach (EffectParameter parameter in effect.ProfileCommon.Parameter)
            {
                if (parameter.Sid.Equals(initFromValue, StringComparison.OrdinalIgnoreCase))
                {
                    string key;
                    if (parameter.Sampler2D != null)
                    {
                        return this.ResolveEffectTexture(effect, parameter.Sampler2D.Source.Content);
                    }

                    if (parameter.Surface != null)
                    {
                        key = parameter.Surface.InitFrom.Source;
                    }
                    else
                    {
                        return initFromValue;
                    }

                    if (this.imageInfo.ContainsKey(key))
                    {
                        return this.imageInfo[key];
                    }

                    break;
                }
            }

            return initFromValue;
        }

        private void BuildMaterialLibrary(ColladaMaterialLibrary materials, ColladaEffectLibrary effectLibrary)
        {
            this.materialInfo.Clear();
            if (materials == null || materials.Materials == null || materials.Materials.Length <= 0
                || effectLibrary == null || effectLibrary.Effects == null || effectLibrary.Effects.Length <= 0)
            {
                return;
            }

            IDictionary<string, string> materialEffectLookup = new Dictionary<string, string>();
            foreach (ColladaMaterial material in materials.Materials)
            {
                materialEffectLookup.Add(GetUrlValue(material.Effect.Url), material.Id);
            }

            foreach (ColladaEffect effect in effectLibrary.Effects)
            {
                EffectTechnique localTechnique = effect.ProfileCommon.Technique;
                string diffuseTexture = null;
                string normalTexture = null;
                string alphaTexture = null;

                if (localTechnique.Phong != null)
                {
                    if (localTechnique.Phong.Diffuse.Texture != null)
                    {
                        diffuseTexture = this.ResolveEffectTexture(effect, localTechnique.Phong.Diffuse.Texture.Texture);
                    }

                    if (localTechnique.Phong.Transparent != null)
                    {
                        alphaTexture = this.ResolveEffectTexture(effect, localTechnique.Phong.Transparent.Texture.Texture);
                    }
                }
                else if (localTechnique.Lambert != null)
                {
                    if (localTechnique.Lambert.Diffuse.Texture != null)
                    {
                        diffuseTexture = this.ResolveEffectTexture(effect, localTechnique.Lambert.Diffuse.Texture.Texture);
                    }
                }
                else if (localTechnique.Blinn != null)
                {
                    if (localTechnique.Blinn.Diffuse.Texture != null)
                    {
                        diffuseTexture = this.ResolveEffectTexture(effect, localTechnique.Blinn.Diffuse.Texture.Texture);
                    }
                }

                if (localTechnique.Extra != null && localTechnique.Extra.Technique != null &&
                        localTechnique.Extra.Technique.Profile == "FCOLLADA")
                {
                    normalTexture = this.ResolveEffectTexture(effect, localTechnique.Extra.Technique.Bump.Texture.Texture);
                    if (normalTexture != null)
                    {
                        if (normalTexture == diffuseTexture)
                        {
                            string normalName = string.Concat(Path.GetFileNameWithoutExtension(normalTexture), "_N", Path.GetExtension(normalTexture));
                            if (!this.colorToNormalImages.ContainsKey(normalName))
                            {
                                this.colorToNormalImages.Add(normalName, normalTexture);
                            }

                            normalTexture = normalName;
                        }
                        else
                        {
                            if (!normalImages.Contains(normalTexture))
                            {
                                this.normalImages.Add(normalTexture);
                            }
                        }
                    }
                }

                // Todo:
                if (diffuseTexture != null)
                {
                    var material = new ModelMaterialElement
                    {
                        Name = materialEffectLookup[effect.Id],
                        DiffuseTexture = diffuseTexture,
                        NormalTexture = normalTexture,
                        AlphaTexture = alphaTexture
                    };
                    materialInfo.Add(material.Name, material);
                }
            }
        }
    }
}
