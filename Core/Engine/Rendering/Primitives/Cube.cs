using Core.Engine.Resource;
using Core.Engine.Resource.Resources.Model;

using SlimDX;

namespace Core.Engine.Rendering.Primitives
{
    public static class Cube
    {
        private static int creationCount;

        public static ModelResource Create(Vector3 origin, float size)
        {
            var builder = new ModelBuilder("Cube " + ++creationCount) { IsIndexed = true };

            Vector3[] bottomVertices;
            Vector3[] topVertices;
            GetVertices(origin, size, out bottomVertices, out topVertices);

            builder.AddVertex(topVertices[0], Vector3.UnitY, new Vector2(0.0f, 0.0f));
            builder.AddVertex(topVertices[1], Vector3.UnitY, new Vector2(1.0f, 0.0f));
            builder.AddVertex(topVertices[2], Vector3.UnitY, new Vector2(1.0f, 0.0f));
            builder.AddVertex(topVertices[3], Vector3.UnitY, new Vector2(0.0f, 1.0f));

            builder.AddVertex(bottomVertices[0], -Vector3.UnitY, new Vector2(0.0f, 0.0f));
            builder.AddVertex(bottomVertices[1], -Vector3.UnitY, new Vector2(1.0f, 0.0f));
            builder.AddVertex(bottomVertices[2], -Vector3.UnitY, new Vector2(1.0f, 0.0f));
            builder.AddVertex(bottomVertices[3], -Vector3.UnitY, new Vector2(0.0f, 1.0f));

            builder.AddVertex(bottomVertices[3], Vector3.UnitZ, new Vector2(0.0f, 0.0f));
            builder.AddVertex(bottomVertices[0], Vector3.UnitZ, new Vector2(1.0f, 0.0f));
            builder.AddVertex(topVertices[0], Vector3.UnitZ, new Vector2(1.0f, 1.0f));
            builder.AddVertex(topVertices[3], Vector3.UnitZ, new Vector2(0.0f, 1.0f));

            builder.AddVertex(bottomVertices[2], Vector3.UnitZ, new Vector2(0.0f, 0.0f));
            builder.AddVertex(bottomVertices[1], Vector3.UnitZ, new Vector2(1.0f, 0.0f));
            builder.AddVertex(topVertices[1], Vector3.UnitZ, new Vector2(1.0f, 1.0f));
            builder.AddVertex(topVertices[2], Vector3.UnitZ, new Vector2(0.0f, 1.0f));

            builder.AddVertex(bottomVertices[0], Vector3.UnitZ, new Vector2(0.0f, 0.0f));
            builder.AddVertex(bottomVertices[1], Vector3.UnitZ, new Vector2(1.0f, 0.0f));
            builder.AddVertex(topVertices[1], Vector3.UnitZ, new Vector2(1.0f, 1.0f));
            builder.AddVertex(topVertices[0], Vector3.UnitZ, new Vector2(0.0f, 1.0f));

            builder.AddVertex(bottomVertices[3], Vector3.UnitZ, new Vector2(0.0f, 0.0f));
            builder.AddVertex(bottomVertices[2], Vector3.UnitZ, new Vector2(1.0f, 0.0f));
            builder.AddVertex(topVertices[2], Vector3.UnitZ, new Vector2(1.0f, 1.0f));
            builder.AddVertex(topVertices[3], Vector3.UnitZ, new Vector2(0.0f, 1.0f));
            
            var indices = new uint[]
                {
                    3, 1, 0, 
                    2, 1, 3, 

                    6, 4, 5, 
                    7, 4, 6, 

                    11, 9, 8, 
                    10, 9, 11, 

                    14, 12, 13, 
                    15, 12, 14, 

                    19, 17, 16, 
                    18, 17, 19, 

                    22, 20, 21,
                    23, 20, 22
                };

            builder.AddIndices(indices);
            return builder.ToResource();
        }

        public static ModelResource CreateBoundingBoxLines(BoundingBox box, Vector4 color)
        {
            var builder = new ModelBuilder("Cube " + ++creationCount) { IsIndexed = true };

            Vector3[] corners = box.GetCorners();

            foreach (Vector3 corner in corners)
            {
                builder.AddVertex(corner, color: color);
            }

            var indices = new uint[]
                {
                    0, 1, 2, 3,
                    4, 5, 6, 7,

                    0, 4, 1, 5,
                    2, 6, 3, 7,

                    0, 3, 1, 2,
                    4, 7, 5, 6
                };

            builder.AddIndices(indices);
            return builder.ToResource();
        }

        private static void GetVertices(Vector3 origin, float size, out Vector3[] bottomVertices, out Vector3[] topVertices)
        {
            float halfSize = size / 2;

            bottomVertices = new[]
                {
                    new Vector3(origin.X - halfSize, origin.Y - halfSize, origin.Z - halfSize),
                    new Vector3(origin.X + halfSize, origin.Y - halfSize, origin.Z - halfSize),
                    new Vector3(origin.X + halfSize, origin.Y - halfSize, origin.Z + halfSize),
                    new Vector3(origin.X - halfSize, origin.Y - halfSize, origin.Z + halfSize)
                };

            topVertices = new[]
                {
                    new Vector3(origin.X - halfSize, origin.Y + halfSize, origin.Z - halfSize),
                    new Vector3(origin.X + halfSize, origin.Y + halfSize, origin.Z - halfSize),
                    new Vector3(origin.X + halfSize, origin.Y + halfSize, origin.Z + halfSize),
                    new Vector3(origin.X - halfSize, origin.Y + halfSize, origin.Z + halfSize)
                };
        }
    }
}
