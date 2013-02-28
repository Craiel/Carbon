using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Resources;

using SlimDX;

namespace Carbon.Engine.Rendering.Primitives
{
    public static class Quad
    {
        private static int creationCount;
        private static int screenCreationCount;

        public static ModelResource Create(Vector3 origin, Vector3 normal, Vector3 up, float width, float height)
        {
            var builder = new MeshBuilder("Quad " + ++creationCount) { IsIndexed = true };
            
            Vector3 left = Vector3.Cross(normal, up);
            Vector3 upperCenter = (up * height / 2) + origin;
            
            Vector3 upperLeft = upperCenter + (left * width / 2);
            Vector3 upperRight = upperCenter - (left * width / 2);

            // LowerLeft
            builder.AddVertex(upperLeft - (up * height), normal, new Vector2(0.0f, 0.0f));
            builder.AddVertex(upperLeft, normal, new Vector2(1.0f, 0.0f));
            builder.AddVertex(upperRight - (up * height), normal, new Vector2(0.0f, 1.0f));
            builder.AddVertex(upperRight, normal, new Vector2(1.0f, 1.0f));

            builder.AddIndices(new uint[] { 0, 2, 1 });
            builder.AddIndices(new uint[] { 2, 3, 1 });
            
            return builder.ToMesh();
        }

        public static ModelResource CreateScreen(Vector2 position, Vector2 size)
        {
            var builder = new MeshBuilder("Screen Quad " + ++screenCreationCount) { IsIndexed = true };

            // Calculate the screen coordinates
            float left = position.X;
            float right = size.X;
            float top = position.Y;
            float bottom = size.Y;

            // LowerLeft
            builder.AddVertex(new Vector3(left, bottom, 0), Vector3.UnitZ, new Vector2(0.0f, 0.0f));
            builder.AddVertex(new Vector3(left, top, 0), Vector3.UnitZ, new Vector2(0.0f, 1.0f));
            builder.AddVertex(new Vector3(right, bottom, 0), Vector3.UnitZ, new Vector2(1.0f, 0.0f));
            builder.AddVertex(new Vector3(right, top, 0), Vector3.UnitZ, new Vector2(1.0f, 1.0f));

            builder.AddIndices(new uint[] { 0, 2, 1 });
            builder.AddIndices(new uint[] { 2, 3, 1 });

            return builder.ToMesh();
        }
    }
}
