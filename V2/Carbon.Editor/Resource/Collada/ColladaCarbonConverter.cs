﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Carbon.Editor.Resource.Collada.Data;
using Carbon.Editor.Resource.Collada.Effect;
using Carbon.Editor.Resource.Collada.Geometry;
using Carbon.Editor.Resource.Collada.Scene;
using Carbon.Engine.Resource;

using Carbon.Engine.Rendering;

using Core.Utils;

using SlimDX;

namespace Carbon.Editor.Resource.Collada
{
    public static class ColladaCarbonConverter
    {
        private readonly static IDictionary<string, int> materialLookup = new Dictionary<string, int>();
        private readonly static IList<MaterialResource> materialLibrary = new List<MaterialResource>();

        private readonly static IDictionary<string, int> meshLookup = new Dictionary<string, int>();
        private readonly static IList<MeshResource> meshLibrary = new List<MeshResource>();

        private readonly static IDictionary<string, string> meshMaterialLibrary = new Dictionary<string, string>();

        private static ColladaInput[] currentInputs;
        private static ColladaSource[] currentSources;
        private static IDictionary<uint, uint[]>[] polygonData;

        private static int[] vertexCount;
        private static int[] indexData;

        private static ColladaInput vertexInput;
        private static ColladaInput normalInput;
        private static ColladaInput textureInput;

        private static Vector3[] positionData;
        private static Vector3[] normalData;
        private static Vector2[] textureData;

        private static string sourceFolder;
        
        public static void Convert(string sourceFile, string targetFolder)
        {
            sourceFolder = Path.GetDirectoryName(sourceFile);

            /*var model = ColladaModel.Load(sourceFile);
            BuildMaterialLibrary(model.MaterialLibrary, model.EffectLibrary);
            BuildGeometryLibrary(model.GeometryLibrary);

            string name = Path.GetFileNameWithoutExtension(sourceFile);
            SceneResource root = new SceneResource { Name = name };
            int index = 0;
            foreach (MeshResource mesh in meshLibrary)
            {
                root.Meshes.Add(index++, mesh.Name);
                meshLookup.Add(mesh.Name, index);
            }

            index = 0;
            foreach (MaterialResource material in materialLibrary)
            {
                root.Materials.Add(index++, material.Name);
                materialLookup.Add(material.Name, index);
            }

            IEnumerable<SceneResourceNode> contents = BuildScene(model.SceneLibrary);
            foreach (SceneResourceNode content in contents)
            {
                root.Nodes.Add(content);
            }*/
        }

        private static IEnumerable<SceneResourceNode> BuildScene(ColladaSceneLibrary library)
        {
            IList<SceneResourceNode> sceneModels = new List<SceneResourceNode>();
            foreach (ColladaSceneNode sceneNode in library.VisualScene.Nodes)
            {
                IEnumerable<SceneResourceNode> models = BuildSceneNode(sceneNode);
                foreach (SceneResourceNode nodeModel in models)
                {
                    sceneModels.Add(nodeModel);
                }
            }

            return sceneModels;
        }

        private static IEnumerable<SceneResourceNode> BuildSceneNode(ColladaSceneNode sceneNode)
        {
            if (sceneNode.InstanceGeometry == null)
            {
                return null;
            }

            IList<SceneResourceNode> models = new List<SceneResourceNode>();

            string targetNode = GetUrlValue(sceneNode.InstanceGeometry.Url);
            if (meshLookup.ContainsKey(targetNode))
            {
                float rotationX = 0;
                float rotationY = 0;
                float rotationZ = 0;

                foreach (ColladaRotate rotation in sceneNode.Rotations)
                {
                    if (rotation.Sid.EndsWith("X"))
                    {
                        rotationX = MathExtension.DegreesToRadians(rotation.ToVector4()[0][3]);
                    }
                    else if (rotation.Sid.EndsWith("Y"))
                    {
                        rotationY = MathExtension.DegreesToRadians(rotation.ToVector4()[0][3]);
                    }
                    else if (rotation.Sid.EndsWith("Z"))
                    {
                        rotationZ = MathExtension.DegreesToRadians(rotation.ToVector4()[0][3]);
                    }
                }

                var node = new SceneResourceNode
                                      {
                                          MeshId = meshLookup[targetNode],
                                          Position = sceneNode.Translation.ToVector3()[0],
                                          Rotation = new Vector3(rotationX, rotationY, rotationZ),
                                          Scale = sceneNode.Scale.ToVector3()[0],
                                      };

                if (materialLookup.ContainsKey(targetNode))
                {
                    node.MaterialId = materialLookup[targetNode];
                }

                if (sceneNode.Children != null && sceneNode.Children.Length > 0)
                {
                    foreach (ColladaSceneNode child in sceneNode.Children)
                    {
                        IEnumerable<SceneResourceNode> nodeChildren = BuildSceneNode(child);
                        if (nodeChildren == null)
                        {
                            continue;
                        }

                        foreach (SceneResourceNode nodeChild in nodeChildren)
                        {
                            node.Children.Add(nodeChild);
                        }
                    }
                }

                models.Add(node);
            }

            return models;
        }

