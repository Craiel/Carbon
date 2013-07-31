namespace Core.Utils.IO
{
    using System;
    using System.IO;

    public class CarbonFile : CarbonPath
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonFile(string path)
            : base(path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                this.FileName = System.IO.Path.GetFileName(path);
                this.DirectoryName = System.IO.Path.GetDirectoryName(path);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string FileName { get; protected set; }

        public override bool Exists
        {
            get
            {
                return base.Exists && File.Exists(this.Path);
            }
        }

        public string FileNameWithoutExtension
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(this.FileName);
            }
        }

        public string ChangeExtension(string newExtension)
        {
            if (this.IsNull)
            {
                throw new InvalidOperationException();
            }

            return System.IO.Path.ChangeExtension(this.Path, newExtension);
        }

        public FileStream OpenRead()
        {
            return File.OpenRead(this.Path);
        }

        public FileStream OpenWrite()
        {
            return File.OpenWrite(this.Path);
        }

        public void Delete()
        {
            File.Delete(this.Path);
        }

        public void DeleteIfExists()
        {
            if (!this.Exists)
            {
                return;
            }

            this.Delete();
        }

        public static CarbonFile GetTempFile()
        {
            return CarbonDirectory.TempDirectory.ToFile(System.IO.Path.GetRandomFileName());
        }

        public static CarbonFile GetRandomFile()
        {
            return new CarbonFile(System.IO.Path.GetRandomFileName());
        }
    }
}
