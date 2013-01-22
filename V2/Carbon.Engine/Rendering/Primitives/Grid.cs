namespace Carbon.Engine.Rendering.Primitives
{
    using SlimDX;

    public static class Grid
    {
        public static void Create(int width, int height, float spacing, out PositionNormalVertex[] vertices, out uint[] indices)
        {
            vertices = new PositionNormalVertex[(width + height + 2) * 2];
            indices = new uint[vertices.Length];
            ushort index = 0;

            for (int i = 0; i < width + 1; i++)
            {
                Vector3 vertex = new Vector3(i * spacing, 0, 0);
                vertices[index].Position = vertex;
                indices[index] = index;
                index++;

                vertex = new Vector3(i, 0, height * spacing);
                vertices[index].Position = vertex;
                indices[index] = index;
                index++;
            }

            for (int i = 0; i < height + 1; i++)
            {
                Vector3 vertex = new Vector3(0, 0, i * spacing);
                vertices[index].Position = vertex;
                indices[index] = index;
                index++;

                vertex = new Vector3(width * spacing, 0, i);
                vertices[index].Position = vertex;
                indices[index] = index;
                index++;
            }
        }
    }
}
