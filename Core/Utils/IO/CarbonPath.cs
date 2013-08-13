namespace Core.Utils.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    public abstract class CarbonPath
    {
        public static readonly string DirectorySeparator = System.IO.Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);
        public static readonly string DirectorySeparatorAlternative = System.IO.Path.AltDirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);

        private string path;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected CarbonPath(string path)
        {
            this.Path = path;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string DirectoryName { get; protected set; }

        public bool IsNull { get; private set; }

        public bool EndsWithSeperator { get; private set; }

        public abstract bool Exists { get; }

        public Uri GetUri()
        {
            return new Uri(this.path);
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

        public T ToRelative<T>(CarbonPath other) where T : CarbonPath
        {
            string relativePath = other.GetUri().MakeRelativeUri(this.GetUri()).ToString();
            if (string.IsNullOrEmpty(relativePath))
            {
                return null;
            }

            // Uri transforms this so we have to bring it back in line
            relativePath = relativePath.Replace("/", DirectorySeparator);
            return (T)Activator.CreateInstance(typeof(T), relativePath);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
        protected string Path
        {
            get
            {
                return this.path;
            }

            set
            {
                this.path = value;

                if (string.IsNullOrEmpty(this.path))
                {
                    this.IsNull = true;
                }
                else
                {
                    this.EndsWithSeperator = this.path.EndsWith(DirectorySeparator)
                                             || this.path.EndsWith(DirectorySeparatorAlternative);
                }
            }
        }

        protected string CombineBefore<T>(params T[] other)
        {
            string result = this.Path;
            for (int i = 0; i < other.Length; i++)
            {
                string otherValue = other[i].ToString();
                result = this.HasDelimiter(result, otherValue) ? 
                    string.Concat(result, otherValue) :
                    string.Concat(result, DirectorySeparator, otherValue);
            }

            return result;
        }

        protected bool HasDelimiter(string first, string second)
        {
            return first.EndsWith(DirectorySeparator) || second.StartsWith(DirectorySeparator);
        }

        protected string GetRelativePath(CarbonPath other)
        {
            return this.GetUri().MakeRelativeUri(other.GetUri()).LocalPath;
        }
    }
}
