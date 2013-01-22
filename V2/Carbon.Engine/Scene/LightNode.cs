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

        public override void Update(Core.Utils.Contracts.ITimer gameTime)
        {
            base.Update(gameTime);

            if (this.Light != null)
            {
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
