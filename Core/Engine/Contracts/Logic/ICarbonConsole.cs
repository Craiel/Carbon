namespace Core.Engine.Contracts.Logic
{
    using Core.Engine.Contracts.Rendering;

    using SlimDX;

    public interface ICarbonConsole : IEngineComponent, IRenderable
    {
        bool EnableTimeStamp { get; set; }
        bool IsEnabled { get; set; }
        bool IsVisible { get; set; }

        int MaxLines { get; set; }

        Vector4 BackgroundColor { get; set; }

        string Text { get; }

        void Write(string text);
        void WriteLine(string line);
    }
}
