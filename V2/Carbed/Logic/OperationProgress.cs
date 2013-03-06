using Carbed.Contracts;

namespace Carbed.Logic
{
    public class OperationProgress : CarbedBase, IOperationProgress
    {
        private int minimum;

        private int maximum;

        private int value;

        private bool inProgress;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Minimum
        {
            get
            {
                return this.minimum;
            }

            set
            {
                if (this.minimum != value)
                {
                    this.minimum = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public int Maximum
        {
            get
            {
                return this.maximum;
            }

            set
            {
                if (this.maximum != value)
                {
                    this.maximum = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public int Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool InProgress
        {
            get
            {
                return this.inProgress;
            }

            set
            {
                if (this.inProgress != value)
                {
                    this.inProgress = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
    }
}
