namespace Core.Engine.Contracts.Scene
{
    using System;
    using System.Collections.Generic;

    public interface ISceneSpatialStructure : IDisposable
    {
        IReadOnlyCollection<ILightEntity> GetLights();
        IReadOnlyCollection<ICameraEntity> GetCameras();
        IReadOnlyCollection<IModelEntity> GetModels();

        ILightEntity GetLightById(string id);
        ICameraEntity GetCameraById(string id);
        IModelEntity GetModelById(string id);
        INode GetNodeById(string id);
    }
}
