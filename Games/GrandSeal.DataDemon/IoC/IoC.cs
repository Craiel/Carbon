﻿namespace GrandSeal.DataDemon.IoC
{
    using CarbonCore.Utils.IoC;

    using Core.Engine.IoC;
    using Core.Processing.IoC;

    using GrandSeal.DataDemon.Contracts;
    using GrandSeal.DataDemon.Logic;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(EngineModule))]
    [DependsOnModule(typeof(DataDemonModule))]
    [DependsOnModule(typeof(CarbonEditorModule))]
    public class DataDemonModule : CarbonModule
    {
        public DataDemonModule()
        {
            this.For<IDataDemon>().Use<DataDemon>().Singleton();

            this.For<IDemonLogic>().Use<DemonLogic>().Singleton();
            this.For<IDemonLog>().Use<DemonLog>().Singleton();

            this.For<IDemonFileInfo>().Use<DemonFileInfo>().Singleton();
            this.For<IDemonBuild>().Use<DemonBuild>();
        }
    }
}