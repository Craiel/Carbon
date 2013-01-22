using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Core.Utils
{
    public static class VectorExtension
    {
        static VectorExtension()
        {
            Vector3Identity = new Vector3(1.0f);
            Vector4Identity = new Vector4(1.0f);
        }

        public static Vector3 Vector3Identity { get; private set; }
        public static Vector4 Vector4Identity { get; private set; }
    }
}
