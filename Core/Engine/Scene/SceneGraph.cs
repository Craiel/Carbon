namespace Core.Engine.Scene
{
    using Core.Engine.Contracts.Scene;

    public interface ISceneGraph : ISceneSpatialStructure
    {
        void Add(INode node, INode parent = null);
        void Remove(INode child, INode parent = null);

        void Clear(INode parent = null);
    }

    public class SceneGraph : SceneSpatialStructure, ISceneGraph
    {
        private readonly INode root;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneGraph()
        {
            this.root = new Node { Name = "Root" };
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Add(INode node, INode parent = null)
        {
            base.Add(node);

            if (parent == null)
            {
                this.root.AddChild(node);
            }
            else
            {
                parent.AddChild(node);
            }
        }

        public void Remove(INode child, INode parent = null)
        {
            base.Remove(child);

            if (parent == null)
            {
                this.root.RemoveChild(child);
            }
            else
            {
                parent.RemoveChild(child);
            }
        }

        public void Clear(INode parent = null)
        {
            this.DoClear(parent ?? this.root);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoClear(INode node)
        {
            if (node.Children != null)
            {
                foreach (INode child in node.Children)
                {
                    this.DoClear(child);
                }
            }

            base.Remove(node);
        }
    }
}
