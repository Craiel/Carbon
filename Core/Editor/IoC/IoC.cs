namespace Core.Processing.IoC
{
    using Autofac;
    using CarbonCore.Utils.IoC;
    using Core.Processing.Contracts;
    using Core.Processing.Logic;

    public class CarbonEditorModule : CarbonModule
    {
        public CarbonEditorModule()
        {
            this.For<IResourceProcessor>().Use<ResourceProcessor>().Singleton();
        }
    }
}