using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic;
using Core.Utils.Contracts;
using SlimDX.DirectInput;

namespace Carbon.V2Test.Logic
{
    public interface ITileController : IEngineComponent, IKeyStateReceiver
    {
        ITile ControlledTile { get; set; }
    }

    public class TileController : EngineComponent, ITileController
    {
        private readonly IKeyStateManager keyStateManager;

        private TileMovement movement;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TileController(IKeyStateManager keyStateManager)
        {
            this.keyStateManager = keyStateManager;
            this.keyStateManager.RegisterReceiver(this);
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

        public ITile ControlledTile { get; set; }
        
        public void ReceivePersists(Key key, ref bool isHandled)
        {
        }

        public void ReceivePressed(Key key, ref bool isHandled)
        {
            switch (key)
            {
                case Key.Q:
                    this.ControlledTile.MoveLeft();
                    break;
                case Key.E:
                    this.ControlledTile.MoveRight();
                    break;
                case Key.LeftArrow:
                    this.ControlledTile.RotateLeft();
                    break;
                case Key.DownArrow:
                    this.ControlledTile.MoveDown();
                    break;
                case Key.RightArrow:
                    this.ControlledTile.RotateRight();
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
    }
}
