namespace GrandSeal.Editor.Logic.Docking
{
    using System.Collections.Generic;

    using CarbonCore.ToolFramework.ViewModel;

    using GrandSeal.Editor.Contracts;

    public class DocumentTemplateCategory : BaseViewModel, IDocumentTemplateCategory
    {
        public DocumentTemplateCategory()
        {
            this.Children = new List<IDocumentTemplateCategory>();
        }

        public string Name { get; set; }

        public IList<IDocumentTemplateCategory> Children { get; private set; }

        public bool Contains(IDocumentTemplateCategory category)
        {
            foreach (IDocumentTemplateCategory child in this.Children)
            {
                if (child == category || child.Contains(category))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
