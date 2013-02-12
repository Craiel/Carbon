using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Editor.Resource;
using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
    public class ModelViewModel : FolderContentViewModel, IModelViewModel
    {
        private readonly SourceModel data;

        private ICommand commandSelectFile;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModelViewModel(IEngineFactory factory, SourceModel data)
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
                return this.data.File;
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
