namespace Core.Engine.Contracts.UserInterface
{
    using System;
    using System.Collections.Generic;

    public interface IUserInterfaceConsole : IUserInterfaceControl
    {
        event Action<string> OnLineEntered;
        event Func<string, string> OnRequestCompletion;

        int MaxLines { get; set; }
        int MaxCharactersPerLine { get; set; }

        string LineFormat { get; set; }

        IReadOnlyCollection<string> History { get; }
        string Text { get; }

        void SetInputBindings(string name);

        void AddLine(string line);
        void AddSystemLine(string line);
    }
}
