using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Editor.Contracts;
using Carbon.Editor.Logic;
using Carbon.Editor.Resource;
using Carbon.Engine.Contracts;

using Microsoft.Win32;

namespace Carbed.ViewModels
{
    public class ModelViewModel : FolderContentViewModel, IModelViewModel
    {
        private const string MeshExtension = ".dae";

        private readonly ICarbonBuilder builder;
        private readonly SourceModel data;

        private ICommand commandSelectFile;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModelViewModel(IEngineFactory factory, SourceModel data)
            : base(factory, data)
        {
            this.data = data;
            this.builder = factory.Get<ICarbonBuilder>();
            
            this.Template = StaticResources.ModelTemplate;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                return this.data.Name ?? "<no name>";
            }
        }
        
        public string FileName
        {
            get
            {
                if (this.data.SourceFile == null)
                {
                    return "none selected";
                }

                return this.data.SourceFile.Reference;
            }

            private set
            {
                if (this.data.SourceFile != null && this.data.SourceFile.Reference == value)
                {
                    return;
                }

                this.data.SourceFile = new SourceReference { Reference = value, Type = SourceReferenceType.File };
                this.NotifyPropertyChanged();
            }
        }
        
        public ICommand CommandSelectFile
        {
            get
            {
                return this.commandSelectFile ?? (this.commandSelectFile = new RelayCommand(this.OnSelectFile));
            }
        }

        private void OnSelectFile(object arg)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                CheckPathExists = true,
                CheckFileExists = true,
                DefaultExt = MeshExtension
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            this.FileName = dialog.FileName;
        }
    }
}
