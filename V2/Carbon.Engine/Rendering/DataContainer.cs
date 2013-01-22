using System;
using System.Runtime.InteropServices;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    internal abstract class DataContainer
    {
        public abstract long Size { get; }

        public abstract void Add(object data);
        public abstract void WriteData(DataStream target);
        public abstract void Clear();
    }

    internal class StaticDataContainer<T> : DataContainer
         where T : struct
    {
        private const int IncreaseSize = 1024;
        private readonly int typeSize;
        private T[] elements;
        private int position;
        
        public StaticDataContainer()
        {
            this.elements = new T[IncreaseSize];
            this.typeSize = Marshal.SizeOf(typeof(T));
        }

        public override long Size
        {
            get
            {
                return this.typeSize * this.position;
            }
        }

        public T this[int index]
        {
            get
            {
                return elements[index];
            }
        }

        public override void Add(object element)
        {
            if (this.position >= this.elements.Length)
            {
                Array.Resize(ref this.elements, this.elements.Length + IncreaseSize);
            }

            this.elements[this.position++] = (T)element;
        }

        public override void WriteData(DataStream target)
        {
            if (this.position == 0)
            {
                return;
            }

            target.WriteRange(this.elements, 0, this.position);
        }

        public override void Clear()
        {
            if (this.position <= 0)
            {
                return;
            }

            Array.Clear(this.elements, 0, this.position - 1);
            this.position = 0;
        }
    }
}
