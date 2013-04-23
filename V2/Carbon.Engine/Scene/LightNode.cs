using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Rendering;

namespace Carbon.Engine.Scene
{
    public interface ILightNode : INode
    {
    }

    public class LightNode : Node, ILightNode
    {
        public ILight Light { get; set; }

        public override void Dispose()
        {
            if (this.Light != null)
            {
                this.Light.Dispose();
            }

            base.Dispose();
        }

        public override void Update(Core.Utils.Contracts.ITimer gameTime)
        {
            base.Update(gameTime);

            if (this.Light != null)
            {
                // Set the position and update the game-time, position is used mainly for light perspective updates
                this.Light.Position = this.Position;
                this.Light.Update(gameTime);
            }
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
    }
}
