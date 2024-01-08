using System;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// Any type that implements IProduct interface can become the object of factory design pattern management.
    /// </summary>
    public interface IProduct
    {
        /// <summary>
        /// Represents the unique number of products, which can be used to find and distinguish products.
        /// </summary>
        /// <value>The unique guid instance.</value>
        public Guid ProductGUID
        {
            get;
        }
    }
}