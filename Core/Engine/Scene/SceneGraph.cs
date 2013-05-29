using Core.Engine.Contracts.Scene;

namespace Core.Engine.Scene
{
    public class SceneGraph
    {
        private readonly INode root;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneGraph()
        {
            this.root = new Node();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public INode Add(ISceneEntity entity, INode parent = null)
        {
            return null;
        }

        public bool Remove(INode child, INode parent = null)
        {
            return false;
        }

        public void Clear(INode parent = null)
        {
            if (parent == null)
            {
                parent = this.root;
            }

            parent.Clear();
        }
    }
}
