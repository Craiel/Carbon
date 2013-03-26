using System;
using System.Collections.Generic;
using System.Linq;

using SlimDX.DirectInput;

namespace Carbon.Engine.Logic
{
    public enum InputBindingModifierMode
    {
        And,
        Or
    }

    public class InputBindingEntry
    {
        public string Value { get; set; }

        public InputBindingModifierMode ModifierMode { get; set; }
        public Key[] Modifiers { get; set; }
    }

    public class InputBindings
    {
        private readonly IDictionary<Key, IList<InputBindingEntry>> bindings;

        private readonly List<Key> usedModifiers;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public InputBindings()
        {
            this.bindings = new Dictionary<Key, IList<InputBindingEntry>>();
            this.usedModifiers = new List<Key>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReadOnlyCollection<Key> UsedModifiers
        {
            get
            {
                return this.usedModifiers.AsReadOnly();
            }
        }

        public InputBindingEntry Bind(string keyName, string value, object[] modifierNames = null)
        {
            Key key = this.GetKey(keyName);
            if (!this.bindings.ContainsKey(key))
            {
                this.bindings.Add(key, new List<InputBindingEntry>());
            }

            Key[] modifiers = null;
            if (modifierNames != null && modifierNames.Length > 0)
            {
                modifiers = new Key[modifierNames.Length];
                for (int i = 0; i < modifierNames.Length; i++)
                {
                    modifiers[i] = this.GetKey(modifierNames[i] as string);
                    if (!this.usedModifiers.Contains(modifiers[i]))
                    {
                        this.usedModifiers.Add(modifiers[i]);
                    }
                }
            }

            var entry = new InputBindingEntry { Value = value, Modifiers = modifiers };
            this.bindings[key].Add(entry);
            return entry;
        }

        public InputBindingEntry BindOr(string keyName, string value, object[] modifierNames = null)
        {
            var entry = this.Bind(keyName, value, modifierNames);
            entry.ModifierMode = InputBindingModifierMode.Or;
            return entry;
        }

        public InputBindingEntry[] GetBindings(string keyName)
        {
            Key key = this.GetKey(keyName);
            if (!this.bindings.ContainsKey(key) || this.bindings[key].Count <= 0)
            {
                return null;
            }

            return this.bindings[key].ToArray();
        }

        public InputBindingEntry[] GetBindings(Key key)
        {
            if (!this.bindings.ContainsKey(key) || this.bindings[key].Count <= 0)
            {
                return null;
            }

            return this.bindings[key].ToArray();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private Key GetKey(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("null or empty binding name");
            }

            return (Key)Enum.Parse(typeof(Key), name);
        }
    }
}
