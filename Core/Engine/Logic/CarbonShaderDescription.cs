﻿namespace Core.Engine.Logic
{
    using System.IO;

    using Core.Utils;

    using SlimDX.D3DCompiler;

    public sealed class CarbonShaderDescription
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string File { get; set; }
        public string Entry { get; set; }
        public string Profile { get; set; }

        public ShaderFlags ShaderFlags { get; set; }
        public EffectFlags EffectFlags { get; set; }

        public ShaderMacro[] Macros { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as CarbonShaderDescription;
            if (other == null)
            {
                return false;
            }

            return this.File.Equals(other.File) &&
                this.Entry.Equals(other.Entry) &&
                this.Profile.Equals(other.Profile) &&
                this.ShaderFlags.Equals(other.ShaderFlags) &&
                this.EffectFlags.Equals(other.EffectFlags) &&
                this.Macros.Equals(other.Macros);
        }

        public override int GetHashCode()
        {
            return
                HashUtils.CombineObjectHashes(
                    new object[]
                        {
                            this.File, 
                            this.Entry, 
                            this.Profile,
                            this.ShaderFlags, 
                            this.EffectFlags,
                            HashUtils.CombineHashes(this.Macros)
                        });
        }

        public string GetCacheFileName()
        {
            string fileName = Path.GetFileNameWithoutExtension(this.File);
            return string.Format(
                "{0}{1}_{2}{3:d}{4:d}{5}.fxc",
                fileName,
                this.Entry,
                this.Profile,
                this.ShaderFlags,
                this.EffectFlags,
                HashUtils.GetFNVFileName(HashUtils.CombineHashes(this.Macros)));
        }
    }
}
