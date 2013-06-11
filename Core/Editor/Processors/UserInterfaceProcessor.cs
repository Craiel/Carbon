using System;
using System.IO;

using Core.Engine.Resource.Resources;

namespace Core.Editor.Processors
{
    public struct UserInterfaceProcessingOptions
    {
        public ScriptProcessingOptions ScriptOptions;
    }

    public class UserInterfaceProcessor
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static UserInterfaceResource Process(string path, UserInterfaceProcessingOptions options)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                throw new ArgumentException("Invalid Script Processing options");
            }

            string scriptPath = path + ".lua";
            if (!File.Exists(scriptPath))
            {
                throw new InvalidDataException("Script file was not found for User Interface");
            }

            var resource = new UserInterfaceResource();

            // Read the Csaml
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    resource.CsamlData = reader.ReadToEnd();
                }
            }

            // Read the Script
            resource.Script = ScriptProcessor.Process(scriptPath, options.ScriptOptions);

            return resource;
        }
    }
}
