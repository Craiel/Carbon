namespace Core.Processing.Logic
{
    using System;

    public class ContentInfoEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string SourcePath { get; set; }
        public string IntermediatePath { get; set; }
        public string TargetPath { get; set; }

        public string InvariantPath { get; set; }

        public string Hash { get; set; }
        
        public DateTime? SourceLastModified { get; private set; }
        public DateTime? IntermediateLastModified { get; private set; }
        public DateTime? TargetLastModified { get; set; }

        public DateTime LastProcessed { get; set; }
        
        public override bool Equals(object obj)
        {
            var typed = obj as ContentInfoEntry;
            if (typed == null)
            {
                return false;
            }

            return typed.InvariantPath.Equals(this.InvariantPath, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.InvariantPath.ToLowerInvariant().GetHashCode();
        }

        public bool IsNewerThan(ContentInfoEntry other)
        {
            if (this.HasNewerAccessTime(this.SourceLastModified, other.SourceLastModified))
            {
                return true;
            }

            if (this.HasNewerAccessTime(this.IntermediateLastModified, other.IntermediateLastModified))
            {
                return true;
            }

            if (this.HasNewerAccessTime(this.TargetLastModified, other.TargetLastModified))
            {
                return true;
            }

            return false;
        }

        public void InitializeFromSource(string file)
        {
            if (string.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
            {
                throw new ArgumentException();
            }

            this.SourcePath = file;
            this.SourceLastModified = System.IO.File.GetLastWriteTime(file);

            this.IntermediatePath = ContentPathUtilities.GetIntermediateFromSource(this.SourcePath);
            if (System.IO.File.Exists(this.IntermediatePath))
            {
                this.IntermediateLastModified = System.IO.File.GetLastWriteTime(this.IntermediatePath);
            }

            this.TargetPath = ContentPathUtilities.GetTargetFromIntermediate(this.IntermediatePath);
        }

        public void InitializeFromIntermediate(string file)
        {
            if (string.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
            {
                throw new ArgumentException();
            }

            this.IntermediatePath = file;
            this.IntermediateLastModified = System.IO.File.GetLastWriteTime(file);

            this.SourcePath = ContentPathUtilities.GetSourceFromIntermediate(this.IntermediatePath);
            if (System.IO.File.Exists(this.SourcePath))
            {
                this.SourceLastModified = System.IO.File.GetLastWriteTime(this.SourcePath);
            }

            this.TargetPath = ContentPathUtilities.GetTargetFromIntermediate(this.IntermediatePath);
        }

        public void InitializeFromTarget(string file, string root, DateTime lastModified)
        {
            if (string.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
            {
                throw new ArgumentException();
            }

            this.TargetPath = file;
            this.TargetLastModified = lastModified;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool HasNewerAccessTime(DateTime? local, DateTime? other)
        {
            // We have no time but the other one has, so we are probably outdated
            if (local == null && other != null)
            {
                return true;
            }

            if (local == null)
            {
                return true;
            }

            if (local > other)
            {
                return true;
            }

            return false;
        }
    }
}
