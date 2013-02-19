using System.Collections.Generic;

using Carbed.Contracts;

using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
    public class MaterialExplorerViewModel : ContentExplorerViewModel, IMaterialExplorerViewModel
    {
        private readonly ICarbedLogic logic;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MaterialExplorerViewModel(IEngineFactory factory, ICarbedLogic logic)
            : base(factory, logic)
        {
            this.logic = logic;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoUpdate(List<ICarbedDocument> target)
        {
            foreach (IMaterialViewModel material in this.logic.Materials)
            {
                target.Add(material);
            }
        }
    }
}
