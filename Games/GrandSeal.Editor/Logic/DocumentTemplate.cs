using System;
using System.Collections.Generic;
using System.Windows.Input;

using GrandSeal.Editor.Contracts;

namespace GrandSeal.Editor.Logic
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
