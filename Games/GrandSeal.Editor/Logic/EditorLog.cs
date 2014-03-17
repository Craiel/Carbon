namespace GrandSeal.Editor.Logic
{
    using CarbonCore.Utils.Diagnostics;

    using GrandSeal.Editor.Contracts;

    public class EditorLog : LogBase, IEditorLog
    {
        public EditorLog()
            : base("GrandSeal.Editor")
        {
        }
    }
}
