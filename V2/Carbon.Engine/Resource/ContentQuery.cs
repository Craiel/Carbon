
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public sealed class ContentQuery<T> where T : ICarbonContent
    {
        private readonly List<ContentCriterion> criteria;
        private readonly List<ContentOrder> order; 
 
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        

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
            var criterion = new ContentCriterion { PropertyInfo = ContentReflection.GetPropertyInfo<T>(property), Type = CriterionType.Equals };
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
    }
}
