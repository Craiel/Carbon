namespace Core.Engine.Scene
{
    using System;

    using Core.Engine.Contracts.Scene;

    public class EntityNode : Node, IEntityNode
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EntityNode(ISceneEntity entity)
        {
            this.Name = entity.Name;
            this.Entity = entity;

            this.ResetTransformations();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ISceneEntity Entity { get; set; }

        public void ResetTransformations()
        {
            this.Position = this.Entity.Position;
            this.Rotation = this.Entity.Rotation;
            this.Scale = this.Entity.Scale;
        }

        public override bool Update(Utils.Contracts.ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            //

            return true;
        }
    }
}
