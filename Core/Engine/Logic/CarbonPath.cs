namespace Core.Engine.Logic
{
    using System;

    public class CarbonPath
    {
        private readonly string path;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonPath(string path)
        {
            this.path = path;

            if (!string.IsNullOrEmpty(path))
            {
                this.FileName = System.IO.Path.GetFileName(path);
                this.DirectoryName = System.IO.Path.GetDirectoryName(path);
            }
            else
            {
                this.IsNull = true;
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string FileName { get; private set; }
        public string DirectoryName { get; private set; }

        public string FileNameWithoutExtension
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(this.FileName);
            }
        }

        public bool IsNull { get; private set; }

        public bool Exists
        {
            get
            {
                return string.IsNullOrEmpty(this.path) || System.IO.File.Exists(this.path);
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
            }//                                        <WrapPanel ItemWidth="150" IsItemsHost="True" MinWidth="100" Width="{Binding ActualWidth,RelativeSource={RelativeSource AncestorType=ScrollContentPresenter}}"/>

            return this.path.Equals(obj.ToString());
        }

        public override string ToString()
        {
            return this.path;
        }

        public bool CopyTo(CarbonPath target)
        {
            throw new NotImplementedException();   
        }

        public string ChangeExtension(string newExtension)
        {
            if (this.IsNull)
            {
                throw new InvalidOperationException();
            }

            return System.IO.Path.ChangeExtension(this.path, newExtension);
        }
    }
}
