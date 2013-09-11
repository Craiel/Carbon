namespace Core.Engine.Scene
{
    using System;

    using Core.Engine.Contracts.Scene;

    public enum EntityDebugType
    {
        Unknown,
        Model,
        Light,
        Camera
    }

    public class SceneEntityDebugEntry
    {
        public SceneEntityDebugEntry(string name, EntityDebugType type, WeakReference<ISceneEntity> source = null)
        {
            this.Name = name;
            this.Type = type;
            this.Source = source;
        }

        public string Name { get; private set; }
        public EntityDebugType Type { get; private set; }
        
        public WeakReference<ISceneEntity> Source { get; private set; }
    }
}
