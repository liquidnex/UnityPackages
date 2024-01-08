namespace Liquid.SkillSystem
{
    /// <summary>
    /// The type that implements IAttributable contains characteristic values that can be changed.
    /// The characteristic value extracted by it are the focus of the buff computing system.
    /// </summary>
    /// <typeparam name="Attr">Characteristic type.</typeparam>
    public interface IAttributable<Attr>
    {
        /// <summary>
        /// Extract characteristic value in deepcopy of every attributable object.
        /// </summary>
        /// <returns>Characteristic value.</returns>
        public Attr Extract();
    }
}