namespace Core.Utils.IO
{
    using System;
    using System.IO;
    using System.Xml;

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
                this.Extension = System.IO.Path.GetExtension(path);
                this.DirectoryName = System.IO.Path.GetDirectoryName(path);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string FileName { get; protected set; }
        public string Extension { get; protected set; }

        public DateTime LastWriteTime
        {
            get
            {
                return File.GetLastWriteTime(this.Path);
            }
        }

        public long Size
        {
            get
            {
                if (!this.Exists)
                {
                    return -1;
                }

                var info = new FileInfo(this.Path);
                return info.Length;
            }
        }

        public override bool Exists
        {
            get
            {
                return File.Exists(this.Path);
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

        public FileStream OpenCreate()
        {
            return new FileStream(this.Path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
        }

        public FileStream OpenRead()
        {
            return File.OpenRead(this.Path);
        }

        public FileStream OpenWrite()
        {
            return File.OpenWrite(this.Path);
        }

        public XmlReader OpenXmlRead()
        {
            return XmlReader.Create(this.Path);
        }

        public XmlWriter OpenXmlWrite()
        {
            return XmlWriter.Create(this.Path);
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

        public CarbonDirectory ToDirectory()
        {
            return new CarbonDirectory(this.DirectoryName);
        }
        
        public CarbonFile ToFile<T>(params T[] other)
        {
            return new CarbonFile(this.Path + string.Concat(other));
        }

        public static CarbonFile GetTempFile()
        {
            return CarbonDirectory.TempDirectory.ToFile(System.IO.Path.GetRandomFileName());
        }

        public static CarbonFile GetRandomFile()
        {
            return new CarbonFile(System.IO.Path.GetRandomFileName());
        }

        public static bool FileExists(CarbonFile file)
        {
            return file != null && !file.IsNull && file.Exists;
        }
    }
}
