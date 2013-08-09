﻿namespace Core.Engine.Rendering
{
    using System;
    using System.Text;

    using Core.Utils;

    using SlimDX;

    // Taken from Mono.XNA implementation
    public class BoundingFrustum : IEquatable<BoundingFrustum>
    {
        public const int CornerCount = 8;

        private Matrix matrix;
        private Plane bottom;
        private Plane far;
        private Plane left;
        private Plane right;
        private Plane near;
        private Plane top;
        private Vector3[] corners;

        public BoundingFrustum(Matrix value)
        {
            this.matrix = value;
            this.CreatePlanes();
            this.CreateCorners();
        }

        public BoundingFrustum(Matrix view, Matrix projection, float depth)
        {
            float minimumZ = -projection.M43 / projection.M33;
            float r = depth / (depth - minimumZ);
            projection.M33 = r;
            projection.M43 = -r * minimumZ;
            this.matrix = Matrix.Multiply(view, projection);
            this.CreatePlanes();
            this.CreateCorners();
        }

        public Plane Bottom
        {
            get
            {
                return this.bottom;
            }
        }

        public Plane Far
        {
            get
            {
                return this.far;
            }
        }

        public Plane Left
        {
            get
            {
                return this.left;
            }
        }

        public Matrix Matrix
        {
            get
            {
                return this.matrix;
            }

            set
            {
                this.matrix = value;
                this.CreatePlanes();    // FIXME: The odds are the planes will be used a lot more often than the matrix
                this.CreateCorners();   // is updated, so this should help performance. I hope ;)
            }
        }

        public Plane Near
        {
            get
            {
                return this.near;
            }
        }

        public Plane Right
        {
            get
            {
                return this.right;
            }
        }

        public Plane Top
        {
            get
            {
                return this.top;
            }
        }

        public static bool operator ==(BoundingFrustum a, BoundingFrustum b)
        {
            if (Equals(a, null))
            {
                return Equals(b, null);
            }

            if (Equals(b, null))
            {
                return false;
            }

            return a.matrix == (b.matrix);
        }

        public static bool operator !=(BoundingFrustum a, BoundingFrustum b)
        {
            return !(a == b);
        }

        public ContainmentType Contains(BoundingBox box)
        {
            ContainmentType result;
            this.Contains(ref box, out result);
            return result;
        }

        public void GetCorners(Vector3[] corners)
        {
            throw new NotImplementedException();
        }

        public void Contains(ref BoundingBox box, out ContainmentType result)
        {
            // FIXME: Is this a bug?
            // If the bounding box is of W * D * H = 0, then return disjoint
            if (box.Minimum == box.Maximum)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            int i;
            ContainmentType contained;
            Vector3[] boxCorners = box.GetCorners();

            // First we assume completely disjoint. So if we find a point that is contained, we break out of this loop
            for (i = 0; i < boxCorners.Length; i++)
            {
                this.Contains(ref boxCorners[i], out contained);
                if (contained != ContainmentType.Disjoint)
                    break;
            }

            if (i == boxCorners.Length) // This means we checked all the corners and they were all disjoint
            {
                result = ContainmentType.Disjoint;
                return;
            }

            if (i != 0)             // if i is not equal to zero, we can fastpath and say that this box intersects
            {                       // because we know at least one point is outside and one is inside.
                result = ContainmentType.Intersects;
                return;
            }

            // If we get here, it means the first (and only) point we checked was actually contained in the frustum.
            // So we assume that all other points will also be contained. If one of the points is disjoint, we can
            // exit immediately saying that the result is Intersects
            i++;
            for (; i < boxCorners.Length; i++)
            {
                this.Contains(ref boxCorners[i], out contained);
                if (contained != ContainmentType.Contains)
                {
                    result = ContainmentType.Intersects;
                    return;
                }
            }

            // If we get here, then we know all the points were actually contained, therefore result is Contains
            result = ContainmentType.Contains;
        }

        // TODO: Implement this
        public ContainmentType Contains(BoundingFrustum frustum)
        {
            if (this == frustum)                // We check to see if the two frustums are equal
                return ContainmentType.Contains;// If they are, there's no need to go any further.

            throw new NotImplementedException();
        }

        public ContainmentType Contains(BoundingSphere sphere)
        {
            ContainmentType result;
            this.Contains(ref sphere, out result);
            return result;
        }

        public void Contains(ref BoundingSphere sphere, out ContainmentType result)
        {
            ContainmentType contained;

            // We first check if the sphere is inside the frustum
            this.Contains(ref sphere.Center, out contained);

            // The sphere is inside. Now we need to check if it's fully contained or not
            // So we see if the perpendicular distance to each plane is less than or equal to the sphere's radius.
            // If the perpendicular distance is less, just return Intersects.
            if (contained == ContainmentType.Contains)
            {
                float val = this.bottom.PerpendicularDistance(ref sphere.Center);
                if (val < sphere.Radius)
                {
                    result = ContainmentType.Intersects;
                    return;
                }

                val = this.far.PerpendicularDistance(ref sphere.Center);
                if (val < sphere.Radius)
                {
                    result = ContainmentType.Intersects;
                    return;
                }

                val = this.left.PerpendicularDistance(ref sphere.Center);
                if (val < sphere.Radius)
                {
                    result = ContainmentType.Intersects;
                    return;
                }

                val = this.near.PerpendicularDistance(ref sphere.Center);
                if (val < sphere.Radius)
                {
                    result = ContainmentType.Intersects;
                    return;
                }

                val = this.right.PerpendicularDistance(ref sphere.Center);
                if (val < sphere.Radius)
                {
                    result = ContainmentType.Intersects;
                    return;
                }

                val = this.top.PerpendicularDistance(ref sphere.Center);
                if (val < sphere.Radius)
                {
                    result = ContainmentType.Intersects;
                    return;
                }

                // If we get here, the sphere is fully contained
                result = ContainmentType.Contains;
                return;
            }
            //duff idea : test if all corner is in same side of a plane if yes and outside it is disjoint else intersect
            // issue is that we can have some times when really close aabb 



            // If we're here, the the sphere's centre was outside of the frustum. This makes things hard :(
            // We can't use perpendicular distance anymore. I'm not sure how to code this.
            throw new NotImplementedException();
        }

        public ContainmentType Contains(Vector3 point)
        {
            ContainmentType result;
            this.Contains(ref point, out result);
            return result;
        }

        public void Contains(ref Vector3 point, out ContainmentType result)
        {
            // If a point is on the POSITIVE side of the plane, then the point is not contained within the frustum

            // Check the top
            float val = this.top.ClassifyPoint(ref point);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the bottom
            val = this.bottom.ClassifyPoint(ref point);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the left
            val = this.left.ClassifyPoint(ref point);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the right
            val = this.right.ClassifyPoint(ref point);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the near
            val = this.near.ClassifyPoint(ref point);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // Check the far
            val = this.far.ClassifyPoint(ref point);
            if (val > 0)
            {
                result = ContainmentType.Disjoint;
                return;
            }

            // If we get here, it means that the point was on the correct side of each plane to be
            // contained. Therefore this point is contained
            result = ContainmentType.Contains;
        }

        public bool Equals(BoundingFrustum other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            var frustum = obj as BoundingFrustum;
            return (!Equals(frustum, null)) && (this == frustum);
        }

        public Vector3[] GetCorners()
        {
            return corners;
        }

        public override int GetHashCode()
        {
            return this.matrix.GetHashCode();
        }

        public bool Intersects(BoundingBox box)
        {
            throw new NotImplementedException();
        }

        public void Intersects(ref BoundingBox box, out bool result)
        {
            throw new NotImplementedException();
        }

        public bool Intersects(BoundingFrustum frustum)
        {
            throw new NotImplementedException();
        }

        public bool Intersects(BoundingSphere sphere)
        {
            throw new NotImplementedException();
        }

        public void Intersects(ref BoundingSphere sphere, out bool result)
        {
            throw new NotImplementedException();
        }

        public PlaneIntersectionType Intersects(Plane plane)
        {
            throw new NotImplementedException();
        }

        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            throw new NotImplementedException();
        }

        public float? Intersects(Ray ray)
        {
            throw new NotImplementedException();
        }

        public void Intersects(ref Ray ray, out float? result)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var sb = new StringBuilder(256);
            sb.Append("{Near:");
            sb.Append(this.near.ToString());
            sb.Append(" Far:");
            sb.Append(this.far.ToString());
            sb.Append(" Left:");
            sb.Append(this.left.ToString());
            sb.Append(" Right:");
            sb.Append(this.right.ToString());
            sb.Append(" Top:");
            sb.Append(this.top.ToString());
            sb.Append(" Bottom:");
            sb.Append(this.bottom.ToString());
            sb.Append("}");
            return sb.ToString();
        }

        private void CreateCorners()
        {
            this.corners = new Vector3[8];
            this.corners[0] = IntersectionPoint(ref this.near, ref this.left, ref this.top);
            this.corners[1] = IntersectionPoint(ref this.near, ref this.right, ref this.top);
            this.corners[2] = IntersectionPoint(ref this.near, ref this.right, ref this.bottom);
            this.corners[3] = IntersectionPoint(ref this.near, ref this.left, ref this.bottom);
            this.corners[4] = IntersectionPoint(ref this.far, ref this.left, ref this.top);
            this.corners[5] = IntersectionPoint(ref this.far, ref this.right, ref this.top);
            this.corners[6] = IntersectionPoint(ref this.far, ref this.right, ref this.bottom);
            this.corners[7] = IntersectionPoint(ref this.far, ref this.left, ref this.bottom);
        }

        private void CreatePlanes()
        {
            // Pre-calculate the different planes needed
            this.left = new Plane(-this.matrix.M14 - this.matrix.M11, -this.matrix.M24 - this.matrix.M21,
                                  -this.matrix.M34 - this.matrix.M31, -this.matrix.M44 - this.matrix.M41);

            this.right = new Plane(this.matrix.M11 - this.matrix.M14, this.matrix.M21 - this.matrix.M24,
                                   this.matrix.M31 - this.matrix.M34, this.matrix.M41 - this.matrix.M44);

            this.top = new Plane(this.matrix.M12 - this.matrix.M14, this.matrix.M22 - this.matrix.M24,
                                 this.matrix.M32 - this.matrix.M34, this.matrix.M42 - this.matrix.M44);

            this.bottom = new Plane(-this.matrix.M14 - this.matrix.M12, -this.matrix.M24 - this.matrix.M22,
                                    -this.matrix.M34 - this.matrix.M32, -this.matrix.M44 - this.matrix.M42);

            this.near = new Plane(-this.matrix.M13, -this.matrix.M23, -this.matrix.M33, -this.matrix.M43);


            this.far = new Plane(this.matrix.M13 - this.matrix.M14, this.matrix.M23 - this.matrix.M24,
                                 this.matrix.M33 - this.matrix.M34, this.matrix.M43 - this.matrix.M44);

            this.NormalizePlane(ref this.left);
            this.NormalizePlane(ref this.right);
            this.NormalizePlane(ref this.top);
            this.NormalizePlane(ref this.bottom);
            this.NormalizePlane(ref this.near);
            this.NormalizePlane(ref this.far);
        }

        private static Vector3 IntersectionPoint(ref Plane a, ref Plane b, ref Plane c)
        {
            /* Formula used
            //                d1 ( N2 * N3 ) + d2 ( N3 * N1 ) + d3 ( N1 * N2 )
            // P =       -------------------------------------------------------------------------
            //                             N1 . ( N2 * N3 )
            //
            // Note: N refers to the normal, d refers to the displacement. '.' means dot product. '*' means cross product*/

            Vector3 v1, v2, v3;
            float f = -Vector3.Dot(a.Normal, Vector3.Cross(b.Normal, c.Normal));

            v1 = a.D * Vector3.Cross(b.Normal, c.Normal);
            v2 = b.D * Vector3.Cross(c.Normal, a.Normal);
            v3 = c.D * Vector3.Cross(a.Normal, b.Normal);

            var vec = new Vector3(v1.X + v2.X + v3.X, v1.Y + v2.Y + v3.Y, v1.Z + v2.Z + v3.Z);
            return vec / f;
        }

        private void NormalizePlane(ref Plane p)
        {
            float factor = 1f / p.Normal.Length();
            p.Normal.X *= factor;
            p.Normal.Y *= factor;
            p.Normal.Z *= factor;
            p.D *= factor;
        }
    }
}