using System;
using System.Collections.Generic;
using System.Linq;

using SlimDX.DirectInput;

namespace Carbon.Engine.Logic
{
    public enum KeyBindingModifierMode
    {
        And,
        Or
    }

    public class KeyBindingEntry
    {
        public string Value { get; set; }

        public KeyBindingModifierMode ModifierMode { get; set; }
        public Key[] Modifiers { get; set; }
    }

    public class KeyBindings
    {
        private readonly IDictionary<Key, IList<KeyBindingEntry>> bindings;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public KeyBindings()
        {
            this.bindings = new Dictionary<Key, IList<KeyBindingEntry>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public KeyBindingEntry Bind(string keyName, string value, object[] modifierNames = null)
        {
            Key key = this.GetKey(keyName);
            if (!this.bindings.ContainsKey(key))
            {
                this.bindings.Add(key, new List<KeyBindingEntry>());
            }

            Key[] modifiers = null;
            if (modifierNames != null && modifierNames.Length > 0)
            {
                modifiers = new Key[modifierNames.Length];
                for (int i = 0; i < modifierNames.Length; i++)
                {
                    modifiers[i] = this.GetKey(modifierNames[i] as string);
                }
            }

            var entry = new KeyBindingEntry { Value = value, Modifiers = modifiers };
            this.bindings[key].Add(entry);
            return entry;
        }

        public KeyBindingEntry BindOr(string keyName, string value, object[] modifierNames = null)
        {
            var entry = this.Bind(keyName, value, modifierNames);
            entry.ModifierMode = KeyBindingModifierMode.Or;
            return entry;
        }

        public KeyBindingEntry[] GetBindings(string keyName)
        {
            Key key = this.GetKey(keyName);
            if (!this.bindings.ContainsKey(key) || this.bindings[key].Count <= 0)
            {
                return null;
            }

            return this.bindings[key].ToArray();
        }

        public KeyBindingEntry[] GetBindings(Key key)
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
