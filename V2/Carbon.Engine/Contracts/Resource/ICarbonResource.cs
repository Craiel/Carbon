using System.IO;

namespace Carbon.Engine.Contracts.Resource
{
    public interface ICarbonResource
    {
        long Save(Stream target);
    }
}
