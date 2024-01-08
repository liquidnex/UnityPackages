using System;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// Product is the base class of product type in factory design pattern.
    /// </summary>
    public abstract class Product: IProduct
    {
        protected Guid productGUID = Guid.NewGuid();

        public Guid ProductGUID
        {
            get => productGUID;
        }
    }
}