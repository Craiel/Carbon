namespace Core.Engine.Rendering.Primitives
{
    using System;
    using System.Collections.Generic;

    using Core.Engine.Resource;
    using Core.Engine.Resource.Resources.Model;

    using SharpDX;

    public static class Sphere
    {
        private static int creationCount;

        // Sphare UV Generation: http://sol.gfxile.net/sphere/index.html
        private struct TriangleIndices
        {
            public readonly uint v1;
            public readonly uint v2;
            public readonly uint v3;

            public TriangleIndices(uint v1, uint v2, uint v3)
            {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
            }
        }

        private static readonly IList<Vector3> pointList = new List<Vector3>();
        private static readonly Dictionary<Int64, uint> middlePointIndexCache = new Dictionary<long, uint>();

        private static uint index;
        
        // add vertex to mesh, fix position to be on unit sphere, return index
        private static uint AddSphereVertex(Vector3 p)
        {
            float length = (float)Math.Sqrt(p.X * p.X + p.Y * p.Y + p.Z * p.Z);
            pointList.Add(new Vector3(p.X / length, p.Y / length, p.Z / length));
            return index++;
        }

        // return index of point in the middle of p1 and p2
        private static uint GetMiddlePoint(uint p1, uint p2)
        {
            // first check if we have it already
            bool firstIsSmaller = p1 < p2;
            Int64 smallerIndex = firstIsSmaller ? p1 : p2;
            Int64 greaterIndex = firstIsSmaller ? p2 : p1;
            Int64 key = (smallerIndex << 32) + greaterIndex;

            uint ret;
            if (middlePointIndexCache.TryGetValue(key, out ret))
            {
                return ret;
            }

            // not in cache, calculate it
            Vector3 point1 = pointList[(int)p1];
            Vector3 point2 = pointList[(int)p2];
            Vector3 middle = new Vector3(
                (point1.X + point2.X) / 2.0f,
                (point1.Y + point2.Y) / 2.0f,
                (point1.Z + point2.Z) / 2.0f);

            // add vertex makes sure point is on unit sphere
            uint i = AddSphereVertex(middle);

            // store it, return index
            middlePointIndexCache.Add(key, i);
            return i;
        }

        public static ModelResource Create(int recursionLevel)
        {
            pointList.Clear();
            middlePointIndexCache.Clear();
            index = 0;

            // create 12 vertices of a icosahedron
            float t = (1.0f + (float)Math.Sqrt(5.0)) / 2.0f;

            AddSphereVertex(new Vector3(-1, t, 0));
            AddSphereVertex(new Vector3(1, t, 0));
            AddSphereVertex(new Vector3(-1, -t, 0));
            AddSphereVertex(new Vector3(1, -t, 0));

            AddSphereVertex(new Vector3(0, -1, t));
            AddSphereVertex(new Vector3(0, 1, t));
            AddSphereVertex(new Vector3(0, -1, -t));
            AddSphereVertex(new Vector3(0, 1, -t));

            AddSphereVertex(new Vector3(t, 0, -1));
            AddSphereVertex(new Vector3(t, 0, 1));
            AddSphereVertex(new Vector3(-t, 0, -1));
            AddSphereVertex(new Vector3(-t, 0, 1));


            // create 20 triangles of the icosahedron
            var faces = new List<TriangleIndices>();

            // 5 faces around point 0
            faces.Add(new TriangleIndices(0, 11, 5));
            faces.Add(new TriangleIndices(0, 5, 1));
            faces.Add(new TriangleIndices(0, 1, 7));
            faces.Add(new TriangleIndices(0, 7, 10));
            faces.Add(new TriangleIndices(0, 10, 11));

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(1, 5, 9));
            faces.Add(new TriangleIndices(5, 11, 4));
            faces.Add(new TriangleIndices(11, 10, 2));
            faces.Add(new TriangleIndices(10, 7, 6));
            faces.Add(new TriangleIndices(7, 1, 8));

            // 5 faces around point 3
            faces.Add(new TriangleIndices(3, 9, 4));
            faces.Add(new TriangleIndices(3, 4, 2));
            faces.Add(new TriangleIndices(3, 2, 6));
            faces.Add(new TriangleIndices(3, 6, 8));
            faces.Add(new TriangleIndices(3, 8, 9));

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(4, 9, 5));
            faces.Add(new TriangleIndices(2, 4, 11));
            faces.Add(new TriangleIndices(6, 2, 10));
            faces.Add(new TriangleIndices(8, 6, 7));
            faces.Add(new TriangleIndices(9, 8, 1));


            // refine triangles
            for (int i = 0; i < recursionLevel; i++)
            {
                var faces2 = new List<TriangleIndices>();
                foreach (var tri in faces)
                {
                    // replace triangle by 4 triangles
                    uint a = GetMiddlePoint(tri.v1, tri.v2);
                    uint b = GetMiddlePoint(tri.v2, tri.v3);
                    uint c = GetMiddlePoint(tri.v3, tri.v1);

                    faces2.Add(new TriangleIndices(tri.v1, a, c));
                    faces2.Add(new TriangleIndices(tri.v2, b, a));
                    faces2.Add(new TriangleIndices(tri.v3, c, b));
                    faces2.Add(new TriangleIndices(a, b, c));
                }
                faces = faces2;
            }

            var builder = new ModelBuilder("Sphere " + ++creationCount) { IsIndexed = true };
            for (int i = 0; i < pointList.Count; i++)
            {
                // Calculate the UV
                double len = Math.Sqrt(pointList[i].X * pointList[i].X + pointList[i].Y * pointList[i].Y + pointList[i].Z * pointList[i].Z);
                float u = (float)Math.Acos(pointList[i].Y / len) / (float)Math.PI;
                float v = (float)(Math.Atan2(pointList[i].Z, pointList[i].X) / Math.PI + 1.0f) * 0.5f;

                // Todo: fix normal
                builder.AddVertex(pointList[i], pointList[i], new Vector2(u, v));

                /*vertices[i].Position = pointList[i];
                vertices[i].Normal = pointList[i]; // Todo: fix normal
                vertices[i].Texture = new Vector2(u, v);*/
            }

            // Todo: Fix the UV Tear by using the triangle indizes to find all Edge Triangles with a UV difference bigger than 0.xx and generate an extra set of triangles
            // Todo: Fix the UV Cap by duplicating the pole vertex and averaging the u of the other two vertices of each of those triangles

            index = 0;
            foreach (var triangle in faces)
            {
                builder.AddIndices(new[] { triangle.v1, triangle.v2, triangle.v3 });
                index += 3;
            }

            return builder.ToResource();
        }
    }
}
