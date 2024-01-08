namespace Liquid.SkillSystem
{
    /// <summary>
    /// Attributable type for struct.
    /// </summary>
    /// <typeparam name="T">Original struct type.</typeparam>
    public class AttributableStruct<T> : IAttributable<T>
        where T : struct
    {
        /// <summary>
        /// Default conversion constructor.
        /// </summary>
        public AttributableStruct(T v)
        {
            value = v;
        }

        /// <summary>
        /// Access the attribute value.
        /// </summary>
        public T Extract()
        {
            return value;
        }

        public static implicit operator AttributableStruct<T>(T v)
        {
            return new AttributableStruct<T>(v);
        }

        public static implicit operator T(AttributableStruct<T> one)
        {
            return one.value;
        }

        /// <summary>
        /// Access the original value.
        /// </summary>
        public T Value
        {
            get => value;
        }

        protected T value;
    }
}