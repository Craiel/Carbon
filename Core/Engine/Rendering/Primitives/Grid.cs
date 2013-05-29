using Core.Engine.Logic;

namespace Core.Engine.Rendering.Primitives
{
    using SlimDX;

    public static class Grid
    {
        public static void Create(TypedVector2<int> size, float spacing, out PositionNormalVertex[] vertices, out uint[] indices)
        {
            vertices = new PositionNormalVertex[(size.X + size.Y + 2) * 2];
            indices = new uint[vertices.Length];
            ushort index = 0;

            for (int i = 0; i < size.X + 1; i++)
            {
                Vector3 vertex = new Vector3(i * spacing, 0, 0);
                vertices[index].Position = vertex;
                indices[index] = index;
                index++;

                vertex = new Vector3(i, 0, size.Y * spacing);
                vertices[index].Position = vertex;
                indices[index] = index;
                index++;
            }

            for (int i = 0; i < size.Y + 1; i++)
            {
                Vector3 vertex = new Vector3(0, 0, i * spacing);
                vertices[index].Position = vertex;
                indices[index] = index;
                index++;

                vertex = new Vector3(size.X * spacing, 0, i);
                vertices[index].Position = vertex;
                indices[index] = index;
                index++;
            }
        }
    }
}
