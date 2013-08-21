namespace Core.Engine.Scene
{
    using System;

    public class InvalidSceneStateException : Exception
    {
        public InvalidSceneStateException()
        {
        }

        public InvalidSceneStateException(string message = null, Exception e = null)
            : base(message, e)
        {
            this.Exception = e;
        }

        public Exception Exception { get; private set; }
    }
}
