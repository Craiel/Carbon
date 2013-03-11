namespace Carbed.Contracts
{
    public interface ICarbedSettings
    {
        string TextureToolsFolder { get; set; }

        void Save();
        void Load();
        void Reset();
    }
}
