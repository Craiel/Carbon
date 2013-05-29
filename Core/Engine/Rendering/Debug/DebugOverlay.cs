using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Engine.Contracts.Logic;
using Core.Engine.Contracts.Rendering;
using Core.Engine.Logic;
using Core.Engine.Rendering.Camera;

namespace Core.Engine.Rendering.Debug
{
    public class DebugOverlay : IDisposable
    {
        private readonly CoordinateDisplay coordinateDisplay;
        private readonly OrientationDisplay orientationDisplay;

        public DebugOverlay()
        {
            this.coordinateDisplay = new CoordinateDisplay();
            this.orientationDisplay = new OrientationDisplay();
        }

        public ICamera Camera { get; set; }

        public void Dispose()
        {
            this.coordinateDisplay.Dispose();
            this.orientationDisplay.Dispose();
        }

        /*public void Render(IRenderer renderer, IList<RenderInstruction> instructions)
        {
            //this.coordinateDisplay.WorldRotation = this.Camera.Rotation;

            this.coordinateDisplay.Render(renderer);

            foreach (RenderInstruction instruction in instructions)
            {
                this.orientationDisplay.Render(renderer, instruction.Position, instruction.Rotation);
            }
        }*/
    }
}
