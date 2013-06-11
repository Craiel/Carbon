using System;
using System.IO;

using Core.Engine.Resource.Resources;

namespace Core.Editor.Processors
{
    public struct ScriptProcessingOptions
    {
    }

    public class ScriptProcessor
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ScriptResource Process(string path, ScriptProcessingOptions options)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                throw new ArgumentException("Invalid Script Processing options");
            }

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    string scriptData = reader.ReadToEnd();

                    // Todo: Process includes and References

                    return new ScriptResource { Script = scriptData };
                }
            }
        }
    }
}
