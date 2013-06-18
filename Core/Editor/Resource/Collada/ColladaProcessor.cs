﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Core.Engine.Resource;
using Core.Engine.Resource.Resources.Model;

using Core.Utils;

using SlimDX;

namespace Core.Editor.Resource.Collada
{
    using Core.Editor.Resource.Collada.Data;
    using Core.Editor.Resource.Collada.Geometry;
    using Core.Editor.Resource.Collada.Scene;
    using Core.Editor.Resource.Generic.Data;

    /// <summary>
    /// Todo:
    /// - Clean up the general process
    /// - First we get all the Model groups out of the collada since that is really all we care for here
    /// - then either group them all together if nothing specific was requested, otherwise return the one that was asked for
    /// - !! Get rid of the offset and scaling entirely here, we will do that at the actual scene definition !!
    /// </summary>
    public static class ColladaProcessor
    {
        private static readonly IDictionary<string, ModelResourceGroup> MeshLibrary = new Dictionary<string, ModelResourceGroup>();

        private static string targetElement;
        private static string texturePath;

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
        
        public static ModelResourceGroup Process(ColladaInfo info, string element, string texPath)
        {
            ClearCache();

            targetElement = element;
            texturePath = texPath;

            using (var stream = new FileStream(info.Source, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var model = ColladaModel.Load(stream);

                BuildGeometryLibrary(info, model.GeometryLibrary);

                foreach (ColladaSceneNode sceneNode in model.SceneLibrary.VisualScene.Nodes)
                {
                    ApplyNodeTranslations(sceneNode);
                }

                if (string.IsNullOrEmpty(element))
                {
                    Debug.Assert(MeshLibrary.Count == 1, "Mesh library was expected to have only single element");
                    return MeshLibrary.First().Value;
                }
                    
                return MeshLibrary[element];
            }
        }

        private static void ClearCache()
        {
            MeshLibrary.Clear();

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

        private static void BuildGeometryLibrary(ColladaInfo info, ColladaGeometryLibrary library)
        {
            foreach (ColladaGeometry colladaGeometry in library.Geometries)
            {
                ClearCache();

                if (colladaGeometry.Mesh.PolyLists == null)
                {
                    // Todo: Check this
                    continue;
                }

                if (!string.IsNullOrEmpty(targetElement) && !colladaGeometry.Id.Equals(targetElement))
                {
                    continue;
                }

                polygonData = new IDictionary<uint, uint[]>[colladaGeometry.Mesh.PolyLists.Length];
                IList<ModelResource> parts = new List<ModelResource>();
                for (int i = 0; i < colladaGeometry.Mesh.PolyLists.Length; i++)
                {
                    ColladaPolyList polyList = colladaGeometry.Mesh.PolyLists[i];

                    ParseGeometry(i, colladaGeometry);
                    ModelResource part = TranslateGeometry(i, colladaGeometry.Name);
                    if (polyList.Material != null && info.MaterialInfos.ContainsKey(polyList.Material))
                    {
                        ModelMaterialElement material = info.MaterialInfos[polyList.Material].Clone();
                        if (material.DiffuseTexture != null && texturePath != null)
                        {
                            material.DiffuseTexture = HashUtils.BuildResourceHash(Path.Combine(texturePath, Uri.UnescapeDataString(material.DiffuseTexture)));
                        }

                        if (material.NormalTexture != null && texturePath != null)
                        {
                            material.NormalTexture = HashUtils.BuildResourceHash(Path.Combine(texturePath, Uri.UnescapeDataString(material.NormalTexture)));
                        }

                        if (material.SpecularTexture != null && texturePath != null)
                        {
                            material.SpecularTexture = HashUtils.BuildResourceHash(Path.Combine(texturePath, Uri.UnescapeDataString(material.SpecularTexture)));   
                        }

                        if (material.AlphaTexture != null && texturePath != null)
                        {
                            material.AlphaTexture = HashUtils.BuildResourceHash(Path.Combine(texturePath, Uri.UnescapeDataString(material.AlphaTexture)));    
                        }

                        if (part.Materials == null)
                        {
                            part.Materials = new List<ModelMaterialElement>();
                        }

                        part.Materials.Add(material);
                    }

                    parts.Add(part);
                }

                MeshLibrary.Add(colladaGeometry.Id, new ModelResourceGroup { Models = parts, Name = colladaGeometry.Id });
            }
        }

        private static void ParseGeometry(int index, ColladaGeometry geometry)
        {
            if (geometry.Mesh == null || geometry.Mesh.Vertices == null)
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
            positionData = DataConversion.ToVector3(positionSource.FloatArray.Data);

            // Now load the Normals and UV's
            if (normalInput != null)
            {
                ColladaSource source = FindSource(normalInput.Source);
                normalData = DataConversion.ToVector3(source.FloatArray.Data);
            }

            if (textureInput != null)
            {
                ColladaSource source = FindSource(textureInput.Source);
                textureData = DataConversion.ToVector2(source.FloatArray.Data);
            }

            // Enable only if needed, causes massive slowness for larger meshes
            // TraceConversionInfo(geometry);
        }

        private static void TraceConversionInfo(ColladaGeometry geometry)
        {
            Trace.TraceInformation("Converting Collada Geometry {0}", geometry.Name);
            Trace.TraceInformation("  -> {0} Polygons", vertexCount.Length);
            Trace.TraceInformation("  -> {0} IndexData", indexData.Length);
            Trace.TraceInformation("  -> {0} Vertices", positionData.Length);
            if (normalData != null)
            {
                Trace.TraceInformation("  -> {0} Normals", normalData.Length);
            }

            if (textureData != null)
            {
                Trace.TraceInformation("  -> {0} UV's", textureData.Length);
            }
        }

        private static ModelResource TranslateGeometry(int polyIndex, string name)
        {
            var builder = new ModelBuilder(name);

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

            return builder.ToResource();
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

        private static void ApplyNodeTranslations(ColladaSceneNode sceneNode)
        {
            if (sceneNode.InstanceGeometry == null)
            {
                return;
            }

            string targetNode = ColladaInfo.GetUrlValue(sceneNode.InstanceGeometry.Url);
            if (!MeshLibrary.ContainsKey(targetNode))
            {
                return;
            }

            if (sceneNode.Translation != null)
            {
                MeshLibrary[targetNode].Offset = DataConversion.ToVector3(sceneNode.Translation.Data)[0];
            }

            MeshLibrary[targetNode].Scale = sceneNode.Scale != null ? DataConversion.ToVector3(sceneNode.Scale.Data)[0] : new Vector3(1);

            if (sceneNode.Rotations != null)
            {
                MeshLibrary[targetNode].Rotation = GetNodeRotation(sceneNode);
            }
        }

        private static Quaternion GetNodeRotation(ColladaSceneNode node)
        {
            float rotationX = 0;
            float rotationY = 0;
            float rotationZ = 0;

            foreach (ColladaRotate rotation in node.Rotations)
            {
                if (rotation.Sid.EndsWith("X"))
                {
                    rotationX = MathExtension.DegreesToRadians(DataConversion.ToVector4(rotation.Data)[0][3]);
                }
                else if (rotation.Sid.EndsWith("Y"))
                {
                    rotationY = MathExtension.DegreesToRadians(DataConversion.ToVector4(rotation.Data)[0][3]);
                }
                else if (rotation.Sid.EndsWith("Z"))
                {
                    rotationZ = MathExtension.DegreesToRadians(DataConversion.ToVector4(rotation.Data)[0][3]);
                }
            }

            return Quaternion.RotationYawPitchRoll(rotationX, rotationY, rotationZ);
        }
    }
}