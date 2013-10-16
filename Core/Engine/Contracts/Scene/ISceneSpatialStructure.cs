namespace Core.Engine.Contracts.Scene
{
    using System;
    using System.Collections.Generic;

    public interface ISceneSpatialStructure : IDisposable
    {
        IList<ILightEntity> GetLights();
        IList<ICameraEntity> GetCameras();
        IList<IModelEntity> GetModels();

        IList<ILightEntity> GetLightsById(string id);
        IList<ICameraEntity> GetCamerasById(string id);
        IList<IModelEntity> GetModelsById(string id);

        IList<INode> GetNodesById(string id);
    }
}
