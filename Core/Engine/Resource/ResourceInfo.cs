namespace Core.Engine.Resource
{
    public class ResourceInfo
    {
        public ResourceInfo(string hash, long size, byte[] md5)
        {
            this.Hash = hash;
            this.Size = size;
            this.Md5 = md5;
        }

        public string Hash { get; private set; }
        public long Size { get; private set; }
        public byte[] Md5 { get; private set; }
    }
}
