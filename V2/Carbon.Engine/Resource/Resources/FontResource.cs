using System.IO;

using Carbon.Engine.Logic;

namespace Carbon.Engine.Resource.Resources
{
    public class FontResource : ResourceBase
    {
        internal const int Version = 2;

        public int CharactersPerRow { get; set; }
        public int RowCount { get; set; }
        public int Texture { get; set; }

        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            if (source.ReadInt() != Version)
            {
                throw new InvalidDataException();
            }

            this.CharactersPerRow = source.ReadInt();
            this.RowCount = source.ReadInt();
            this.Texture = source.ReadInt();
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(Version);
            target.Write(this.CharactersPerRow);
            target.Write(this.RowCount);
            target.Write(this.Texture);
        }
    }
}
