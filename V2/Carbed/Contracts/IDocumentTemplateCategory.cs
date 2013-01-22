using System.Collections.Generic;

namespace Carbed.Contracts
{
    public interface IDocumentTemplateCategory
    {
        string Name { get; set; }

        IList<IDocumentTemplateCategory> Children { get; }

        bool Contains(IDocumentTemplateCategory category);
    }
}
