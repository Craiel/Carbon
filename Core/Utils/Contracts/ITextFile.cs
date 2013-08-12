namespace Core.Utils.Contracts
{
    using System;

    public interface ITextFile : IDisposable
    {
        string FileName { get; set; }

        void Write(string value);
        void WriteLine(string line);
        void Clear();
        void Close();
    }
}
