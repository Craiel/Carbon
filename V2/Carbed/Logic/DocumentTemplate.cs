using System;
using System.Collections.Generic;
using System.Windows.Input;

using Carbed.Contracts;

namespace Carbed.Logic
{
    public class DocumentTemplate : IDocumentTemplate
    {
        public DocumentTemplate()
        {
            this.Categories = new List<IDocumentTemplateCategory>();
        }

        public IList<IDocumentTemplateCategory> Categories { get; private set; }

        public string TemplateName { get; set; }

        public string Description { get; set; }

        public Uri IconUri { get; set; }

        public ICommand CommandCreate { get; set; }

        public object CreateParameter { get; set; }
    }
}
