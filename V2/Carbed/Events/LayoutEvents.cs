namespace Carbed.Events
{
    public class EventLoadLayout
    {
        public EventLoadLayout(string file)
        {
            this.File = file;
        }

        public string File { get; set; }
    }

    public class EventSaveLayout
    {
        public EventSaveLayout(string file)
        {
            this.File = file;
        }

        public string File { get; set; }
    }

    public class EventWindowClosing
    {
    }
}
