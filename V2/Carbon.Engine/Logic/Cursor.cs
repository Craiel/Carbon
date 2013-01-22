using Carbon.Engine.Contracts.Logic;
using Core.Utils.Contracts;
using SlimDX;
using SlimDX.DirectInput;

namespace Carbon.Engine.Logic
{
    public delegate void ButtonChangeDelegate(int button, bool pressed);

    public interface ICursor : IEngineComponent
    {
        event ButtonChangeDelegate ButtonChanged;

        Vector2 Position { get; set; }
        Vector2 LastDelta { get; }
        Vector2 MinPosition { get; set; }
        Vector2 MaxPosition { get; set; }

        float WheelPosition { get; }

        bool Visible { get; }

        void Show();
        void Hide();

        bool IsPressed(int button);
    }

    public class Cursor : EngineComponent, ICursor
    {
        private readonly DirectInput directInput;
        private readonly Mouse mouse;

        private float wheelPosition;

        private bool[] previousButtonState;

        private bool visible;

        private Vector2 position;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Cursor()
        {
            this.directInput = new DirectInput();
            this.mouse = new Mouse(directInput);
            this.mouse.Acquire();
        }

        public override void Dispose()
        {
            base.Dispose();

            this.directInput.Dispose();
            this.mouse.Dispose();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event ButtonChangeDelegate ButtonChanged;

        public Vector2 Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.position = value;
            }
        }

        public Vector2 LastDelta { get; private set; }

        public Vector2 MinPosition { get; set; }
        public Vector2 MaxPosition { get; set; }

        public bool Visible
        {
            get
            {
                return this.visible;
            }
        }
        
        public float WheelPosition
        {
            get
            {
                return this.wheelPosition;
            }
        }

        public override void Update(ITimer gameTime)
        {
            MouseState state = this.mouse.GetCurrentState();
            this.LastDelta = new Vector2(state.X, state.Y);
            this.position += this.LastDelta;
            this.wheelPosition += state.Z;

            // Keep the position within the current boundaries
            this.position = Vector2.Clamp(this.position, this.MinPosition, this.MaxPosition);
            
            bool[] buttonStates = state.GetButtons();

            // This probably never occurs but just in case
            if ((this.previousButtonState != null) && (this.previousButtonState.Length == buttonStates.Length))
            {
                // check if any new ones are pressed
                for (int i = 0; i < buttonStates.Length; i++)
                {
                    // State persists
                    if (this.previousButtonState[i] == buttonStates[i])
                    {
                        continue;
                    }

                    this.OnButtonStateChange(i);
                }
            }

            this.previousButtonState = buttonStates;
        }

        public void Show()
        {
            this.visible = true;
        }

        public void Hide()
        {
            this.visible = false;
        }

        public bool IsPressed(int button)
        {
            if (this.previousButtonState.Length > button)
            {
                return this.previousButtonState[button];
            }

            return false;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnButtonStateChange(int button)
        {
            if (this.ButtonChanged != null)
            {
                this.ButtonChanged(button, !this.previousButtonState[button]);
            }
        }        
    }
}
