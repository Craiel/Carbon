﻿namespace GrandSeal.DataDemon.IoC
{
    using CarbonCore.Processing.IoC;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;

    using Core.Engine.IoC;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.Logic;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(EngineModule))]
    [DependsOnModule(typeof(DataDemonModule))]
    [DependsOnModule(typeof(CarbonProcessingModule))]
    public class DataDemonModule : CarbonQuickModule
    {
        public DataDemonModule()
        {
            this.For<IDataDemon>().Use<DataDemon>().Singleton();

            this.For<IDemonLogic>().Use<DemonLogic>().Singleton();

            this.For<IDemonFileInfo>().Use<DemonFileInfo>().Singleton();
            this.For<IDemonBuild>().Use<DemonBuild>();
        }
    }
}