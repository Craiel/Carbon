using System;
using System.IO;

namespace Carbon.Engine.Contracts.Resource
{
    public interface ICarbonResource : IDisposable
    {
        long Save(Stream target);
    }
}
