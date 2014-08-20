namespace Core.Processing.IoC
{
    using CarbonCore.Utils.IoC;
    using Core.Processing.Contracts;
    using Core.Processing.Logic;

    public class CarbonProcessingModule : CarbonModule
    {
        public CarbonProcessingModule()
        {
            this.For<IResourceProcessor>().Use<ResourceProcessor>().Singleton();
        }
    }
}