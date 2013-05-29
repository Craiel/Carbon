using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Engine.Contracts.Rendering;
using Core.Engine.Logic;
using Core.Engine.Rendering.Primitives;
using Core.Utils.Contracts;
using SlimDX;

namespace Core.Engine.Rendering.Debug
{
    public class CoordinateDisplay : EngineComponent
    {
        //private readonly Mesh centerCube;
        //private readonly Mesh axisMarker;

        /*private readonly Material defaultMaterial;
        private readonly Material materialX;
        private readonly Material materialY;
        private readonly Material materialZ;*/

        public CoordinateDisplay()
        {
            /*this.WorldRotation = Quaternion.Identity;

            PositionNormalVertex[] meshData;
            uint[] indexData;
            Cone.Create(10, out meshData, out indexData);
            this.axisMarker = this.CreateMesh();
            this.axisMarker.SetData(meshData, indexData);

            this.centerCube = this.CreateMesh();
            this.centerCube.SetData(Cube.Data, Cube.Indices);

            this.defaultMaterial = this.CreateMaterial();
            this.defaultMaterial.Color = new SlimDX.Vector4(0.3f, 0.3f, 0.3f, 1.0f);

            this.materialX = this.CreateMaterial();
            this.materialX.Color = new SlimDX.Vector4(0.5f, 0, 0, 1.0f);
            this.materialY = this.CreateMaterial();
            this.materialY.Color = new SlimDX.Vector4(0, 0.5f, 0, 1.0f);
            this.materialZ = this.CreateMaterial();
            this.materialZ.Color = new SlimDX.Vector4(0, 0, 0.5f, 1.0f);*/
        }

        public Quaternion WorldRotation { get; set; }
        
        /*public void Render(IRenderer renderer)
        {
            var instruction = new RenderInstruction { Mesh = this.centerCube, Material = this.defaultMaterial, Scale = new SlimDX.Vector3(0.2f) };
            renderer.AddInstruction(instruction);

            var scaling = new SlimDX.Vector3(0.15f);
            float distance = 0.4f;
            instruction = new RenderInstruction { Mesh = this.axisMarker, Material = this.materialX, Scale = scaling, Position = new SlimDX.Vector3(distance, 0, 0), Rotation = Quaternion.RotationAxis(Vector3.UnitZ, 90) };
            renderer.AddInstruction(instruction);
            instruction = new RenderInstruction { Mesh = this.axisMarker, Material = this.defaultMaterial, Scale = scaling, Position = new SlimDX.Vector3(-distance, 0, 0), Rotation = Quaternion.RotationAxis(Vector3.UnitZ, -90) };
            renderer.AddInstruction(instruction);

            instruction = new RenderInstruction { Mesh = this.axisMarker, Material = this.materialY, Scale = scaling, Position = new SlimDX.Vector3(0, distance, 0), Rotation = Quaternion.RotationAxis(Vector3.UnitX, 180) };
            renderer.AddInstruction(instruction);
            instruction = new RenderInstruction { Mesh = this.axisMarker, Material = this.defaultMaterial, Scale = scaling, Position = new SlimDX.Vector3(0, -distance, 0) };
            renderer.AddInstruction(instruction);

            instruction = new RenderInstruction { Mesh = this.axisMarker, Material = this.materialZ, Scale = scaling, Position = new SlimDX.Vector3(0, 0, distance), Rotation = Quaternion.RotationAxis(Vector3.UnitX, -90) };
            renderer.AddInstruction(instruction);
            instruction = new RenderInstruction { Mesh = this.axisMarker, Material = this.defaultMaterial, Scale = scaling, Position = new SlimDX.Vector3(0, 0, -distance), Rotation = Quaternion.RotationAxis(Vector3.UnitX, 90) };
            renderer.AddInstruction(instruction);
        }*/
    }
}
