
namespace Carbon.Engine.Resource
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;

    using Carbon.Engine.Contracts.Resource;

    public enum CriterionType
    {
        Equals
    }

    public struct ContentCriterion
    {
        public PropertyInfo PropertyInfo { get; set; }
        public CriterionType Type { get; set; }
    }

    public struct ContentOrder
    {
        public PropertyInfo PropertyInfo { get; set; }
    }

    public sealed class ContentQuery<T> where T : ICarbonContent
    {
        // Todo: Is static in the context of a generic global or per generic evaluation
        private static IDictionary<Type, IDictionary<string, PropertyInfo>> propertyLookupCache;

        private readonly List<ContentCriterion> criteria;
        private readonly List<ContentOrder> order; 
 
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static ContentQuery()
        {
            propertyLookupCache = new Dictionary<Type, IDictionary<string, PropertyInfo>>();
        }

        public ContentQuery()
        {
            this.criteria = new List<ContentCriterion>();
            this.order = new List<ContentOrder>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ReadOnlyCollection<ContentCriterion> Criterion
        {
            get
            {
                return this.criteria.AsReadOnly();
            }
        }

        public ReadOnlyCollection<ContentOrder> Order
        {
            get
            {
                return this.order.AsReadOnly();
            }
        }

        public ContentQuery<T> IsEqual(string property, object value)
        {
            var criterion = new ContentCriterion { PropertyInfo = GetPropertyInfo(property), Type = CriterionType.Equals };
            return this.AddCriterion(criterion);
        }
 
        public ContentQuery<T> AddCriterion(ContentCriterion criterion)
        {
            if (this.criteria.Contains(criterion))
            {
                throw new ArgumentException("Criterion was already added");
            }

            this.criteria.Add(criterion);
            return this;
        }

        public ContentQuery<T> AddOrder(ContentOrder entry)
        {
            if (this.order.Contains(entry))
            {
                throw new ArgumentException("Order Criterion was already added");
            }

            this.order.Add(entry);
            return this;
        }

        private static PropertyInfo GetPropertyInfo(string propertyName)
        {
            Type type = typeof(T);

            if (!propertyLookupCache.ContainsKey(type))
            {
                propertyLookupCache.Add(type, new Dictionary<string, PropertyInfo>());
            }

            if (!propertyLookupCache[type].ContainsKey(propertyName))
            {
                PropertyInfo info = type.GetProperty(propertyName);
                if (info == null)
                {
                    throw new InvalidOperationException("Property was not found on type for criteria");
                }

                propertyLookupCache[type].Add(propertyName, info);
            }

            return propertyLookupCache[type][propertyName];
        }
    }
}
