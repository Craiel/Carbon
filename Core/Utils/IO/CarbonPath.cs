namespace Core.Utils.IO
{
    using System;
    using System.Text;

    public abstract class CarbonPath
    {
        private readonly string path;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected CarbonPath(string path)
        {
            this.path = path;

            if (string.IsNullOrEmpty(path))
            {
                this.IsNull = true;
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string DirectoryName { get; protected set; }

        public bool IsNull { get; private set; }

        public virtual bool Exists
        {
            get
            {
                return string.IsNullOrEmpty(this.path);
            }
        }

        public override int GetHashCode()
        {
            return this.path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this.path == null)
            {
                return obj == null;
            }

            if (obj as CarbonPath == null)
            {
                return false;
            }

            return this.path.Equals(obj.ToString());
        }

        public override string ToString()
        {
            return this.path;
        }

        public virtual bool CopyTo(CarbonPath target)
        {
            throw new NotImplementedException();   
        }

        public CarbonDirectory ToDirectory()
        {
            return new CarbonDirectory(this.path);
        }

        public CarbonDirectory ToDirectory<T>(params T[] other)
        {
            return new CarbonDirectory(this.CombineBefore(other));
        }

        public CarbonFile ToFile()
        {
            return new CarbonFile(this.path);
        }

        public CarbonFile ToFile<T>(params T[] other)
        {
            return new CarbonFile(this.CombineAfter(other));
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected string Path
        {
            get
            {
                return this.path;
            }
        }

        protected string CombineBefore<T>(params T[] other)
        {
            var builder = new StringBuilder();
            builder.Append(this.Path);
            for (int i = 0; i < other.Length - 1; i++)
            {
                builder.Append(System.IO.Path.PathSeparator);
                builder.Append(other[i]);
            }

            builder.Append(other[other.Length - 1]);
            return builder.ToString();
        }

        protected string CombineAfter<T>(params T[] other)
        {
            var builder = new StringBuilder();
            
            for (int i = 0; i < other.Length; i++)
            {
                builder.Append(other[i]);
                builder.Append(System.IO.Path.PathSeparator);
            }

            builder.Append(this.Path);
            return builder.ToString();
        }
    }
}
