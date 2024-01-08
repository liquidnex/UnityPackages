using System;
using System.Linq;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// Extension of the System.Type.
    /// It is used to judge the inheritance relationship related to generic classes / generic interfaces.
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Tests whether a given type inherits a specific generic interface.
        /// </summary>
        /// <param name="type">Type to test.</param>
        /// <param name="generic">Generic interface type.</param>
        /// <returns></returns>
        public static bool HasImplementedRawGeneric(this Type type, Type generic)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (generic == null) throw new ArgumentNullException(nameof(generic));

            return type.GetInterfaces().Any(x => generic == (x.IsGenericType ? x.GetGenericTypeDefinition() : x));
        }

        /// <summary>
        /// Tests whether a given type inherits a specific generic type.
        /// </summary>
        /// <param name="type">Type to test.</param>
        /// <param name="generic">Generic class type.</param>
        /// <returns></returns>
        public static bool IsSubClassOfRawGeneric(this Type type, Type generic)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (generic == null) throw new ArgumentNullException(nameof(generic));

            if (IsTheRawGenericType(type))
                return true;
            else if (type != null && type != typeof(object))
                return type.BaseType.IsSubClassOfRawGeneric(generic);
            else
                return false;

            bool IsTheRawGenericType(Type test) =>
                generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test) ||
                HasImplementedRawGeneric(type, generic);
        }
    }
}