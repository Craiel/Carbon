namespace Core.Utils
{
    using System;
    using System.IO;

    using Core.Utils.Contracts;

    public class TextFile : ITextFile
    {
        private readonly object fileLock;

        private string fileName;
        private TextWriter writer;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TextFile()
        {
            this.fileLock = new object();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string FileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                if (this.fileName == null || !this.fileName.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    this.Close();
                    this.fileName = value;
                }
            }
        }

        public void Dispose()
        {
            lock (this.fileLock)
            {
                this.Close();
            }

            GC.SuppressFinalize(this);
        }

        public void Write(string value)
        {
            lock (this.fileLock)
            {
                this.OpenIfNeeded();
                if (this.writer != null)
                {
                    this.writer.Write(value);
                    this.writer.Flush();
                }
            }
        }

        public void WriteLine(string line)
        {
            lock (this.fileLock)
            {
                this.OpenIfNeeded();
                if (this.writer != null)
                {
                    this.writer.WriteLine(line);
                    this.writer.Flush();
                }
            }
        }

        public void Clear()
        {
            lock (this.fileLock)
            {
                this.Close();
                if (File.Exists(this.fileName))
                {
                    File.Delete(this.fileName);
                }
            }
        }

        public void Close()
        {
            lock (this.fileLock)
            {
                if (this.writer != null)
                {
                    this.writer.Flush();
                    this.writer.Close();
                    this.writer.Dispose();
                }

                this.writer = null;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OpenIfNeeded()
        {
            if (this.writer != null)
            {
                return;
            }

            try
            {
                string directory = Path.GetDirectoryName(this.FileName);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                FileStream stream = File.Open(this.fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                this.writer = new StreamWriter(stream);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("Failed to open Log File: {0}\n{1}", this.fileName, e.Message);
            }
        }
    }
}
