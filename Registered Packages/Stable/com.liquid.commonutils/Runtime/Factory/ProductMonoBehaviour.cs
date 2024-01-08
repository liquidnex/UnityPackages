using System;
using UnityEngine;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// ProductMonoBehaviour is the base class of product type in factory mode, which is designed for Unity GameObject.
    /// </summary>
    public class ProductMonoBehaviour : MonoBehaviour, IProduct
    {
        protected Guid productGUID = Guid.NewGuid();

        public Guid ProductGUID
        {
            get => productGUID;
        }
    }
}