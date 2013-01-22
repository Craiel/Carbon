using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;
using Core.Utils.Contracts;
using SlimDX.DirectInput;

namespace Carbon.Engine.Logic
{
    public interface IDebugController : IEngineComponent, IKeyStateReceiver
    {
    }

    public class DebugController : EngineComponent, IDebugController
    {
        private readonly IKeyStateManager keyStateManager;
        private readonly ICursor cursor;
        private readonly ICarbonGraphics graphics;
        private readonly IFrameManager frameManager;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DebugController(ICursor cursor, IKeyStateManager keyStateManager, ICarbonGraphics graphics, IFrameManager frameManager)
        {
            this.frameManager = frameManager;

            this.keyStateManager = keyStateManager;
            this.keyStateManager.RegisterReceiver(this);

            this.cursor = cursor;
            this.cursor.ButtonChanged += this.OnButtonChanged;

            this.graphics = graphics;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Order
        {
            get
            {
                return 100;
            }
        }
        
        public void ReceivePersists(Key key, ref bool isHandled)
        {
        }

        public void ReceivePressed(Key key, ref bool isHandled)
        {
            switch (key)
            {
                case Key.F3:
                    this.frameManager.EnableDebugOverlay = !this.frameManager.EnableDebugOverlay;
                    break;
                case Key.F4:
                    //this.renderer.FrameStatistics.Trace();
                    break;

                case Key.F9:
                    this.graphics.DisableDepth();
                    break;
                case Key.F10:
                    this.graphics.EnableDepth();
                    break;
                
                case Key.F12:
                    this.graphics.EnableWireframe();
                    break;
                case Key.F11:
                    this.graphics.DisableWireframe();
                    break;
            }
        }

        public void ReceiveReleased(Key key, ref bool isHandled)
        {
        }

        public override void Update(ITimer gameTime)
        {
        }

        public override void Dispose()
        {
            this.keyStateManager.UnregisterReceiver(this);

            base.Dispose();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnButtonChanged(int button, bool pressed)
        {
        }
    }
}
