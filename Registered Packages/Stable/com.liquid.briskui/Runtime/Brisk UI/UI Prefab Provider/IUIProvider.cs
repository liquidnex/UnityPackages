using System.Collections.Generic;
using UnityEngine;

namespace Liquid.BriskUI
{
    /// <summary>
    /// Implement this interface to provide ui prefab gameobject.
    /// </summary>
    public interface IUIProvider
    {
        public List<string> Labels { get; }

        public GameObject Get(string address);
    }
}