using System;

using Core.Utils;

namespace Carbon.Engine.Logic
{
    public class InputLayoutDescription
    {
        public InputLayoutDescription(CarbonShaderDescription description)
        {
            this.Description = description;
        }

        public Type Type { get; set; }
        public CarbonShaderDescription Description { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as InputLayoutDescription;
            if (other == null)
            {
                return false;
            }

            return this.Type == other.Type && this.Description.Equals(other.Description);
        }

        public override int GetHashCode()
        {
            return HashUtils.CombineHashes(new[] { this.Type.GetHashCode(), this.Description.GetHashCode() });
        }
    }
}
