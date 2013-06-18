using System.Collections.Generic;
using System.IO;
using System.Xml;

using Core.Engine.Contracts;
using Core.Engine.Contracts.UserInterface;
using Core.Engine.Logic;
using Core.Engine.Resource.Resources;

namespace Core.Engine.UserInterface
{
    public class UserInterface : EngineComponent, IUserInterface
    {
        private const string NodeImage = "image";
        private const string NodeFrame = "frame";

        private const string AttributeName = "name";

        private readonly IEngineFactory factory;
        private readonly UserInterfaceResource data;

        private readonly IList<IUserInterfaceControl> controls;
        private readonly IDictionary<string, IUserInterfaceControl> namedControls;
        private readonly IDictionary<IUserInterfaceControl, IUserInterfaceControl> parentDictionary;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UserInterface(IEngineFactory factory, UserInterfaceResource data)
        {
            this.factory = factory;
            this.data = data;

            this.controls = new List<IUserInterfaceControl>();
            this.namedControls = new Dictionary<string, IUserInterfaceControl>();
            this.parentDictionary = new Dictionary<IUserInterfaceControl, IUserInterfaceControl>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Initialize()
        {
            this.ReloadCsaml();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ReloadCsaml()
        {
            var document = new XmlDocument();
            document.LoadXml(this.data.CsamlData);

            this.BuildControls(document.ChildNodes, null);
        }

        private void BuildControls(XmlNodeList nodes, IUserInterfaceControl parent)
        {
            foreach (XmlNode node in nodes)
            {
                // Create the control for this node
                IUserInterfaceControl currentControl;
                string key = node.Name.ToLower();
                switch (key)
                {
                    case NodeImage:
                        {
                            currentControl = this.BuildImageControl(node);
                            break;
                        }

                    case NodeFrame:
                        {
                            currentControl = this.BuildFrameControl(node);
                            break;
                        }

                    default:
                        {
                            throw new InvalidDataException("Unknown Node Type: " + key);
                        }
                }

                // See if this node is named, if so add it to our dictionary
                if (!string.IsNullOrEmpty(node.Name))
                {
                    if (this.namedControls.ContainsKey(node.Name))
                    {
                        throw new InvalidDataException("Node with the same name was already present: " + node.Name);
                    }

                    this.namedControls.Add(node.Name, currentControl);
                }

                // Add the node to the hierarchy check
                if (parent != null)
                {
                    this.parentDictionary.Add(currentControl, parent);
                }

                // Process the children
                if (node.ChildNodes.Count > 0)
                {
                    this.BuildControls(node.ChildNodes, currentControl);
                }
            }
        }

        private IUserInterfaceImage BuildImageControl(XmlNode source)
        {
        }

        private IUserInterfaceFrame BuildFrameControl(XmlNode source)
        {
        }

        private IUserInterfaceConsole BuildConsoleControl(XmlNode source)
        {
        }

        private void SetBasicAttributes(IUserInterfaceControl control, XmlAttributeCollection source)
        {
            foreach (XmlAttribute attribute in source)
            {
                string key = attribute.Name.ToLower();
                switch (key)
                {
                    case AttributeName:
                        {
                            control.Name = attribute.Value;
                            continue;
                        }
                }
            }
        }
    }
}