using System;
using System.IO;

using Carbon.Editor.Contracts;
using Carbon.Editor.Logic;
using Carbon.Editor.Resource;
using Carbon.Editor.Resource.Collada;

namespace Carbon.Editor.Processors
{
    public class ModelProcessor : IContentProcessor
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ProcessingResult Process(CarbonBuilderEntry entry)
        {
            /*SourceModel source = (SourceModel)entry.Content;

            if (source.File == null)
            {
                throw new ArgumentException();
            }

            // Todo: resolve file reference
            string file = source.File.Reference;

            byte[] data;
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
            }

            ColladaModel model = ColladaModel.Load(data);
            ColladaCarbonConverter.Convert(file, model);*/
            return null;
        }
    }
}
