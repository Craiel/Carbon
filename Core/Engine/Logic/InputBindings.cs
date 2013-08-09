namespace Core.Engine.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public enum InputBindingModifierMode
    {
        And,
        Or
    }

    public enum InputBindingTriggerMode
    {
        Press,
        PressAndRelease,
        Release,
        HeldOnly,
        Always
    }

    public class InputBindingEntry
    {
        public string Value { get; set; }

        public InputBindingTriggerMode TriggerMode { get; set; }

        public InputBindingModifierMode ModifierMode { get; set; }
        public string[] Modifiers { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class InputBindings
    {
        private readonly IDictionary<string, IList<InputBindingEntry>> bindings;

        private readonly List<string> usedModifiers;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public InputBindings()
        {
            this.bindings = new Dictionary<string, IList<InputBindingEntry>>();
            this.usedModifiers = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReadOnlyCollection<string> UsedModifiers
        {
            get
            {
                return this.usedModifiers.AsReadOnly();
            }
        }
        
        public InputBindingEntry Bind(string keyName, string value, object[] modifierNames = null)
        {
            return this.DoBind(keyName, value, modifierNames);
        }

        public InputBindingEntry BindEx(string keyName, string value, string trigger, string mode, object[] modifierNames = null)
        {
            var entry = this.Bind(keyName, value, modifierNames);

            InputBindingModifierMode modifierMode;
            Enum.TryParse(mode, out modifierMode);
            entry.ModifierMode = modifierMode;

            InputBindingTriggerMode triggerMode;
            Enum.TryParse(trigger, out triggerMode);
            entry.TriggerMode = triggerMode;

            return entry;
        }

        public InputBindingEntry[] GetBindings(string entry)
        {
            if (!this.bindings.ContainsKey(entry) || this.bindings[entry].Count <= 0)
            {
                return null;
            }

            return this.bindings[entry].ToArray();
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private InputBindingEntry DoBind(string entryName, string value, object[] modifierNames = null)
        {
            if (!this.bindings.ContainsKey(entryName))
            {
                this.bindings.Add(entryName, new List<InputBindingEntry>());
            }

            string[] modifiers = null;
            if (modifierNames != null && modifierNames.Length > 0)
            {
                modifiers = new string[modifierNames.Length];
                for (int i = 0; i < modifierNames.Length; i++)
                {
                    modifiers[i] = modifierNames[i] as string;
                    if (!this.usedModifiers.Contains(modifiers[i]))
                    {
                        this.usedModifiers.Add(modifiers[i]);
                    }
                }
            }

            var entry = new InputBindingEntry
            {
                Value = value,
                Modifiers = modifiers,
                TriggerMode = InputBindingTriggerMode.PressAndRelease
            };
            this.bindings[entryName].Add(entry);
            return entry;
        }
    }
}
