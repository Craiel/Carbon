using System;
using System.IO;

namespace Carbon.Engine.Contracts.Resource
{
    public interface ICarbonResource : IDisposable
    {
        void Load(Stream source);
        long Save(Stream target);
    }
}
