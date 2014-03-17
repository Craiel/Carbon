namespace Core.Processing.IoC
{
    using Autofac;

    using Core.Processing.Contracts;
    using Core.Processing.Logic;

    public class CarbonEditorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ResourceProcessor>().As<IResourceProcessor>().SingleInstance();
        }
    }
}