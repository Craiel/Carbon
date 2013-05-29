using System;
using System.Reflection;
using System.Text;
using Core.Utils;

namespace Core.Engine.Rendering
{
    public sealed class FrameStatistics
    {
        private static readonly PropertyInfo[] Properties = typeof(FrameStatistics).GetProperties();
        private readonly StringBuilder traceBuilder;
        private static ulong FrameCounter;

        public FrameStatistics()
        {
            this.traceBuilder = new StringBuilder();
            this.StartFrame();
        }

        public ulong Id { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public double Duration { get; private set; }
        public ulong Triangles { get; set; }
        public ulong Instructions { get; set; }
        public ulong CulledInstructions { get; set; }
        public ulong InstructionsRendered { get; set; }
        public ulong InstructionsDiscarded { get; set; }
        public ulong InstanceCount { get; set; }
        public ulong MeshSwitches { get; set; }
        public ulong ShaderSwitches { get; set; }
        public ulong DrawIndexedCalls { get; set; }
        public ulong DrawInstancedCalls { get; set; }
        public ulong DrawIndexedInstancedCalls { get; set; }
        public ulong InstanceLimitExceeded { get; set; }

        public bool Ended { get; private set; }

        public void Trace()
        {
            this.traceBuilder.AppendLine("Frame Statistic");
            
            foreach (PropertyInfo property in Properties)
            {
                this.traceBuilder.AppendFormat("  {0}: {1}\n", property.Name, property.GetValue(this, null));
            }

            System.Diagnostics.Trace.TraceInformation(this.traceBuilder.ToString());
        }

        public void StartFrame()
        {
            this.Ended = false;
            this.Id = ++FrameCounter;
            this.StartTime = Timer.CoreTimer.ElapsedTime;

            this.Duration = 0;
            this.Triangles = 0;
            this.Instructions = 0;
            this.CulledInstructions = 0;
            this.InstructionsRendered = 0;
            this.InstructionsDiscarded = 0;
            this.InstanceCount = 0;
            this.MeshSwitches = 0;
            this.ShaderSwitches = 0;
            this.DrawIndexedCalls = 0;
            this.DrawInstancedCalls = 0;
            this.DrawIndexedInstancedCalls = 0;
            this.InstanceLimitExceeded = 0;
        }

        public void EndFrame()
        {
            this.Duration = (Timer.CoreTimer.ElapsedTime - this.StartTime).TotalMilliseconds;

            this.Ended = true;
        }
    }
}
