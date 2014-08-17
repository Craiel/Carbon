namespace Core.Engine.UserInterface
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using CarbonCore.Utils.Contracts.IoC;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Contracts.UserInterface;
    using Core.Engine.Logic;
    using Core.Engine.Resource.Resources;
    using Core.Protocol.Resource;

    public class UserInterface : EngineComponent, IUserInterface
    {
        private readonly IFactory factory;
        private readonly UserInterfaceResource data;

        private readonly IList<IUserInterfaceControl> controls;
        private readonly IDictionary<string, IUserInterfaceControl> namedControls;
        private readonly IDictionary<IUserInterfaceControl, IUserInterfaceControl> parentDictionary;

        private readonly List<ISceneEntity> entities;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UserInterface(IFactory factory, UserInterfaceResource data)
        {
            this.factory = factory;
            this.data = data;

            this.entities = new List<ISceneEntity>();
            this.controls = new List<IUserInterfaceControl>();
            this.namedControls = new Dictionary<string, IUserInterfaceControl>();
            this.parentDictionary = new Dictionary<IUserInterfaceControl, IUserInterfaceControl>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ReadOnlyCollection<ISceneEntity> Entities
        {
            get
            {
                return this.entities.AsReadOnly();
            }
        }

        public void Initialize()
        {
            this.ReloadCsaml();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ReloadCsaml()
        {
            // Clear out the old data
            this.controls.Clear();
            this.namedControls.Clear();
            this.parentDictionary.Clear();

            this.BuildControls(this.data.CsamlData.NodesList, null);
        }

        private void BuildControls(IEnumerable<CsamlNode> nodes, IUserInterfaceControl parent)
        {
            foreach (CsamlNode node in nodes)
            {
                // Create the control for this node
                IUserInterfaceControl currentControl;
                switch (node.Type)
                {
                    case CsamlNode.Types.CsamlNodeType.Image:
                        {
                            currentControl = this.BuildImageControl(node);
                            break;
                        }

                    case CsamlNode.Types.CsamlNodeType.Frame:
                        {
                            currentControl = this.BuildFrameControl(node);
                            break;
                        }

                    case CsamlNode.Types.CsamlNodeType.Console:
                        {
                            currentControl = this.BuildConsoleControl(node);
                            break;
                        }

                    default:
                        {
                            throw new InvalidDataException("Unknown Node Type: " + node.Type);
                        }
                }

                // See if this node is named, if so add it to our dictionary
                if (!string.IsNullOrEmpty(currentControl.Name))
                {
                    if (this.namedControls.ContainsKey(currentControl.Name))
                    {
                        throw new InvalidDataException("Node with the same name was already present: " + currentControl.Name);
                    }

                    this.namedControls.Add(currentControl.Name, currentControl);
                }

                // Add the node to the hierarchy check
                if (parent != null)
                {
                    this.parentDictionary.Add(currentControl, parent);
                }
                
                // Add the control to the general list
                this.controls.Add(currentControl);

                // Process the children
                if (node.ChildrenCount > 0)
                {
                    this.BuildControls(node.ChildrenList, currentControl);
                }
            }
        }

        private IUserInterfaceImage BuildImageControl(CsamlNode source)
        {
            var control = this.factory.Resolve<IUserInterfaceImage>();
            this.SetBasicAttributes(control, source.AttributesList);

            return control;
        }

        private IUserInterfaceFrame BuildFrameControl(CsamlNode source)
        {
            var control = this.factory.Resolve<IUserInterfaceFrame>();
            this.SetBasicAttributes(control, source.AttributesList);

            return control;
        }

        private IUserInterfaceConsole BuildConsoleControl(CsamlNode source)
        {
            var control = this.factory.Resolve<IUserInterfaceConsole>();
            this.SetBasicAttributes(control, source.AttributesList);

            return control;
        }

        private void SetBasicAttributes(IUserInterfaceControl control, IEnumerable<CsamlAttribute> source)
        {
            foreach (CsamlAttribute attribute in source)
            {
                switch (attribute.Type)
                {
                    case CsamlAttribute.Types.CsamlAttributeType.ControlName:
                        {
                            control.Name = attribute.ValueString;
                            continue;
                        }
                }
            }
        }
    }
}