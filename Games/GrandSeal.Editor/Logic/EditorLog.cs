using GrandSeal.Editor.Contracts;

using Core.Utils.Diagnostics;

namespace GrandSeal.Editor.Logic
{
    public class EditorLog : LogBase, IEditorLog
    {
        public EditorLog()
            : base("GrandSeal.Editor")
        {
        }
    }
}
