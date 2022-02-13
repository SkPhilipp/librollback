using System;

namespace Rollback.Tests
{
    public class ComparableObjectExample : IComparable
    {
        public readonly int Order;

        public ComparableObjectExample(int order)
        {
            Order = order;
        }

        protected bool Equals(ComparableObjectExample other)
        {
            return Order == other.Order;
        }

        public override bool Equals(object obj)
        {
            return obj is ComparableObjectExample coe && Order == coe.Order;
        }

        public override int GetHashCode()
        {
            return Order.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            if (obj is ComparableObjectExample other)
            {
                return Order - other.Order;
            }

            return 1;
        }
    }
}