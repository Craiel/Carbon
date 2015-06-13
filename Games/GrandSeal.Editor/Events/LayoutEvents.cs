namespace GrandSeal.Editor.Events
{
    using CarbonCore.Utils.Compat.IO;

    using GrandSeal.Editor.Contracts;

    public class EventLoadLayout
    {
        public EventLoadLayout(IMainViewModel mainViewModel, CarbonFile file)
        {
            this.MainViewModel = mainViewModel;
            this.File = file;
        }

        public IMainViewModel MainViewModel { get; private set; }
        public CarbonFile File { get; private set; }
    }

    public class EventSaveLayout
    {
        public EventSaveLayout(CarbonFile file)
        {
            this.File = file;
        }

        public CarbonFile File { get; private set; }
    }

    public class EventWindowClosing
    {
    }
}
