using System;
using System.Collections.Generic;

using Core.Engine.Resource;

using Core.Utils;

using SlimDX;

namespace Core.Engine.Rendering.Primitives
{
    using Core.Engine.Resource.Resources.Model;

    public static class Cone
    {
        private static uint creationCount;

        public static ModelResource Create(int segments, float height = 2.0f, float radius = 1.0f)
        {
            float deltaAngle = 2.0f * (float)(Math.PI / segments);
            float segmentLength = 1.0f / segments;
            float normalY = (90.0f - MathExtension.RadiansToDegrees((float)Math.Atan(height / radius))) / 90.0f;

            var builder = new ModelBuilder("Cone " + ++creationCount) { IsIndexed = true };

            // Top Vertex
            builder.AddVertex(new Vector3(0, height / 2.0f, 0), new Vector3(0, normalY, 0), new Vector2(1.0f - segmentLength, 0));
            
            // Bottom Triangle fan center
            builder.AddVertex(new Vector3(0, 0.0f - (height / 2.0f), 0), new Vector3(0, -1.0f, 0), new Vector2(0.5f, 0.5f));
            
            // Build the fan
            for (int i = 0; i < segments; i++)
            {
                var positionX = (float)(radius * Math.Sin(i * deltaAngle));
                var positionZ = (float)(radius * Math.Cos(i * deltaAngle));

                builder.AddVertex(
                    new Vector3(positionX, 0.0f - (height / 2.0f), positionZ),
                    new Vector3(positionX, normalY, positionZ),
                    new Vector2(1.0f - (segmentLength * i), 1.0f));
            }

            builder.AddIndices(BuildIndices(segments));
            return builder.ToResource();
        }
        
        private static uint[] BuildIndices(int segments)
        {
            IList<uint> indices = new List<uint>();
            uint index = 2;
            for (int i = 0; i < segments; i++)
            {
                // Side Triangle
                indices.Add(index);
                if (i == segments - 1)
                {
                    indices.Add(2);
                }
                else
                {
                    indices.Add(index + 1);
                }

                indices.Add(0);

                // Bottom Triangle
                indices.Add(index);
                indices.Add(1);
                if (i == segments - 1)
                {
                    indices.Add(2);
                }
                else
                {
                    indices.Add(index + 1);
                }

                index++;
            }

            uint[] data = new uint[indices.Count];
            for (int i = 0; i < indices.Count; i++)
            {
                data[i] = indices[i];
            }
            return data;
        }
    }
}
