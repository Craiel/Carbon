using System;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic;
using Carbon.Engine.Rendering;
using Carbon.Engine.Resource.Resources;

using SlimDX;

namespace Carbon.Engine.Scene
{
    public interface IScene : IEngineComponent
    {
        void Render();
        void Resize(int width, int height);
    }

    public abstract class Scene : EngineComponent, IScene
    {        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract void Render();

        public abstract void Resize(int width, int height);

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected ModelNode CreateNode(ICarbonGraphics graphics, ModelResource resource)
        {
            var node = new ModelNode
            {
                Mesh = new Mesh(resource),
                Position = new Vector4(resource.Offset, 1.0f),
                Scale = resource.Scale,
                Rotation = resource.Rotation
            };

            if (resource.Materials != null && resource.Materials.Count > 0)
            {
                if (resource.Materials.Count > 1)
                {
                    throw new NotImplementedException();
                }

                node.Material = new Material(graphics, resource.Materials[0]);
            }

            if (resource.SubParts != null && resource.SubParts.Count > 0)
            {
                foreach (ModelResource part in resource.SubParts)
                {
                    ModelNode child = this.CreateNode(graphics, part);
                    if (child != null)
                    {
                        node.AddChild(child);
                    }
                }
            }

            return node;
        }
    }
}
