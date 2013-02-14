using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Engine.Contracts;
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
{
    public class ModelViewModel : ResourceViewModel, IModelViewModel
    {
        private readonly ResourceEntry data;

        private ICommand commandSelectFile;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModelViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
            this.data = data;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string FileName
        {
            get
            {
                return this.data.SourcePath;
            }
        }

        public ICommand CommandSelectFile
        {
            get
            {
                return this.commandSelectFile ?? (this.commandSelectFile = new RelayCommand(this.OnSelectFile));
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnSelectFile(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
