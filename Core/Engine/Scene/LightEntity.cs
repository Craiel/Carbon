﻿namespace Core.Engine.Scene
{
    using CarbonCore.Utils.Compat.Contracts;

    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Rendering;

    using SharpDX;

    public class LightEntity : SceneEntity, ILightEntity
    {
        public ILight Light { get; set; }

        public override bool CanRender
        {
            get
            {
                return true;
            }
        }

        public override bool Update(ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            if (this.Light != null)
            {
                // Set the position and update the game-time, position is used mainly for light perspective updates
                this.Light.Position = this.Position;
                this.Light.Update(gameTime);
            }

            return true;
        }

        public override void Render(FrameInstructionSet frameSet)
        {
            base.Render(frameSet);

            if (this.Light == null)
            {
                return;
            }

            Matrix world = this.OverrideWorld ?? this.GetWorld();
            var instruction = new LightInstruction { Light = this.Light, Position = this.Position, World = world };
            frameSet.LightInstructions.Add(instruction);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override ISceneEntity DoClone()
        {
            return new LightEntity { Light = this.Light };
        }
    }
}
