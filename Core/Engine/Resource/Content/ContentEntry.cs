using System;
using System.Collections.Generic;

using Core.Engine.Contracts.Resource;

using Core.Utils;

namespace Core.Engine.Resource.Content
{
    public abstract class ContentEntry : ICarbonContent
    {
        private int changeState;

        protected ContentEntry()
        {
            this.IsValid = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract bool IsNew { get; }

        public bool IsChanged
        {
            get
            {
                return this.GetState() != this.changeState;
            }
        }

        public abstract MetaDataTargetEnum MetaDataTarget { get; }
        
        public bool IsValid { get; private set; }

        public virtual ICarbonContent Clone(bool fullCopy = false)
        {
            throw new InvalidOperationException("Clone is not implemented for " + this.GetType());
        }

        public virtual void LoadFrom(ICarbonContent source)
        {
            throw new InvalidOperationException("LoadFrom is not implemented for " + this.GetType());
        }

        public void LockChangeState()
        {
            this.changeState = this.GetState();
        }

        public void Invalidate()
        {
            this.IsValid = false;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private int GetState()
        {
            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(this.GetType());
            var hashes = new int[properties.Count];
            for (int i = 0; i < properties.Count; i++)
            {
                var value = properties[i].Info.GetValue(this);
                if (value == null)
                {
                    continue;
                }

                if (value as ContentEntry != null)
                {
                    hashes[i] = ((ContentEntry)value).GetState();
                    continue;
                }

                hashes[i] = value.GetHashCode();
            }

            return HashUtils.CombineHashes(hashes);
        }
    }
}
