namespace Core.Utils.Threading
{
    using Core.Utils.Contracts;

    public class ThreadQueuePayload : IThreadQueueOperationPayload
    {
        public object Data { get; set; }
    }
}
