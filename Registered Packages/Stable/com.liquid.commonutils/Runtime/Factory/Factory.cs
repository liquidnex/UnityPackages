using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// This is a template of the factory type.
    /// </summary>
    /// <typeparam name="F">The type of factory.</typeparam>
    /// <typeparam name="P">The type of product.</typeparam>
    public class Factory<F, P>
        where F : Factory<F, P>
        where P : class, IProduct
    {
        protected List<P> products = new List<P>();

        /// <summary>
        /// The original product list.
        /// </summary>
        /// <value>The original product list.</value>
        public List<P> Products
        {
            get => products;
        }

        /// <summary>
        /// Creates an object instance with a type name and construction parameters into the factory.
        /// </summary>
        /// <param name="typeName">
        /// Describes the type of object to create.
        /// The parameter "typeName" can be the type name qualified by its namespace,
        /// or an assembly-qualified name that includes an assembly name specification(AssemblyQualifiedName).
        /// </param>
        /// <param name="args">Construction parameters of object to create.</param>
        /// <returns>Created instance.</returns>
        public P Create(string typeName, params object[] args)
        {
            if (typeName == null)
                return null;

            Type type = Type.GetType(typeName);
            return Create(type, args);
        }

        /// <summary>
        /// Creates an object instance with a assembly specified type name and construction parameters into the factory.
        /// </summary>
        /// <param name="asm">The Assembly for the lookup of type name.</param>
        /// <param name="typeName">
        /// Describes the type of object to create.
        /// The parameter "typeName" should be the type name qualified by the "asm".
        /// </param>
        /// <param name="args">Construction parameters of object to create.</param>
        /// <returns>Created instance.</returns>
        public P Create(Assembly asm, string typeName, params object[] args)
        {
            if (asm == null ||
                typeName == null)
                return null;

            Type type = asm.GetType(typeName);
            return Create(type, args);
        }

        /// <summary>
        /// Creates an object instance with the specified type "T" and construction parameters into the factory.
        /// </summary>
        /// <param name="args">Construction parameters of object to create.</param>
        /// <returns>Created instance.</returns>
        public T Create<T>(params object[] args)
            where T : class, P
        {
            Type t = typeof(T);
            return Create(t, args) as T;
        }

        /// <summary>
        /// Creates an object instance with a specified type and construction parameters into the factory.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="args">Construction parameters of object to create.</param>
        /// <returns>Created instance.</returns>
        public P Create(Type type, params object[] args)
        {
            if (type == null)
                return null;

            if (!typeof(P).IsAssignableFrom(type))
                return null;

            object creation;

            var createMethod = type.GetMethod("ProductCreate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (createMethod != null)
            {
                creation = createMethod.Invoke(null, new object[] { this, args });
            }
            else
            {
                try
                {
                    creation = type.Assembly.CreateInstance(type.FullName, false, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, args, null, null);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                    return null;
                }
            }

            P product = creation as P;
            if (product != null && !products.Contains(product))
                products.Add(product);

            var onCreateMethod = type.GetMethod("OnProductCreate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (onCreateMethod != null)
            {
                onCreateMethod.Invoke(product, new object[] { });
            }

            return product;
        }

        /// <summary>
        /// Remove an existing product from the factory.
        /// </summary>
        /// <param name="product">The product to remove.</param>
        /// <returns>
        /// Returns true when the product is successfully removed.
        /// Returns false when no product is found or removed.
        /// </returns>
        public bool Remove<T>(T product)
        {
            if (product == null)
                return false;

            if (!(product is P p))
                return false;

            if (products.Contains(p))
            {
                Type type = p.GetType();
                var destroyMethod = type.GetMethod("ProductDestroy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (destroyMethod != null)
                {
                    bool isDestroyAvailable = (bool)destroyMethod.Invoke(null, new object[] { this, product });
                    if (isDestroyAvailable)
                        products.Remove(p);
                }
                else
                {
                    products.Remove(p);
                }

                var onDestroyMethod = type.GetMethod("OnProductDestroy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (onDestroyMethod != null)
                {
                    onDestroyMethod.Invoke(product, new object[] { });
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Remvoe all products related to a given type in the factory.
        /// </summary>
        /// <param name="isFuzzyMatch">
        /// If isfuzzymatch is true, fuzzy search is enabled.
        /// All instances of the given type and its derived types will be removed.
        /// Otherwise, only instances of the given type will be removed.
        /// </param>
        /// <typeparam name="T">The specific type to remove.</typeparam>
        public void RemoveAll<T>(bool isFuzzyMatch = false)
            where T : P
        {
            Type t = typeof(T);
            RemoveAll(t, isFuzzyMatch);
        }

        /// <summary>
        /// Remvoe all products related to a given type in the factory.
        /// </summary>
        /// <param name="type">The specific type to remove.</param>
        /// <param name="isFuzzyMatch">
        /// If isfuzzymatch is true, fuzzy search is enabled.
        /// All instances of the given type and its derived types will be removed.
        /// Otherwise, only instances of the given type will be removed.
        /// </param>
        public void RemoveAll(Type type, bool isFuzzyMatch = false)
        {
            if (type == null)
                return;

            if (!typeof(P).IsAssignableFrom(type))
                return;

            List<P> matchProducts = FindAll(type, isFuzzyMatch);
            foreach (P m in matchProducts)
            {
                Remove(m);
            }
        }

        /// <summary>
        /// Remove all products that satisfy the expression "matches".
        /// </summary>
        /// <param name="matches">A filter, and the filtered products will be removed.</param>
        public void RemoveAll(Predicate<P> matches)
        {
            List<P> matchProducts = FindAll(matches);
            foreach (P m in matchProducts)
            {
                Remove(m);
            }
        }

        /// <summary>
        /// Delete all products managed by the factory.
        /// </summary>
        public void Clear()
        {
            products.Clear();
        }

        /// <summary>
        /// Find product by GUID.
        /// </summary>
        /// <param name="productGUID">Unique number of the product.</param>
        /// <returns>Product search result.</returns>
        public P GetProductByGUID(Guid productGUID)
        {
            return products.Find(p => p.ProductGUID == productGUID);
        }

        /// <summary>
        /// Find the first product with matching type from the factory.
        /// </summary>
        /// <param name="isFuzzyMatch">
        /// If isfuzzymatch is true, fuzzy search is enabled.
        /// All instances of the given type and its derived types will be searched.
        /// Otherwise, only instances of the given type will be searched.
        /// </param>
        /// <typeparam name="T">The specific type for searching.</typeparam>
        /// <returns>Product search result.</returns>
        public T Find<T>(bool isFuzzyMatch = false)
            where T : class, P
        {
            Type t = typeof(T);
            return Find(t, isFuzzyMatch) as T;
        }

        /// <summary>
        /// Find the first product with matching type from the factory.
        /// </summary>
        /// <param name="type">The specific type for searching.</param>
        /// <param name="isFuzzyMatch">
        /// If isfuzzymatch is true, fuzzy search is enabled.
        /// All instances of the given type and its derived types will be searched.
        /// Otherwise, only instances of the given type will be searched.
        /// </param>
        /// <returns>Product search result.</returns>
        public P Find(Type type, bool isFuzzyMatch = false)
        {
            if (type == null)
                return null;

            if (!typeof(P).IsAssignableFrom(type))
                return null;

            foreach (P v in products)
            {
                if (v == null)
                    continue;

                bool isMatch;
                if (isFuzzyMatch)
                {
                    isMatch = type.IsAssignableFrom(v.GetType());
                }
                else
                {
                    isMatch = (type == v.GetType());
                }

                if(isMatch)
                    return v;
            }
            return null;
        }

        /// <summary>
        /// Find the first product that satisfy the expression "matches".
        /// </summary>
        /// <param name="matches">A filter, and the filtered product will be searched.</param>
        /// <returns>Product search result.</returns>
        public P Find(Predicate<P> matches)
        {
            if (matches == null)
                return null;

            return products.Find(matches);
        }

        /// <summary>
        /// Find all products related to a given type in the factory.
        /// </summary>
        /// <param name="isFuzzyMatch">
        /// If isfuzzymatch is true, fuzzy search is enabled.
        /// All instances of the given type and its derived types will be searched.
        /// Otherwise, only instances of the given type will be searched.
        /// </param>
        /// <typeparam name="T">The specific type for searching.</typeparam>
        /// <returns>Product list of search result.</returns>
        public List<T> FindAll<T>(bool isFuzzyMatch = false)
            where T : P
        {
            Type t = typeof(T);
            return FindAll(t, isFuzzyMatch).
                ConvertAll(n => (T)n);
        }

        /// <summary>
        /// Find all products related to a given type in the factory.
        /// </summary>
        /// <param name="type">The specific type for searching.</param>
        /// <param name="isFuzzyMatch">
        /// If isfuzzymatch is true, fuzzy search is enabled.
        /// All instances of the given type and its derived types will be searched.
        /// Otherwise, only instances of the given type will be searched.
        /// </param>
        /// <returns>Product list of search result.</returns>
        public List<P> FindAll(Type type, bool isFuzzyMatch = false)
        {
            if (type == null)
                return null;

            if (!typeof(P).IsAssignableFrom(type))
                return null;

            List<P> matches = new List<P>();
            foreach (P v in products)
            {
                bool isMatch;
                if (isFuzzyMatch)
                {
                    isMatch = type.IsAssignableFrom(v.GetType());
                }
                else
                {
                    isMatch = (type == v.GetType());
                }

                if (isMatch)
                    matches.Add(v);
            }

            return matches;
        }

        /// <summary>
        /// Find the all products that satisfy the expression "matches".
        /// </summary>
        /// <param name="matches">A filter, and the filtered products will be searched.</param>
        /// <returns>Product list of search result.</returns>
        public List<P> FindAll(Predicate<P> matches)
        {
            return products.FindAll(matches);
        }

        /// <summary>
        /// Count all products related to a given type in the factory.
        /// </summary>
        /// <param name="isFuzzyMatch">
        /// If isfuzzymatch is true, fuzzy search is enabled.
        /// All instances of the given type and its derived types will be counted.
        /// Otherwise, only instances of the given type will be counted.
        /// </param>
        /// <typeparam name="T">The specific type for counting.</typeparam>
        /// <returns>The total count result.</returns>
        public int Count<T>(bool isFuzzyMatch = false)
            where T : P
        {
            Type t = typeof(T);
            return Count(t, isFuzzyMatch);
        }
        
        /// <summary>
        /// Count all products related to a given type in the factory.
        /// </summary>
        /// <param name="type">The specific type for counting.</param>
        /// <param name="isFuzzyMatch">
        /// If isfuzzymatch is true, fuzzy search is enabled.
        /// All instances of the given type and its derived types will be counted.
        /// Otherwise, only instances of the given type will be counted.
        /// </param>
        /// <returns>The total count result.</returns>
        public int Count(Type type, bool isFuzzyMatch = false)
        {
            if (type == null)
                return 0;

            if (!typeof(P).IsAssignableFrom(type))
                return 0;

            int count = 0;
            foreach (P v in products)
            {
                bool isMatch;
                if (isFuzzyMatch)
                {
                    isMatch = type.IsAssignableFrom(v.GetType());
                }
                else
                {
                    isMatch = (type == v.GetType());
                }

                if (isMatch)
                    ++count;
            }

            return count;
        }

        /// <summary>
        /// Count all products that satisfy the expression "matches".
        /// </summary>
        /// <param name="matches">A filter, and the filtered products will be counted.</param>
        /// <returns>The total count result.</returns>
        public int Count(Predicate<P> matches)
        {
            return products.FindAll(matches).Count;
        }

        /// <summary>
        /// Count all products in the factory.
        /// </summary>
        /// <returns>The total count result.</returns>
        public int Count()
        {
            return products.Count;
        }
    }
}