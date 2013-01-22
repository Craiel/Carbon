using System.Collections.Generic;

using Carbed.Contracts;

namespace Carbed.Logic
{
    class DocumentTemplateCategory : CarbedBase, IDocumentTemplateCategory
    {
        public DocumentTemplateCategory()
        {
            this.Children = new List<IDocumentTemplateCategory>();
        }

        public string Name { get; set; }

        public IList<IDocumentTemplateCategory> Children { get; private set; }

        public bool Contains(IDocumentTemplateCategory category)
        {
            foreach (IDocumentTemplateCategory child in Children)
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
