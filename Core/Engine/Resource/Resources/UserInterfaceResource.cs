using System.IO;

using Google.ProtocolBuffers;

namespace Core.Engine.Resource.Resources
{
    public class UserInterfaceResource : ProtocolResource
    {
        internal const int Version = 1;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string CsamlData { get; set; }
        public ScriptResource Script { get; set; }

        public override void Load(Stream source)
        {
            Protocol.Resource.UserInterface entry = Protocol.Resource.UserInterface.ParseFrom(source);

            if (entry.Version != Version)
            {
                throw new InvalidDataException("UserInterface version is not correct: " + entry.Version);
            }

            this.CsamlData = entry.Csaml.ToStringUtf8();

            this.Script = new ScriptResource();
            this.Script.Load(entry.Script);
        }

        public override long Save(Stream target)
        {
            if (string.IsNullOrEmpty(this.CsamlData))
            {
                throw new InvalidDataException("CsamlData was empty on Save");
            }

            var builder = new Protocol.Resource.UserInterface.Builder
                              {
                                  Version = Version,
                                  Csaml = ByteString.CopyFromUtf8(this.CsamlData),
                                  Script = this.Script.Save()
                              };

            Protocol.Resource.UserInterface entry = builder.Build();
            entry.WriteTo(target);
            return entry.SerializedSize;
        }
    }
}
