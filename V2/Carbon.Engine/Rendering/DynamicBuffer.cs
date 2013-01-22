using System;

using SlimDX;
using SlimDX.Direct3D11;

using Buffer = SlimDX.Direct3D11.Buffer;

namespace Carbon.Engine.Rendering
{
    public class DynamicBuffer : IDisposable
    {
        private readonly Device device;

        private BufferDescription description;
        private Buffer buffer;

        private DataBox box;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DynamicBuffer(Device device, BindFlags bindFlags)
        {
            this.device = device;
            this.description = new BufferDescription(4096, ResourceUsage.Dynamic, bindFlags, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            this.Resize();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public long Size
        {
            get
            {
                return this.description.SizeInBytes;
            }

            set
            {
                if (this.description.SizeInBytes != value)
                {
                    this.description.SizeInBytes = (int)value;
                    this.Resize();
                }
            }
        }

        public Buffer Buffer
        {
            get
            {
                return this.buffer;
            }
        }

        public void BeginUpdate(out DataStream stream, long dataSize)
        {
            if (dataSize > this.Size)
            {
                this.Size = dataSize;
            }

            this.box = this.device.ImmediateContext.MapSubresource(this.buffer, MapMode.WriteDiscard, MapFlags.None);
            stream = this.box.Data;
        }

        public void EndUpdate()
        {
            // Wind the stream back to beginning
            this.box.Data.Position = 0;
            this.device.ImmediateContext.UnmapSubresource(this.buffer, 0);
        }

        public void Dispose()
        {
            if (this.buffer != null)
            {
                this.buffer.Dispose();
            }
        }

        private void Resize()
        {
            if(this.buffer != null)
            {
                this.buffer.Dispose();
            }

            this.buffer = new Buffer(this.device, this.description);
        }
    }
}
