namespace Core.Engine.Resource.Generic
{
    using Core.Engine.Contracts.Resource;

    public sealed class ContentQuery<T> : ContentQuery where T : ICarbonContent
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ContentQuery()
            : base(typeof(T))
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public new ContentQuery<T> IsEqual(string property, object value)
        {
            base.IsEqual(property, value);
            return this;
        }

        public new ContentQuery<T> Contains(string property, object[] values)
        {
            base.Contains(property, values);
            return this;
        }

        public new ContentQuery<T> Not()
        {
            base.Not();
            return this;
        }

        public new ContentQuery<T> OrderBy(string property, bool ascending = true)
        {
            base.OrderBy(property, ascending);
            return this;
        }
    }
}
