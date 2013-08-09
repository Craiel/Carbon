namespace Core.Engine.Logic
{
    using System;

    public struct TypedVector2<T>
    {
        public T X;
        public T Y;

        public TypedVector2(T n)
        {
            this.X = n;
            this.Y = n;
        }

        public TypedVector2(T x, T y)
        {
            this.X = x;
            this.Y = y;
        }

        public static bool operator ==(TypedVector2<T> a, TypedVector2<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TypedVector2<T> a, TypedVector2<T> b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            var other = (TypedVector2<T>)obj;
            return other.X.Equals(this.X) && other.Y.Equals(this.Y);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.X, this.Y).GetHashCode();
        }
    }

    public struct TypedVector3<T>
    {
        public T X;
        public T Y;
        public T Z;

        public TypedVector3(T n)
        {
            this.X = n;
            this.Y = n;
            this.Z = n;
        }

        public TypedVector3(T x, T y, T z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static bool operator ==(TypedVector3<T> a, TypedVector3<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TypedVector3<T> a, TypedVector3<T> b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            var other = (TypedVector3<T>)obj;
            return other.X.Equals(this.X) && other.Y.Equals(this.Y) && other.Z.Equals(this.Z);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.X, this.Y, this.Z).GetHashCode();
        }
    }

    public struct TypedVector4<T>
    {
        public T X;
        public T Y;
        public T Z;
        public T W;

        public TypedVector4(T n)
        {
            this.X = n;
            this.Y = n;
            this.Z = n;
            this.W = n;
        }

        public TypedVector4(T x, T y, T z, T w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public static bool operator ==(TypedVector4<T> a, TypedVector4<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TypedVector4<T> a, TypedVector4<T> b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            var other = (TypedVector4<T>)obj;
            return other.X.Equals(this.X) && other.Y.Equals(this.Y) && other.Z.Equals(this.Z) && other.W.Equals(this.W);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.X, this.Y, this.Z, this.W).GetHashCode();
        }
    }
}
