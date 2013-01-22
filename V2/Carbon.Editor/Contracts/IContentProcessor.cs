namespace Carbon.Editor.Contracts
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Carbon.Editor.Logic;

    public sealed class ProcessingResult
    {
        public ProcessingResult(IList<long> content, IList<string> resources)
        {
            this.Content = new ReadOnlyCollection<long>(content);
            this.Resources = new ReadOnlyCollection<string>(resources);
        }

        public ReadOnlyCollection<long> Content { get; private set; }
        public ReadOnlyCollection<string> Resources { get; private set; }
    }

    public interface IContentProcessor
    {
        ProcessingResult Process(CarbonBuilderEntry entry);
    }
}
