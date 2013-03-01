﻿using System;
using System.IO;

namespace Carbon.Engine.Logic
{
    public class CarbonBinaryFormatter : IDisposable
    {
        private readonly System.Text.ASCIIEncoding stringEncoding = new System.Text.ASCIIEncoding();
        private readonly Stream target;
        private readonly MemoryStream bufferStream;

        private byte[] buffer = new byte[1024];

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonBinaryFormatter(Stream target)
        {
            this.target = target;
            this.bufferStream = new MemoryStream();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public long Position
        {
            get
            {
                return this.bufferStream.Position;
            }
        }

        public void Flush()
        {
            this.bufferStream.Position = 0;
            this.bufferStream.WriteTo(this.target);
            this.bufferStream.SetLength(0);
        }

        public void Dispose()
        {
            this.Flush();
            this.bufferStream.Dispose();
        }

        public bool ReadBoolean()
        {
            this.SafeRead(1);
            return BitConverter.ToBoolean(this.buffer, 0);
        }

        public float ReadSingle()
        {
            this.SafeRead(4);
            return BitConverter.ToSingle(this.buffer, 0);
        }

        public short ReadShort()
        {
            this.SafeRead(2);
            return BitConverter.ToInt16(this.buffer, 0);
        }

        public int ReadInt()
        {
            this.SafeRead(4);
            return BitConverter.ToInt32(this.buffer, 0);
        }

        public uint ReadUInt()
        {
            this.SafeRead(4);
            return BitConverter.ToUInt32(this.buffer, 0);
        }

        public string ReadString()
        {
            int length = this.ReadInt();
            if (length == 0)
            {
                return null;
            }

            this.SafeRead(length);
            return this.stringEncoding.GetString(this.buffer, 0, length);
        }

        public void Write(bool value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Buffer.BlockCopy(data, 0, this.buffer, 0, data.Length);
            this.SafeWrite(data.Length);
        }

        public void Write(Single value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Buffer.BlockCopy(data, 0, this.buffer, 0, data.Length);
            this.SafeWrite(data.Length);
        }

        public void Write(short value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Buffer.BlockCopy(data, 0, this.buffer, 0, data.Length);
            this.SafeWrite(data.Length);
        }

        public void Write(int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Buffer.BlockCopy(data, 0, this.buffer, 0, data.Length);
            this.SafeWrite(data.Length);
        }

        public void Write(uint value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Buffer.BlockCopy(data, 0, this.buffer, 0, data.Length);
            this.SafeWrite(data.Length);
        }

        public void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                this.Write(0);
                return;
            }

            this.Write(value.Length);
            byte[] data = stringEncoding.GetBytes(value);
            Buffer.BlockCopy(data, 0, this.buffer, 0, data.Length);
            this.SafeWrite(data.Length);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void SafeRead(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Read called with zero length");
            }

            if (this.buffer.Length < count)
            {
                buffer = new byte[count * 2];
            }

            int bytesRead = this.bufferStream.Read(buffer, 0, count);
            if (bytesRead != count)
            {
                throw new IOException(string.Format("Expected to read {0} bytes but got {1}", count, bytesRead));
            }
        }

        private void SafeWrite(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Write called with zero length");
            }

            this.bufferStream.Write(this.buffer, 0, count);
        }
    }
}
