namespace Liquid.SkillSystem
{
    /// <summary>
    /// Preset struct type buffable field.
    /// </summary>
    /// <typeparam name="T">Struct type.</typeparam>
    public class BuffableFieldString : BuffableField<AttributableString, string>
    {
        /// <summary>
        /// Construct buffable field by a struct value.
        /// </summary>
        /// <param name="initValue">Initial value.</param>
        public BuffableFieldString(string initValue)
            : base(initValue) {}

        /// <summary>
        /// Apply a attribute to the value.
        /// </summary>
        /// <param name="v">Attribute value.</param>
        protected override void ApplyAttr(string v)
        {
            value = v;
        }

        public static implicit operator BuffableFieldString(string v)
        {
            return new BuffableFieldString(v);
        }

        public static implicit operator string(BuffableFieldString one)
        {
            return one.value;
        }
    }
}