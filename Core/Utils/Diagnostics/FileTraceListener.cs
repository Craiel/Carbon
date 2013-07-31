using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

using Core.Utils.Contracts;

namespace Core.Utils.Diagnostics
{
    /// <summary>
    /// Taken in large parts from Essential Diagnostics Project
    /// </summary>
    public class FileTraceListener : TraceListener
    {
        private const string DefaultFileNameTemplate = "{AssemblyName}-{DateTime:yyyy-MM-dd}.log";
        private const string DefaultTemplate = "{DateTime:u}\t{Source}({ThreadId})\t{EventType}\t{Id}\t{Message}";

        private readonly IFormatter formatter;
        private readonly ITextFile file;

        private string template = DefaultTemplate;

        private int maxRotation = 10;

        private bool attributesProcessed;
        private bool isRotating;

        private IList<string> traceCache;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileTraceListener()
            : this(DefaultFileNameTemplate)
        {
        }

        public FileTraceListener(string file)
        {
            this.formatter = new Formatter();
            this.traceCache = new List<string>();
            this.file = new TextFile { FileName = this.formatter.Format(file) };
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (!this.attributesProcessed)
            {
                this.ProcessAttributes();
            }
            
            this.formatter.Set("Source", source);
            this.formatter.Set("EventType", eventType.ToString());
            this.formatter.Set("Id", id.ToString(CultureInfo.InvariantCulture));
            if (args != null && args.Length > 0)
            {
                this.formatter.Set("Message", string.Format(format, args));
            }
            else
            {
                this.formatter.Set("Message", format);
            }

            lock (this.traceCache)
            {
                this.traceCache.Add(this.formatter.Format(this.template));
            }

            this.Flush();
        }

        public override void Write(string message)
        {
            this.WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            this.SetFormatDefaults();
            this.formatter.Set("Message", message);
            lock (this.traceCache)
            {
                this.traceCache.Add(this.formatter.Format(this.template));
            }

            this.Flush();
        }

        public override void Flush()
        {
            if (this.isRotating)
            {
                return;
            }

            lock (this.traceCache)
            {
                foreach (string line in this.traceCache)
                {
                    this.file.WriteLine(line);
                }

                this.traceCache.Clear();
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override string[] GetSupportedAttributes()
        {
            return new[] { "template", "Template", "rotateFiles", "RotateFiles", "maxRotation", "MaxRotation" };
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ProcessAttributes()
        {
            lock (this.Attributes)
            {
                this.attributesProcessed = true;

                if (this.Attributes.ContainsKey("template"))
                {
                    this.template = this.Attributes["template"];
                    this.template = this.template.Replace("\\t", "\t"); // Gets escaped so we have to reverse this, can't see a better way atm
                }

                if (this.Attributes.ContainsKey("rotateFiles"))
                {
                    bool rotateFiles = bool.Parse(this.Attributes["rotateFiles"]);
                    if (rotateFiles)
                    {
                        this.RotateFiles();
                    }
                }

                if (this.Attributes.ContainsKey("maxRotation"))
                {
                    this.maxRotation = int.Parse(this.Attributes["maxRotation"]);
                }
            }
        }

        private void SetFormatDefaults()
        {
            this.formatter.Set("Source", "N/A");
            this.formatter.Set("EventType", "N/A");
            this.formatter.Set("Id", "N/A");
            this.formatter.Set("Message", "N/A");
        }

        private void RotateFiles()
        {
            this.isRotating = true;

            lock (this.file)
            {
                this.file.Close();

                if (System.IO.File.Exists(this.file.FileName))
                {
                    string basePath = System.IO.Path.GetDirectoryName(this.file.FileName);
                    string baseName = System.IO.Path.GetFileNameWithoutExtension(this.file.FileName);
                    string baseExtension = System.IO.Path.GetExtension(this.file.FileName);
                    IList<string> files = new List<string>();
                    for (int i = this.maxRotation - 1; i >= 0; i--)
                    {
                        files.Add(string.Format("{0}\\{1}.{2}{3}", basePath, baseName, i, baseExtension));
                    }

                    if (System.IO.File.Exists(files[0]))
                    {
                        System.IO.File.Delete(files[0]);
                    }

                    for (int i = 0; i < files.Count; i++)
                    {
                        if (System.IO.File.Exists(files[i]))
                        {
                            System.IO.File.Move(files[i], files[i - 1]);
                        }
                    }

                    System.IO.File.Move(this.file.FileName, files[files.Count - 1]);
                }
            }

            this.isRotating = false;
        }
    }
}
