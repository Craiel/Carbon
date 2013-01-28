using System.IO;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource.Resources
{
    public class FontResource : ICarbonResource
    {
        internal const int Version = 2;

        public FontResource()
        {
        }

        public FontResource(Stream source)
        {
            using (var reader = new BinaryReader(source))
            {
                if (reader.ReadInt32() != Version)
                {
                    throw new InvalidDataException();
                }

                this.CharactersPerRow = reader.ReadInt32();
                this.RowCount = reader.ReadInt32();
                this.Texture = reader.ReadInt32();
            }
        }

        public int CharactersPerRow { get; set; }
        public int RowCount { get; set; }
        public int Texture { get; set; }

        public long Save(Stream target)
        {
            long size;
            using (var dataStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(dataStream))
                {
                    writer.Write(Version);
                    writer.Write(this.CharactersPerRow);
                    writer.Write(this.RowCount);
                    writer.Write(this.Texture);

                    size = dataStream.Position;
                    dataStream.Position = 0;
                    dataStream.WriteTo(target);
                }
            }

            return size;
        }
    }
}
