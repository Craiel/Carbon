﻿namespace Core.Engine.Resource
{
    using System;

    using Core.Utils;

    public class ContentCriterion
    {
        public ContentReflectionProperty PropertyInfo { get; set; }
        public CriterionType Type { get; set; }
        public object[] Values { get; set; }
        public bool Negate { get; set; }

        public override int GetHashCode()
        {
            return Tuple.Create(this.PropertyInfo, this.Type, HashUtils.CombineObjectHashes(this.Values), this.Negate).GetHashCode();
        }
    }
}