        private static void ClearCache()
        {
            currentInputs = null;
            currentSources = null;
            polygonData = null;

            vertexCount = null;
            indexData = null;

            vertexInput = null;
            normalInput = null;
            textureInput = null;

            positionData = null;
            normalData = null;
            textureData = null;
        }

        private static string GetUrlValue(string url)
        {
            return url.Substring(1, url.Length - 1);
        }

        private static void BuildMaterialLibrary(ColladaMaterialLibrary materials, ColladaEffectLibrary effectLibrary)
        {
            materialLookup.Clear();
            materialLibrary.Clear();

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
                        diffuseTexture = GetLibraryTextureName(localTechnique.Phong.Diffuse.Texture.Texture);
                    }

                    if (localTechnique.Phong.Transparent != null)
                    {
                        alphaTexture = GetLibraryTextureName(localTechnique.Phong.Transparent.Texture.Texture);
                    }
                }
                else if (localTechnique.Lambert != null)
                {
                    if (localTechnique.Lambert.Diffuse.Texture != null)
                    {
                        diffuseTexture = GetLibraryTextureName(localTechnique.Lambert.Diffuse.Texture.Texture);
                    }
                }
                else if (localTechnique.Blinn != null)
                {
                    if (localTechnique.Blinn.Diffuse.Texture != null)
                    {
                        diffuseTexture = GetLibraryTextureName(localTechnique.Blinn.Diffuse.Texture.Texture);
                    }
                }

                if (localTechnique.Extra != null && localTechnique.Extra.Technique != null &&
                        localTechnique.Extra.Technique.Profile == "FCOLLADA")
                {
                    normalTexture = GetLibraryTextureName(localTechnique.Extra.Technique.Bump.Texture.Texture);
                    if (normalTexture == diffuseTexture)
                    {
                        normalTexture = null;
                    }
                }

