using Carbon.Engine.Resource.Resources;

using SlimDX;

namespace Carbon.Engine.Rendering.Primitives
{
    public static class Cube
    {
        private static int creationCount;

        public static ModelResource Create(Vector3 origin, float size)
        {
            var builder = new MeshBuilder("Cube " + ++creationCount) { IsIndexed = true };

            float halfSize = size / 2;
            Vector3[] bottomVertices = new[]
                {
                    new Vector3(origin.X - halfSize, origin.Y - halfSize, origin.Z - halfSize),
                    new Vector3(origin.X + halfSize, origin.Y - halfSize, origin.Z - halfSize),
                    new Vector3(origin.X + halfSize, origin.Y - halfSize, origin.Z + halfSize),
                    new Vector3(origin.X - halfSize, origin.Y - halfSize, origin.Z + halfSize)
                };

            Vector3[] topVertices = new[]
                {
                    new Vector3(origin.X - halfSize, origin.Y + halfSize, origin.Z - halfSize),
                    new Vector3(origin.X + halfSize, origin.Y + halfSize, origin.Z - halfSize),
                    new Vector3(origin.X + halfSize, origin.Y + halfSize, origin.Z + halfSize),
                    new Vector3(origin.X - halfSize, origin.Y + halfSize, origin.Z + halfSize)
                };

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
            return builder.ToMesh();
        }
    }
}
