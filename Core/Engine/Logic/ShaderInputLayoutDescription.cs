using System;

using Core.Utils;

namespace Core.Engine.Logic
{
    public class ShaderInputLayoutDescription
    {
        public ShaderInputLayoutDescription(CarbonShaderDescription description)
        {
            this.Description = description;
        }

        public Type Type { get; set; }
        public CarbonShaderDescription Description { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as ShaderInputLayoutDescription;
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
