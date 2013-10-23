namespace Core.Engine.Scene
{
    using Core.Engine.Contracts.Scene;

    public interface ISceneGraph : ISceneSpatialStructure
    {
        ISceneEntity Root { get; }

        void Add(ISceneEntity entity, ISceneEntity parent = null);
        void Remove(ISceneEntity child, ISceneEntity parent = null);

        void Clear(ISceneEntity parent = null);

        void Append(ISceneGraph graph);
        void AppendInto(ISceneGraph graph, string name);
    }

    public class SceneGraph : SceneSpatialStructure, ISceneGraph
    {
        private readonly ISceneEntity root;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneGraph(ISceneEntity root)
        {
            this.root = root;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ISceneEntity Root
        {
            get
            {
                return this.root;
            }
        }

        public void Add(ISceneEntity entity, ISceneEntity parent = null)
        {
            base.Add(entity);

            if (parent == null)
            {
                entity.AddParent(this.root);
            }
            else
            {
                entity.AddParent(parent);
            }
        }

        public void Remove(ISceneEntity child, ISceneEntity parent = null)
        {
            base.Remove(child);

            if (parent == null)
            {
                child.AddParent(this.root);
            }
            else
            {
                child.AddParent(parent);
            }
        }

        public void Clear(ISceneEntity parent = null)
        {
            this.DoClear(parent ?? this.root);
        }

        public void AppendInto(ISceneGraph graph, string name)
        {
            ISceneEntity subentity = new EmptyEntity { Name = name };
            this.Add(subentity, this.root);
            foreach (ISceneEntity child in graph.Root.Children)
            {
                this.AppendRecursive(subentity, child);
            }
        }

        public void Append(ISceneGraph graph)
        {
            foreach (ISceneEntity child in graph.Root.Children)
            {
                this.AppendRecursive(this.root, child);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void AppendRecursive(ISceneEntity parent, ISceneEntity entity)
        {
            ISceneEntity clone = entity.Clone();
            this.Add(clone, parent);
            if (entity.Children != null)
            {
                foreach (ISceneEntity child in entity.Children)
                {
                    this.AppendRecursive(clone, child);
                }
            }
        }

        private void DoClear(ISceneEntity entity)
        {
            if (entity.Children != null)
            {
                foreach (ISceneEntity child in entity.Children)
                {
                    this.DoClear(child);
                }
            }

            base.Remove(entity);
        }
    }
}
