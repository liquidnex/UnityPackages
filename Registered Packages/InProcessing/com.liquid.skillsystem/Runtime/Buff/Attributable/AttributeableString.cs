namespace Liquid.SkillSystem
{
    /// <summary>
    /// Attributable type for struct.
    /// </summary>
    public class AttributableString : IAttributable<string>
    {
        /// <summary>
        /// Default conversion constructor.
        /// </summary>
        public AttributableString(string v)
        {
            value = v;
        }

        /// <summary>
        /// Access the attribute value.
        /// </summary>
        public string Extract()
        {
            return (string)value.Clone();
        }

        public static implicit operator AttributableString(string v)
        {
            return new AttributableString(v);
        }

        public static implicit operator string(AttributableString one)
        {
            return one.value;
        }

        /// <summary>
        /// Access the original value.
        /// </summary>
        public string Value
        {
            get => value;
        }

        protected string value;
    }
}