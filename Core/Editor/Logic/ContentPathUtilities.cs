namespace Core.Processing.Logic
{
    using System;
    using System.Text.RegularExpressions;

    public static class ContentPathUtilities
    {
        private static readonly Regex ModelRootCheck =
            new Regex(string.Format(@"\{0}{1}\{0}", System.IO.Path.DirectorySeparatorChar, ContentSettings.RootModel), RegexOptions.IgnoreCase);

        private static readonly Regex StageRootCheck =
            new Regex(string.Format(@"\{0}{1}\{0}", System.IO.Path.DirectorySeparatorChar, ContentSettings.RootStage), RegexOptions.IgnoreCase);

        private static readonly Regex TextureRootCheck =
            new Regex(string.Format(@"\{0}{1}\{0}", System.IO.Path.DirectorySeparatorChar, ContentSettings.RootTexture), RegexOptions.IgnoreCase);

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IsInModelRoot(string path)
        {
            return ModelRootCheck.IsMatch(path);
        }

        public static bool IsInStageRoot(string path)
        {
            return StageRootCheck.IsMatch(path);
        }

        public static bool IsInTextureRoot(string path)
        {
            return TextureRootCheck.IsMatch(path);
        }

        public static string GetSourceFromIntermediate(string path)
        {
            if (!path.Contains(ContentSettings.ExtensionIntermediateFolder))
            {
                throw new ArgumentException("Path is not within a valid Intermediate directory: " + path);
            }

            string sourcePath = path.Replace(ContentSettings.ExtensionIntermediateFolder, ContentSettings.ExtensionSourceFolder);
            string extension = System.IO.Path.GetExtension(sourcePath);
            if (string.IsNullOrEmpty(extension))
            {
                System.Diagnostics.Trace.TraceWarning("Intermediate -> Source: File has no extension, returning as is!");
                return sourcePath;
            }

            // Now we have to do contextual check's
            if (IsInModelRoot(sourcePath))
            {
                if (extension.Equals(ContentSettings.ExtensionCollada, StringComparison.OrdinalIgnoreCase))
                {
                    sourcePath = sourcePath.Replace(ContentSettings.ExtensionCollada, ContentSettings.ExtensionBlender);
                }
            }
            else if (IsInStageRoot(sourcePath))
            {
                if (extension.Equals(ContentSettings.ExtensionStage, StringComparison.OrdinalIgnoreCase))
                {
                    sourcePath = sourcePath.Replace(ContentSettings.ExtensionStage, ContentSettings.ExtensionBlender);
                }
            }
            else if (IsInTextureRoot(sourcePath))
            {
                if (extension.Equals(ContentSettings.ExtensionPng, StringComparison.OrdinalIgnoreCase))
                {
                    sourcePath = sourcePath.Replace(ContentSettings.ExtensionPng, ContentSettings.ExtensionGimp);
                }
            }

            return sourcePath;
        }

        public static string GetIntermediateFromSource(string path)
        {
            if (!path.Contains(ContentSettings.ExtensionSourceFolder))
            {
                throw new ArgumentException("Path is not within a valid Source directory: " + path);
            }

            string sourcePath = path.Replace(ContentSettings.ExtensionSourceFolder, ContentSettings.ExtensionIntermediateFolder);
            string extension = System.IO.Path.GetExtension(sourcePath);
            if (string.IsNullOrEmpty(extension))
            {
                System.Diagnostics.Trace.TraceWarning("Source -> Intermediate: File has no extension, returning as is!");
                return sourcePath;
            }

            // Now we have to do contextual check's
            if (IsInModelRoot(sourcePath))
            {
                if (extension.Equals(ContentSettings.ExtensionBlender, StringComparison.OrdinalIgnoreCase))
                {
                    sourcePath = sourcePath.Replace(ContentSettings.ExtensionBlender, ContentSettings.ExtensionCollada);
                }
            }
            else if (IsInStageRoot(sourcePath))
            {
                if (extension.Equals(ContentSettings.ExtensionBlender, StringComparison.OrdinalIgnoreCase))
                {
                    sourcePath = sourcePath.Replace(ContentSettings.ExtensionBlender, ContentSettings.ExtensionStage);
                }
            }
            else if (IsInTextureRoot(sourcePath))
            {
                if (extension.Equals(ContentSettings.ExtensionGimp, StringComparison.OrdinalIgnoreCase))
                {
                    sourcePath = sourcePath.Replace(ContentSettings.ExtensionGimp, ContentSettings.ExtensionPng);
                }
            }

            return sourcePath;
        }
        
        public static string GetTargetFromIntermediate(string path)
        {
            int indexOfFolder = path.IndexOf(ContentSettings.ExtensionIntermediateFolder, StringComparison.OrdinalIgnoreCase);
            if (indexOfFolder < 0)
            {
                throw new ArgumentException("Path is not a valid Intermediate directory: " + path);
            }

            string target = path.Substring(indexOfFolder + ContentSettings.ExtensionIntermediateFolder.Length, path.Length - indexOfFolder - ContentSettings.ExtensionIntermediateFolder.Length);
            return target.TrimStart(System.IO.Path.DirectorySeparatorChar);
        }
    }
}
