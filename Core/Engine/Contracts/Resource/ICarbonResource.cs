namespace Core.Engine.Contracts.Resource
{
    using System;
    using System.IO;

    public interface ICarbonResource : IDisposable
    {
        void Load(Stream source);
        long Save(Stream target);
    }
}
