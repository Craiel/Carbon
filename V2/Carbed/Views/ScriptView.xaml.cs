using System.Collections.Generic;
using System.Windows.Input;

using Carbed.Contracts;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Carbed.Views
{
    public partial class ScriptView
    {
        private readonly IList<ICompletionData> completion;

        private CompletionWindow completionWindow;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ScriptView()
        {
            InitializeComponent();
            
            syntaxEditor.TextArea.TextEntering += this.TextAreaOnTextEntering;
            syntaxEditor.TextArea.TextEntered += this.TextAreaOnTextEntered;

            this.completion = new List<ICompletionData>();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void TextAreaOnTextEntered(object sender, TextCompositionEventArgs args)
        {
            if (args.Text == ".")
            {
                this.completion.Clear();
                string context = this.GetContext(sender as TextArea, 1);
                ((IResourceViewModel)this.DataContext).UpdateAutoCompletion(this.completion, context);

                this.ShowAutoCompletion();
            }
        }

        private void TextAreaOnTextEntering(object sender, TextCompositionEventArgs args)
        {
            if (args.Text == " " && this.completionWindow == null &&
                (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                // Todo: This is very very not good!
                this.completion.Clear();
                ((IResourceViewModel)this.DataContext).UpdateAutoCompletion(this.completion);

                this.ShowAutoCompletion();
                args.Handled = true;
                return;
            }

            if (args.Text.Length > 0 && this.completionWindow != null)
            {
                if (!char.IsLetterOrDigit(args.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    this.completionWindow.CompletionList.RequestInsertion(args);
                }
            }
        }

        private string GetContext(TextArea area, int offset = 0)
        {
            DocumentLine line = area.Document.GetLineByNumber(area.Caret.Line);
            string lineContent = area.Document.Text.Substring(line.Offset, area.Caret.Column - 1);
            string context = string.Empty;
            for (int i = lineContent.Length - (1 + offset); i >= 0; i--)
            {
                if (!char.IsLetterOrDigit(lineContent[i]))
                {
                    break;
                }

                context = lineContent[i] + context;
            }

            return context;
        }

        private void ShowAutoCompletion()
        {
            if (this.completion.Count > 0)
            {
                // Open code completion after the user has pressed dot:
                this.completionWindow = new CompletionWindow(syntaxEditor.TextArea);
                foreach (ICompletionData data in this.completion)
                {
                    this.completionWindow.CompletionList.CompletionData.Add(data);
                }

                this.completionWindow.Show();
                this.completionWindow.Closed += delegate
                    { this.completionWindow = null; };
            }
        }
    }
}
