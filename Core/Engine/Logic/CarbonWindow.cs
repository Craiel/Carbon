using SlimDX.Windows;
using SlimDX;

namespace Core.Engine.Logic
{
    public class CarbonWindow : RenderForm
    {
        protected override void OnResizeEnd(System.EventArgs e)
        {
            base.OnResizeEnd(e);

            this.Center = new Vector2(this.Width / 2, this.Height / 2);
        }

        public Vector2 Center { get; private set; }
    }
}
