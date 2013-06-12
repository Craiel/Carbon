using System;
using System.IO;

using Core.Engine.Resource.Resources;

namespace Core.Editor.Processors
{
    using System.Text.RegularExpressions;

    using Core.Utils;

    public delegate string ResolveIncludeDelegate(string include);
    public struct ScriptProcessingOptions
    {
        public ResolveIncludeDelegate IncludeResolver;
    }

    public class ScriptProcessor
    {
        private static readonly Regex ScriptFieldRegex = new Regex("{([a-z]+)[\\s]*([^\"]*)}", RegexOptions.IgnoreCase);

        private static ScriptProcessingOptions? currentOptions;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ScriptResource Process(string path, ScriptProcessingOptions options)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                throw new ArgumentException("Invalid Script Processing options");
            }

            currentOptions = options;
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string scriptData = reader.ReadToEnd();

                        scriptData = ScriptFieldRegex.Replace(scriptData, FieldEvaluator);

                        return new ScriptResource { Script = scriptData };
                    }
                }
            }
            finally
            {
                currentOptions = null;
            }
        }

        private static string FieldEvaluator(Match match)
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

                case "include":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            System.Diagnostics.Trace.TraceWarning("Argument missing in include Field");
                            return "ERROR";
                        }

                        if (currentOptions == null || currentOptions.Value.IncludeResolver == null)
                        {
                            System.Diagnostics.Trace.TraceWarning("Include Resolver not present, can not resolve include field");
                            return "ERROR";
                        }

                        return currentOptions.Value.IncludeResolver(fieldValue);
                    }
            }

            System.Diagnostics.Trace.TraceWarning("Unknown Field in Script: " + match.Captures[0].Value);
            return "ERROR";
        }
    }
}
