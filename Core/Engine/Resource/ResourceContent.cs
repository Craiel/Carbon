﻿namespace Core.Engine.Resource
{
    using System.IO;

    using CarbonCore.Processing.Contracts;
    
    public abstract class ResourceContent
    {
        public abstract Stream Load(string hash);
        public abstract bool Store(string hash, ICarbonResource data);
        public abstract bool Replace(string hash, ICarbonResource data);
        public abstract bool Delete(string hash);

        public abstract ResourceInfo GetInfo(string hash);
    }
}
