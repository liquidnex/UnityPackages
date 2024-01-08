namespace Liquid.SkillSystem
{
    /// <summary>
    /// Preset struct type buffable field.
    /// </summary>
    /// <typeparam name="T">Struct type.</typeparam>
    public class BuffableFieldStruct<T> : BuffableField<AttributableStruct<T>, T>
        where T : struct
    {
        /// <summary>
        /// Construct buffable field by a struct value.
        /// </summary>
        /// <param name="initValue">Initial value.</param>
        public BuffableFieldStruct(T initValue)
            : base(initValue) {}

        /// <summary>
        /// Apply a attribute to the value.
        /// </summary>
        /// <param name="v">Attribute value.</param>
        protected override void ApplyAttr(T v)
        {
            value = v;
        }

        public static implicit operator BuffableFieldStruct<T>(T v)
        {
            return new BuffableFieldStruct<T>(v);
        }

        public static implicit operator T(BuffableFieldStruct<T> one)
        {
            return one.value;
        }
    }
}