                // Todo:
                if (diffuseTexture != null)
                {
                    var material = new MaterialResource
                                                  {
                                                      Name = materialEffectLookup[effect.Id],
                                                      DiffuseTexture = diffuseTexture,
                                                      NormalTexture = normalTexture,
                                                      AlphaTexture = alphaTexture
                                                  };
                    materialLibrary.Add(material);
                }
            }
        }

        private static void BuildGeometryLibrary(ColladaGeometryLibrary library)
        {
            meshLookup.Clear();
            meshLibrary.Clear();

            foreach (ColladaGeometry colladaGeometry in library.Geometries)
            {
                ClearCache();

                if (colladaGeometry.Mesh.PolyLists == null)
                {
                    // Todo: Check this
                    continue;
                }

                polygonData = new IDictionary<uint, uint[]>[colladaGeometry.Mesh.PolyLists.Length];
                IList<MeshResource> parts = new List<MeshResource>();
                for (int i = 0; i < colladaGeometry.Mesh.PolyLists.Length; i++)
                {
                    string id = string.Format("{0}_{1}", colladaGeometry.Id, i);
                    ColladaPolyList polyList = colladaGeometry.Mesh.PolyLists[i];

                    ParseGeometry(i, colladaGeometry);
                    parts.Add(TranslateGeometry(i, colladaGeometry.Name));

                    if (polyList.Material != null)
                    {
                        meshMaterialLibrary.Add(id, polyList.Material);
                    }
                }

                meshLibrary.Add(new MeshResource(parts) { Name = colladaGeometry.Id });
            }
        }

        private static string GetLibraryTextureName(string textureReference)
        {
            int index = textureReference.IndexOf("_tga");
            if (index < 0)
            {
                index = textureReference.IndexOf("_png");
            }
            if (index < 0)
            {
                index = textureReference.IndexOf("_jpg");
            }
            if (index < 0)
            {
                index = textureReference.IndexOf("-sampler");
            }

            return string.Format(@"Textures\{0}_textures\{1}.dds", textureReference.Substring(0, index));
        }

        private static void ParseGeometry(int index, ColladaGeometry geometry)
        {
            if(geometry.Mesh == null || geometry.Mesh.Vertices == null)
            {
                throw new InvalidOperationException("ConvertGeometry failed, no Mesh or Vertex data found for " + geometry.Name);
            }

            if (geometry.Mesh.PolyLists == null && geometry.Mesh.PolyLists.Length > index)
            {
                throw new NotImplementedException("Currently we do not support models without poly lists");
            }

            currentSources = geometry.Mesh.Sources;
            
            // Get the Layout information
            currentInputs = geometry.Mesh.PolyLists[index].Inputs;
            vertexInput = FindInput("VERTEX");
            normalInput = FindInput("NORMAL");
            textureInput = FindInput("TEXCOORD");
            if (!geometry.Mesh.Vertices.Id.Equals(vertexInput.Source.TrimStart('#')))
            {
                throw new InvalidDataException("Vertex source does not match position source!");
            }

            vertexCount = geometry.Mesh.PolyLists[index].VertexCount.Data;
            indexData = geometry.Mesh.PolyLists[index].P.Data;
            LoadPolygonData(index, indexData);

            // Process the Vertex Data
            ColladaSource positionSource = FindSource(geometry.Mesh.Vertices.Input.Source);
            positionData = positionSource.FloatArray.ToVector3();
            
            // Now load the Normals and UV's
            if (normalInput != null)
            {
                ColladaSource source = FindSource(normalInput.Source);
                normalData = source.FloatArray.ToVector3();
            }

            if (textureInput != null)
            {
                ColladaSource source = FindSource(textureInput.Source);
                textureData = source.FloatArray.ToVector2();
            }

            TraceConversionInfo(geometry);
        }

        private static void TraceConversionInfo(ColladaGeometry geometry)
        {
            Trace.TraceInformation("Converting Collada Geometry {0}", geometry.Name);
            Trace.TraceInformation("  -> {0} Polygons", vertexCount.Length);
            Trace.TraceInformation("  -> {0} IndexData", indexData.Length);
            Trace.TraceInformation("  -> {0} Vertices", positionData.Length);
            if(normalData != null)
            {
                Trace.TraceInformation("  -> {0} Normals", normalData.Length);
            }

            if(textureData != null)
            {
                Trace.TraceInformation("  -> {0} UV's", textureData.Length);
            }
        }

        private static MeshResource TranslateGeometry(int polyIndex, string name)
        {
            var builder = new MeshBuilder(name);
            
            // All data is set now, Build the polygons into our mesh
            int indexPosition = 0;
            for (int index = 0; index < vertexCount.Length; index++)
            {
                int count = vertexCount[index];
                builder.BeginPolygon();
                for (int i = 0; i < count; i++)
                {
                    Vector3 position = positionData[polygonData[polyIndex][(uint)vertexInput.Offset][indexPosition]];
                    Vector3 normal = Vector3.Zero;
                    Vector2 texture = Vector2.Zero;

                    if (normalData != null)
                    {
                        normal = normalData[polygonData[polyIndex][(uint)normalInput.Offset][indexPosition]];
                    }

                    if (textureData != null)
                    {
                        texture = textureData[polygonData[polyIndex][(uint)textureInput.Offset][indexPosition]];
                    }

                    builder.AddVertex(position, normal, texture);
                    indexPosition++;
                }

                builder.EndPolygon();
            }

            return builder.ToMesh();
        }
        
        private static ColladaInput FindInput(string semantic)
        {
            return currentInputs.FirstOrDefault(colladaInput => colladaInput.Semantic.Equals(semantic));
        }

        private static ColladaSource FindSource(string id)
        {
            id = id.TrimStart('#');
            return currentSources.FirstOrDefault(source => source.Id.Equals(id));
        }

        private static void LoadPolygonData(int index, int[] data)
        {
            polygonData[index] = new Dictionary<uint, uint[]>();
            uint highestOffset = 0;
            foreach (ColladaInput input in currentInputs)
            {
                if (input.Offset > highestOffset)
                {
                    highestOffset = (uint)input.Offset;
                }
            }

            foreach (ColladaInput input in currentInputs)
            {
                if (polygonData[index].ContainsKey((uint)input.Offset))
                {
                    // Todo: This currently happens for normal map texture mapping, the same offset is defined twice with different material source
                    //throw new InvalidDataException("Multiple inputs defined with the same offset");
                    continue;
                }

                polygonData[index].Add((uint)input.Offset, new uint[data.Length / (highestOffset + 1)]);
            }

            uint offset = 0;
            uint element = 0;
            while (offset < data.Length)
            {
                uint currentOffset = 0;
                while (currentOffset <= highestOffset)
                {
                    polygonData[index][currentOffset][element] = (uint)data[offset + currentOffset];
                    currentOffset++;
                }

                offset += highestOffset + 1;
                element++;
            }
        }
    }
}
