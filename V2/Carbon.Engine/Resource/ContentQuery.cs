using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
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
    }

    public class ContentQuery
    {
        private readonly List<ContentCriterion> criteria;
        private readonly List<ContentOrder> order;

        private readonly IList<ContentReflectionProperty> eligibleProperties;
 
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ContentQuery(Type type)
        {
            this.criteria = new List<ContentCriterion>();
            this.order = new List<ContentOrder>();

            this.Type = type;

            this.eligibleProperties = ContentReflection.GetPropertyInfos(type);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

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

        public ContentQuery IsEqual(string property, object value)
        {
            var criterion = new ContentCriterion { PropertyInfo = this.PropertyCheck(property), Type = CriterionType.Equals };
            return this.AddCriterion(criterion);
        }
 
        public ContentQuery AddCriterion(ContentCriterion criterion)
        {
            if (this.criteria.Contains(criterion))
            {
                throw new ArgumentException("Criterion was already added");
            }

            this.criteria.Add(criterion);
            return this;
        }

        public ContentQuery AddOrder(ContentOrder entry)
        {
            if (this.order.Contains(entry))
            {
                throw new ArgumentException("Order Criterion was already added");
            }

            this.order.Add(entry);
            return this;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private PropertyInfo PropertyCheck(string propertyName)
        {
            var entry = this.eligibleProperties.FirstOrDefault(x => x.Name.Equals(propertyName));
            if (entry == null)
            {
                throw new ArgumentException("Property was not found on underlying content object: " + propertyName);
            }

            return entry.Info;
        }
    }
}
