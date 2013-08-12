namespace Core.Utils
{
    using System;
    using System.Collections.Generic;

    using SlimDX;

    public static class MathExtension
    {
        public static readonly double PiOver2 = Math.PI / 2;
        public static readonly double TwoPi = Math.PI * 2;

        public static float DegreesToRadians(float degree)
        {
            return (float)(degree * (Math.PI / 180.0f));
        }

        public static float RadiansToDegrees(float radian)
        {
            return (float)(radian * (180.0f / Math.PI));
        }

        public static float Clamp(this int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static float Clamp(this float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            var distance = new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            return (float)Math.Sqrt((distance.X * distance.X) + (distance.Y * distance.Y) + (distance.Z * distance.Z));
        }

        public static BoundingBox Transform(this BoundingBox box, Matrix matrix)
        {
            Vector4 min = Vector3.Transform(box.Minimum, matrix);
            Vector4 max = Vector3.Transform(box.Maximum, matrix);
            return new BoundingBox(new Vector3(min.X, min.Y, min.Z), new Vector3(max.X, max.Y, max.Z));
        }

        public static float ClassifyPoint(this Plane plane, ref Vector3 point)
        {
            return (point.X * plane.Normal.X) + (point.Y * plane.Normal.Y) + (point.Z * plane.Normal.Z) + plane.D;
        }

        // Returns the perpendicular distance from a point to a plane
        public static float PerpendicularDistance(this Plane plane, ref Vector3 point)
        {
            // dist = (ax + by + cz + d) / sqrt(a*a + b*b + c*c)
            return
                (float)
                Math.Abs(
                    ((plane.Normal.X * point.X) + (plane.Normal.Y * point.Y) + (plane.Normal.Z * point.Z))
                    /
                    Math.Sqrt(
                        (plane.Normal.X * plane.Normal.X) + (plane.Normal.Y * plane.Normal.Y)
                        + (plane.Normal.Z * plane.Normal.Z)));
        }

        public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent)
        {
            Vector3.Normalize(ref normal, out normal);
            Vector3 proj = normal * Vector3.Dot(tangent, normal);
            Vector3.Subtract(ref tangent, ref proj, out tangent);
            Vector3.Normalize(ref tangent, out tangent);
        }

        public static IList<int> ComputePrimes(int max)
        {
            var primes = new bool[max + 1];
            var sqrt = (int)Math.Sqrt(max);
            for (int x = 1; x < sqrt; x++)
            {
                var squareX = x * x;
                for (int y = 1; y <= sqrt; y++)
                {
                    var squareY = y * y;
                    var n = (4 * squareX) + squareY;
                    if (n <= max && (n % 12 == 1 || n % 12 == 5))
                    {
                        primes[n] ^= true;
                    }

                    n = (3 * squareX) + squareY;
                    if (n <= max && n % 12 == 7)
                    {
                        primes[n] ^= true;
                    }

                    n = (3 * squareX) - squareY;
                    if (x > y && n <= max && n % 12 == 11)
                    {
                        primes[n] ^= true;
                    }
                }
            }

            var primeList = new List<int> { 2, 3 };
            for (int i = 5; i <= sqrt; i++)
            {
                if (primes[i])
                {
                    primeList.Add(i);
                    int square = i * i;
                    for (int k = square; k < max; k += square)
                    {
                        primes[k] = false;
                    }
                }
            }

            for (int i = sqrt + 1; i <= max; i++)
            {
                if (primes[i])
                {
                    primeList.Add(i);
                }
            }

            return primeList;
        }
    }
}
