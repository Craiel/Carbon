using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Carbed.Contracts
{
    public interface IDocumentTemplate
    {
        IList<IDocumentTemplateCategory> Categories { get; }

        string TemplateName { get; set; }
        string Description { get; set; }

        Uri IconUri { get; set; }

        ICommand CommandCreate { get; set; }
        object CreateParameter { get; set; }
    }
}
