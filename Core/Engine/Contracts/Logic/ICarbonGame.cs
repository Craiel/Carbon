namespace Core.Engine.Contracts.Logic
{
    public interface ICarbonGame
    {
        float FramesPerSecond { get; }
        float FramesSkipped { get; }

        float GameSpeed { get; set; }

        int TargetFrameRate { get; set; }

        bool LimitFrameRate { get; set; }

        void Run();

        void ClearCache();
    }
}
