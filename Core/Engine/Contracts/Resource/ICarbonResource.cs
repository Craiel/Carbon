using System;
using System.IO;

namespace Core.Engine.Contracts.Resource
{
    public interface ICarbonResource : IDisposable
    {
        void Load(Stream source);
        long Save(Stream target);
    }
}
