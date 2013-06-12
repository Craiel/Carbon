using System;
using System.IO;

using Core.Engine.Resource.Resources;

namespace Core.Editor.Processors
{
    using System.Text.RegularExpressions;

    using Core.Utils;

    public struct UserInterfaceProcessingOptions
    {
        public ScriptProcessingOptions ScriptOptions;
    }

    public class UserInterfaceProcessor
    {
        private static readonly Regex CsamlFieldRegex = new Regex("{([a-z]+)[\\s]*([^\"]*)}", RegexOptions.IgnoreCase);

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
                    string data = reader.ReadToEnd();
                    data = CsamlFieldRegex.Replace(data, CsamlFieldEvaluator);
                    resource.CsamlData = data;
                }
            }

            // Read the Script
            resource.Script = ScriptProcessor.Process(scriptPath, options.ScriptOptions);

            return resource;
        }

        private static string CsamlFieldEvaluator(Match match)
        {
            if (match.Captures.Count <= 0 || match.Groups.Count < 2)
            {
                System.Diagnostics.Trace.TraceWarning("Could not evaluate Resource, no capture data");
                return "ERROR";
            }

            string fieldId = match.Groups[1].Value.ToLower();
            string fieldValue = match.Groups.Count > 2 ? match.Groups[2].Value : null;
            switch (fieldId)
            {
                case "resource":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            System.Diagnostics.Trace.TraceWarning("Argument missing in resource Field");
                            return "ERROR";
                        }

                        return HashUtils.BuildResourceHash(fieldValue);
                    }
            }

            System.Diagnostics.Trace.TraceWarning("Unknown Field in Script: " + match.Captures[0].Value);
            return "ERROR";
        }
    }
}
