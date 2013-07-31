namespace Core.Utils.IO
{
    public class CarbonDirectory : CarbonPath
    {
        public static readonly CarbonDirectory TempDirectory = new CarbonDirectory(System.IO.Path.GetTempPath());

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonDirectory(string path)
            : base(path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // Check if we are creating from a File
                if (System.IO.File.Exists(path))
                {
                    this.DirectoryName = System.IO.Path.GetDirectoryName(path);
                }
                else
                {
                    this.DirectoryName = path;   
                }
            }
        }

        public CarbonDirectory(CarbonFile file)
            : this(file.DirectoryName)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool Exists
        {
            get
            {
                return base.Exists && System.IO.Directory.Exists(this.Path);
            }
        }

        public void Create()
        {
            if (this.Exists)
            {
                return;
            }

            System.IO.Directory.CreateDirectory(this.Path);
        }

        public void Delete(bool recursive = false)
        {
            System.IO.Directory.Delete(this.Path, recursive);
        }

        public static CarbonDirectory GetTempDirectory()
        {
            return TempDirectory.ToDirectory(System.IO.Path.GetRandomFileName());
        }
    }
}
