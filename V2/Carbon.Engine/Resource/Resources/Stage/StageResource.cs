using System.IO;

using Carbon.Engine.Logic;

namespace Carbon.Engine.Resource.Resources.Stage
{
    public class StageResource : ResourceBase
    {
        internal enum MeshFlags
        {
            None = 0,
            HasCameras = 1,
            HasLights = 2,
            HasModels = 4
        }

        internal const int Version = 1;

        public StageCameraElement[] Cameras { get; set; }
        public StageLightElement[] Lights { get; set; }
        public StageModelElement[] Models { get; set; }

        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            int version = source.ReadInt();
            if (version != Version)
            {
                throw new InvalidDataException("Stage version is not correct: " + version);
            }

            uint flags = source.ReadUInt();
            bool hasCameras = (flags & (int)MeshFlags.HasCameras) == (int)MeshFlags.HasCameras;
            bool hasLights = (flags & (int)MeshFlags.HasLights) == (int)MeshFlags.HasLights;
            bool hasModels = (flags & (int)MeshFlags.HasModels) == (int)MeshFlags.HasModels;

            if (hasCameras)
            {
                short count = source.ReadShort();
                this.Cameras = new StageCameraElement[count];
                for (int i = 0; i < count; i++)
                {
                    this.Cameras[i] = new StageCameraElement();
                    this.Cameras[i].Load(source);
                }
            }

            if (hasLights)
            {
                short count = source.ReadShort();
                this.Lights = new StageLightElement[count];
                for (int i = 0; i < count; i++)
                {
                    this.Lights[i] = new StageLightElement();
                    this.Lights[i].Load(source);
                }
            }

            if (hasModels)
            {
                int count = source.ReadInt();
                this.Models = new StageModelElement[count];
                for (int i = 0; i < count; i++)
                {
                    this.Models[i] = new StageModelElement();
                    this.Models[i].Load(source);
                }
            }
        }

        private uint GetStageFlags()
        {
            uint flags = 0;
            if (this.Cameras != null && this.Cameras.Length > 0)
            {
                flags |= (uint)MeshFlags.HasCameras;
            }

            if (this.Lights != null && this.Lights.Length > 0)
            {
                flags |= (uint)MeshFlags.HasLights;
            }

            if (this.Models != null && this.Models.Length > 0)
            {
                flags |= (uint)MeshFlags.HasModels;
            }

            return flags;
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(Version);
            uint flags = this.GetStageFlags();
            bool hasCameras = (flags & (int)MeshFlags.HasCameras) == (int)MeshFlags.HasCameras;
            bool hasLights = (flags & (int)MeshFlags.HasLights) == (int)MeshFlags.HasLights;
            bool hasModels = (flags & (int)MeshFlags.HasModels) == (int)MeshFlags.HasModels;
            target.Write(flags);
            
            if (hasCameras != null)
            {
                target.Write((short)this.Cameras.Length);
                for (int i = 0; i < this.Cameras.Length; i++)
                {
                    this.Cameras[i].Save(target);
                }
            }

            if (hasLights != null)
            {
                target.Write((short)this.Lights.Length);
                for (int i = 0; i < this.Lights.Length; i++)
                {
                    this.Lights[i].Save(target);
                }
            }

            if (hasModels != null)
            {
                target.Write(this.Models.Length);
                for (int i = 0; i < this.Models.Length; i++)
                {
                    this.Models[i].Save(target);
                }
            }
        }
    }
}
