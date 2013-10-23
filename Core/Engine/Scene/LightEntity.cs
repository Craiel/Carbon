namespace Core.Engine.Scene
{
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Rendering;

    public class LightEntity : SceneEntity, ILightEntity
    {
        public ILight Light { get; set; }
        
        public override bool Update(Utils.Contracts.ITimer gameTime)
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

            var instruction = new LightInstruction { Light = this.Light, Position = this.Position, World = this.World };
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
