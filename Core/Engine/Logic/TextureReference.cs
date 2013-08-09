namespace Core.Engine.Logic
{
    using System;

    public class TextureReference
    {
        internal TextureReference(TextureReferenceDescription description)
        {
            this.Register = description.Register;
            this.Type = description.Type;
            this.Size = description.Size;
            this.IsValid = true;
        }

        internal TextureReference(string resourceHash, TextureReferenceDescription description)
            : this(description)
        {
            this.ResourceHash = resourceHash;
        }

        internal TextureReference(byte[] data, TextureReferenceDescription description)
            : this(description)
        {
            this.Data = data;
        }

        public TextureReferenceType Type { get; private set; }

        public string ResourceHash { get; private set; }

        public byte[] Data { get; set; }

        public int Register { get; private set; }

        public TypedVector2<int> Size { get; private set; }

        public bool IsValid { get; private set; }

        public void Invalidate()
        {
            this.IsValid = false;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.ResourceHash).GetHashCode();
        }
    }
}
