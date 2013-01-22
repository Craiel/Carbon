using System;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

using Device = SlimDX.Direct3D11.Device;

namespace Carbon.Engine.Rendering
{
    public abstract class CarbonDeviceContext
    {
        protected CarbonDeviceContext(IntPtr targetHandle)
        {
            if (targetHandle == IntPtr.Zero)
            {
                throw new ArgumentException("Target handle for Device Context can not be null!", "targetHandle");
            }

            this.TargetHandle = targetHandle;
        }

        public abstract Device Device { get; }

        protected IntPtr TargetHandle { get; private set; }
        
        public abstract void Present(PresentFlags flags);
    }

    public class CarbonDeviceContextDx11 : CarbonDeviceContext, IDisposable
    {
        private readonly DeviceSettings settings;

        private Factory factory;
        private SwapChain swapChain;
        private Device device;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonDeviceContextDx11(IntPtr targetHandle, DeviceSettings settings)
            : base(targetHandle)
        {
            if (settings == null)
            {
                throw new ArgumentException("Settings for Device Context can not be null!", "settings");
            }

            this.settings = settings;

            this.Initialize();
        }

        ~CarbonDeviceContextDx11()
        {
            this.Dispose(false);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Factory Factory
        {
            get
            {
                return this.factory;
            }
        }

        public SwapChain SwapChain
        {
            get
            {
                return this.swapChain;
            }
        }

        public override Device Device
        {
            get
            {
                return this.device;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override void Present(PresentFlags flags)
        {
            this.swapChain.Present(0, flags);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Initialize()
        {
            this.factory = new Factory();

            var description = new SwapChainDescription
            {
                BufferCount = 1,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = this.TargetHandle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(this.settings.Width, this.settings.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };

            Device.CreateWithSwapChain(DriverType.Hardware, this.settings.CreationFlags, description, out this.device, out this.swapChain);

            this.factory.SetWindowAssociation(this.TargetHandle, WindowAssociationFlags.IgnoreAll | WindowAssociationFlags.IgnoreAltEnter);
        }

        private void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                if (this.swapChain.IsFullScreen)
                {
                    this.swapChain.IsFullScreen = false;
                }

                this.swapChain.Dispose();
                this.device.Dispose();
                this.factory.Dispose();
            }
        }
    }
}
