using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Carbed.Logic;

namespace Carbed.Views
{
    public sealed partial class TaskProgress
    {
        private static TaskProgress instance;

        private readonly Task[] tasks;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public TaskProgress(Task[] tasks, int maxProgress = 0)
        {
            this.tasks = tasks;
            this.Owner = Application.Current.MainWindow;
            this.MaxProgress = maxProgress;

            if (instance != null)
            {
                instance.Close();
            }

            instance = this;
            instance.DataContext = this;

            this.InitializeComponent();

            this.tasks = tasks;
            Task.Factory.StartNew(this.ProgressLoop);
            this.ShowDialog();
        }

        public static string Message { get; set; }
        public static string CurrentMessage { get; set; }

        public static int Progress { get; set; }
        public static int CurrentProgress { get; set; }
        public static int CurrentMaxProgress { get; set; }

        public int MaxProgress { get; private set; }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ProgressLoop()
        {
            foreach (Task task in this.tasks)
            {
                task.Start();
            }

            while (this.tasks.Any(x => !x.IsCompleted))
            {
                this.Dispatcher.Invoke(this.Update);
                CarbedLogic.DoEvents(Application.Current.Dispatcher);

                try
                {
                    Task.WaitAll(this.tasks, 100);
                }
                catch (Exception)
                {
                    break;
                }
            }

            this.Dispatcher.BeginInvoke(new Action(this.CloseDialog));
        }

        private void CloseDialog()
        {
            this.Close();

            instance = null;
        }

        private void Update()
        {
            this.MessageControl.Text = Message;
            this.ProgressControl.Value = Progress;

            this.CurrentMessageControl.Text = CurrentMessage;
            if (string.IsNullOrEmpty(this.CurrentMessageControl.Text))
            {
                this.CurrentMessageControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CurrentMessageControl.Visibility = Visibility.Visible;
            }

            this.CurrentProgressControl.Maximum = CurrentMaxProgress;
            this.CurrentProgressControl.Value = CurrentProgress;

            if (this.CurrentProgressControl.Maximum <= 0)
            {
                this.CurrentProgressControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CurrentProgressControl.Visibility = Visibility.Visible;
            }
        }
    }
}
