using Carbed.Contracts;

namespace Carbed.Events
{
    public class EventLoadLayout
    {
        public EventLoadLayout(IMainViewModel mainViewModel, string file)
        {
            this.MainViewModel = mainViewModel;
            this.File = file;
        }

        public IMainViewModel MainViewModel { get; private set; }
        public string File { get; private set; }
    }

    public class EventSaveLayout
    {
        public EventSaveLayout(string file)
        {
            this.File = file;
        }

        public string File { get; private set; }
    }

    public class EventWindowClosing
    {
    }
}
