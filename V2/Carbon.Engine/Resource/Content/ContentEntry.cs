using System;
using System.Collections.Generic;

using Carbon.Engine.Contracts.Resource;

using Core.Utils;

namespace Carbon.Engine.Resource.Content
{
    public abstract class ContentEntry : ICarbonContent
    {
        private int changeState;

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

        public virtual ICarbonContent Clone(bool fullCopy = false)
        {
            throw new NotImplementedException("Clone is not implemented for " + this.GetType());
        }

        public virtual void LoadFrom(ICarbonContent source)
        {
            throw new NotImplementedException("LoadFrom is not implemented for " + this.GetType());
        }

        public void LockChangeState()
        {
            this.changeState = this.GetState();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private int GetState()
        {
            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(this.GetType());
            int[] hashes = new int[properties.Count];
            for (int i = 0; i < properties.Count; i++)
            {
                var value = properties[i].Info.GetValue(this);
                if (value == null)
                {
                    continue;
                }

                hashes[i] = value.GetHashCode();
            }

            return HashUtils.CombineHashes(hashes);
        }
    }
}
