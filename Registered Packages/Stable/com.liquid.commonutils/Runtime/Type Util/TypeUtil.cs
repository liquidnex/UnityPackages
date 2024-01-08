using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// Provides tools for inter type checking and reflection.
    /// </summary>
    public class TypeUtil
    {
        /// <summary>
        /// Traverse all fields of object to find fields that match a specific type.
        /// </summary>
        /// <typeparam name="T">Target object type.</typeparam>
        /// <typeparam name="R">Search field type.</typeparam>
        /// <param name="obj">Target object.</param>
        /// <param name="isFuzzyMatch">
        /// If isfuzzymatch is true, fuzzy search is enabled.
        /// All fields of the given type and its derived types will be indicated.
        /// Otherwise, only field of the given type will be indicated.
        /// </param>
        /// <param name="checkContainer">
        /// Whether to check the container field.
        /// If this option is turned on,
        /// only the direct field members of the container field are checked to see if they match a specific type.
        /// Otherwise skip the container field.
        /// </param>
        /// <returns>Search results for specific types of fields.</returns>
        public static List<R> GetFields<T, R>(T obj, bool checkContainer = false, bool isFuzzyMatch = false)
        {
            return GetFields(obj, typeof(R), checkContainer, isFuzzyMatch).ConvertAll(e => (R)e);
        }

        /// <summary>
        /// Traverse all fields of object to find fields that match a specific type.
        /// </summary>
        /// <typeparam name="T">Target object type.</typeparam>
        /// <param name="obj">Target object.</param>
        /// <param name="searchFieldType">Search field type.</param>
        /// <param name="isFuzzyMatch">
        /// If isfuzzymatch is true, fuzzy search is enabled.
        /// All fields of the given type and its derived types will be indicated.
        /// Otherwise, only field of the given type will be indicated.
        /// </param>
        /// <param name="checkContainer">
        /// Whether to check the container field.
        /// If this option is turned on,
        /// only the direct field members of the container field are checked to see if they match a specific type.
        /// Otherwise skip the container field.
        /// </param>
        /// <returns>Search results for specific types of fields.</returns>
        public static List<object> GetFields<T>(T obj, Type searchFieldType, bool checkContainer = false, bool isFuzzyMatch = false)
        {
            List<object> result = new List<object>();

            if (obj == null ||
                searchFieldType == null)
                return result.Distinct().ToList();

            Type t = obj.GetType();
            if ((!isFuzzyMatch && searchFieldType == t) ||
                (isFuzzyMatch && searchFieldType.IsAssignableFrom(t)))
            {
                result.Add(obj);
                return result.Distinct().ToList();
            }

            FieldInfo[] fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo f in fields)
            {
                object value = f.GetValue(obj);
                if (value == null)
                    continue;

                if ((!isFuzzyMatch && searchFieldType == f.FieldType) ||
                    (isFuzzyMatch && searchFieldType.IsAssignableFrom(f.FieldType)))
                {
                    result.Add(value);
                }
                else if (checkContainer &&
                        value is IEnumerable enumerableObj)
                {
                    foreach (var o in enumerableObj)
                    {
                        if (o == null)
                            continue;

                        Type ot = o.GetType();
                        if ((!isFuzzyMatch && searchFieldType == ot) ||
                            (isFuzzyMatch && searchFieldType.IsAssignableFrom(ot)))
                        {
                            result.Add(o);
                        }
                    }
                }
            }

            return result.Distinct().ToList();
        }
    }
}