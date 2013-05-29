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
    public class OrientationDisplay : EngineComponent
    {
        //private readonly Mesh axisCore;
        //private readonly Mesh axisMarker;

        /*private readonly Material materialX;
        private readonly Material materialY;
        private readonly Material materialZ;*/

        public OrientationDisplay()
        {
            /*PositionNormal[] meshData;
            uint[] indexData;
            Cone.Create(10, out meshData, out indexData);
            this.axisMarker = this.CreateMesh();
            this.axisMarker.SetData(meshData, indexData);

            this.axisCore = this.CreateMesh();
            this.axisCore.SetData(Cube.Data, Cube.Indices);

            this.materialX = this.CreateMaterial();
            this.materialX.Color = new SlimDX.Vector4(0.5f, 0, 0, 1.0f);
            this.materialY = this.CreateMaterial();
            this.materialY.Color = new SlimDX.Vector4(0, 0.5f, 0, 1.0f);
            this.materialZ = this.CreateMaterial();
            this.materialZ.Color = new SlimDX.Vector4(0, 0, 0.5f, 1.0f);*/
        }
        
        /*public void Render(IRenderer renderer, Vector3 position, Quaternion rotation)
        {
            float pointerSize = 0.2f;
            float pointerLength = 0.3f;
            float coreSize = 0.04f;
            float coreLength = 1.0f;

            Vector3 corePos = position + new Vector3(coreLength, 0, 0);
            var instruction = new RenderInstruction { Mesh = this.axisCore, Material = this.materialX, Scale = new SlimDX.Vector3(coreLength, coreSize, coreSize), Position = corePos, Rotation = rotation };
            renderer.AddInstruction(instruction);
            instruction = new RenderInstruction { Mesh = this.axisMarker, Material = this.materialX, Scale = new Vector3(pointerSize, pointerLength, pointerSize), Position = corePos + new Vector3(coreLength, 0, 0), Rotation = Quaternion.RotationAxis(Vector3.UnitZ, -90) * rotation };
            renderer.AddInstruction(instruction);

            corePos = position + new Vector3(0, coreLength, 0);
            instruction = new RenderInstruction { Mesh = this.axisCore, Material = this.materialY, Scale = new SlimDX.Vector3(coreSize, coreLength, coreSize), Position = corePos, Rotation = rotation };
            renderer.AddInstruction(instruction);
            instruction = new RenderInstruction { Mesh = this.axisMarker, Material = this.materialY, Scale = new Vector3(pointerSize, pointerLength, pointerSize), Position = corePos + new Vector3(0, coreLength, 0), Rotation = rotation };
            renderer.AddInstruction(instruction);

            corePos = position + new Vector3(0, 0, coreLength);
            instruction = new RenderInstruction { Mesh = this.axisCore, Material = this.materialZ, Scale = new SlimDX.Vector3(coreSize, coreSize, coreLength), Position = corePos, Rotation = rotation };
            renderer.AddInstruction(instruction);
            instruction = new RenderInstruction { Mesh = this.axisMarker, Material = this.materialZ, Scale = new Vector3(pointerSize, pointerLength, pointerSize), Position = corePos + new Vector3(0, 0, coreLength), Rotation = Quaternion.RotationAxis(Vector3.UnitX, 90) * rotation };
            renderer.AddInstruction(instruction);
        }*/
    }
}
