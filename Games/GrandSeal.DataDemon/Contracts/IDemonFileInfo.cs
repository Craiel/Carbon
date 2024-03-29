﻿namespace GrandSeal.DataDemon.Contracts
{
    using System.Collections.ObjectModel;
    
    public interface IDemonFileInfo
    {
        ReadOnlyCollection<string> SourceIncludes { get; }
        ReadOnlyCollection<string> IntermediateIncludes { get; }

        int PendingEntries { get; }

        void Refresh();
        
        void AddSourceInclude(string path);
        void AddIntermediateInclude(string path);

        void RemoveSourceInclude(string path);
        void RemoveIntermediateInclude(string path);

        void Clear();
    }
}
